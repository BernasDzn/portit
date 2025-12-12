import { Service } from "typedi";
import { OperationPlan } from "../domain/operationPlan";
import { Operation } from "../domain/value/operation";
import { OperationPlanMetadata } from "../domain/value/operationPlanMetadata";
import { Resource, ResourceType } from "../domain/value/resource";
import { OperationPlanDto } from "../dto/operationPlanDto";
import { ScheduleDataMapper } from "../mappers/scheduleDataMapper";
import { OperationPlanRepository } from "../repository/operationPlanRepository";
import { taskCategoryRepository } from "../repository/taskCategoryRepository";
import { LinkedList } from "../utils/linkedList";
import { Page, Pageable } from "../utils/page";
import { Payload } from "../domain/value/payload";
import { ContainerDto } from "../dto/container";


@Service("operationPlanService")
export class OperationPlanService {

	operationPlanRepository: OperationPlanRepository;

	constructor() {
		this.operationPlanRepository = new OperationPlanRepository();
	}

	async getAll(pageable: Pageable): Promise<Page<OperationPlanDto>> {
		return await this.operationPlanRepository.getAll(pageable);
	}

	async getById(id: string): Promise<OperationPlanDto | null> {
		return await this.operationPlanRepository.getById(id);
	}

	async getByDateGrouped(): Promise<{ date: string; plans: OperationPlanDto[] }[]> {
		return await this.operationPlanRepository.getByDateGrouped();
	}

	async getNotificationsWithoutPlan(token: string): Promise<string[]> {
		return await this.operationPlanRepository.getNotificationsWithoutPlan(token);
	}

	async createPlans(plansData: any, createdBy: string, token: string): Promise<OperationPlanDto[]> {
		const savedPlans: OperationPlanDto[] = [];
		const scheduleDataDto = ScheduleDataMapper.toDto(plansData);
		
		const baseDate = new Date(scheduleDataDto.date);

		const existingPlans = await this.operationPlanRepository.getByDateGrouped();
		const plansOnDate = existingPlans.find(group => group.date === scheduleDataDto.date);
		for (const plan of plansOnDate?.plans || []) {
			await this.operationPlanRepository.deleteById(plan.id!);
		}

        console.log("Plans Data:", plansData);
		
		for (const dockData of scheduleDataDto.data) {
			const dockCode = dockData.dock;
			
			for (const vesselSchedule of dockData.schedule) {
                console.log("Processing vessel schedule:", vesselSchedule);
				const operationSchedule = new LinkedList<Operation>();
				
				const craneResources = vesselSchedule.cranes.map(craneName => 
					new Resource({
						name: craneName,
						type: ResourceType.Crane
					})
				);
				
				const unloadStartTime = new Date(baseDate.getTime() + vesselSchedule.unloading_enter_time * 60 * 60 * 1000);
				const unloadEndTime = new Date(baseDate.getTime() + vesselSchedule.unloading_exit_time * 60 * 60 * 1000);
				
				const unloadOperations = await this.subdivideOperationIntoContainers(unloadStartTime, unloadEndTime, craneResources, true, token, vesselSchedule.name);
                for (const unloadOperation of unloadOperations) 
                    operationSchedule.insertAtEnd(unloadOperation);
				
				const loadStartTime = new Date(baseDate.getTime() + vesselSchedule.loading_enter_time * 60 * 60 * 1000);
				const loadEndTime = new Date(baseDate.getTime() + vesselSchedule.loading_exit_time * 60 * 60 * 1000);
				
                const loadOperations = await this.subdivideOperationIntoContainers(loadStartTime, loadEndTime, craneResources, false, token, vesselSchedule.name);
                for (const loadOperation of loadOperations) 
                    operationSchedule.insertAtEnd(loadOperation);
				
				const dockIndex = scheduleDataDto.data.indexOf(dockData);
				const metric = scheduleDataDto.metrics[dockIndex];
				
				const metadata = new OperationPlanMetadata({
					createdBy: createdBy,
					createdAt: new Date(),
					algorithmUsed: metric?.algorithm || 'unknown'
				});
				
				const operationPlan = new OperationPlan({
					relatedVVN: vesselSchedule.name,
					dock: dockCode,
					operationSchedule: operationSchedule,
					metadata: metadata
				});
				
				const savedPlan = await this.operationPlanRepository.create(operationPlan);
				savedPlans.push(savedPlan);
			}
		}
		
		return savedPlans;
	}

    async subdivideOperationIntoContainers(startTime: Date, endTime: Date, resources: Resource[], isUnload: boolean, token: string, vvnId: string): Promise<Operation[]> {

        // const loadOperation = new Operation({
        // 	operationType: loadCategory!,
        // 	startTime: loadStartTime,
        // 	endTime: loadEndTime,
        // 	resources: craneResources,
        // 	payload: new Payload({})
        // });
        // operationSchedule.insertAtEnd(loadOperation);

        const unloadCategory = await taskCategoryRepository.getCategoryByCode('UNLOAD');
        const loadCategory = await taskCategoryRepository.getCategoryByCode('LOAD');

        if (!unloadCategory || !loadCategory) {
            throw new Error('Required task categories UNLOAD or LOAD not found');
        }

        // The number of track divisions is based on the number of resources
        // 2 Resources means two containers can be handled in parallel
        const laneCount: number = resources.length;

        // The containers to subdivide the original task into
        let containerList = await this.operationPlanRepository.getContainersOfNotification(vvnId, token, isUnload);
        if (containerList.length == 0) return [];

        const operationsPerLane = Math.ceil(containerList.length / laneCount);

        console.log(containerList);

        // Tier is the z index of the container in the stack
        // Lower tier means it is lower in the stack, for that we can seperate 
        // operations by z, because we only want to proceed to higher/lower tiers (depending if unload/load)
        // once the lower/higher tiers are done
        let containerMap = new Map<number, ContainerDto[]>();
        for (const container of containerList) {
            const tier = container.position.tier;
            if (!containerMap.has(tier)) {
                containerMap.set(tier, []);
            }
            containerMap.get(tier)!.push(container);
        }

        let order = Array.from(containerMap.keys()).sort((a, b) => !isUnload ? a - b : b - a);

        let operationSchedule: Operation[] = [];
        let currentTime = new Date(startTime.getTime());

        const singleOperationDuration = (endTime.getTime() - startTime.getTime()) / containerList.length;

        for (const tier of order) {
            const containersInTier = containerMap.get(tier)!;
            // Create operations
            for (let i = 0; i < containersInTier.length; i++) {

                const container = containersInTier[i];
                const laneIndex = i % laneCount;
                
                // Calculate time for this specific operation
                const operationStartTime = new Date(currentTime.getTime());
                const operationEndTime = new Date(operationStartTime.getTime() + singleOperationDuration);
                
                console.log(`Container assigned to lane ${laneIndex} at tier ${tier}`);
                console.log(`Operation Start: ${operationStartTime}, Operation End: ${operationEndTime}`);        

                const operation = new Operation({
                    operationType: isUnload ? unloadCategory : loadCategory,
                    startTime: operationStartTime,
                    endTime: operationEndTime,
                    resources: resources,
                    payload: new Payload({
                        containerId: container?.containerNumber || '',
                        storageLocation: container?.area || ''
                    })
                });
        
                operationSchedule.push(operation);
                
                // Advance time for next operation
                currentTime = new Date(operationEndTime.getTime());
            }
        }

        console.log("Generated Operations:", operationSchedule);
        return operationSchedule;
    }

	async create(operationPlanDto: OperationPlanDto): Promise<OperationPlanDto> {
		let operationSchedule = new LinkedList<Operation>();
		for (const opDto of operationPlanDto.operationSchedule) {
			const payload = opDto.payload ? new Payload({
				containerId: opDto.payload.containerId,
				storageLocation: opDto.payload.storageLocation
			}) : new Payload({});
			
			const operation = new Operation({
				operationType: (await taskCategoryRepository.getCategoryByCode(opDto.type.category))!,
				startTime: new Date(opDto.startTime),
				endTime: new Date(opDto.endTime),
				resources: opDto.resources.map(resDto => {
					return new Resource({
						name: resDto.name,
						type: ResourceType[resDto.type as keyof typeof ResourceType]
					});
				}),
				payload
			});
			operationSchedule.insertAtEnd(operation);
		}

		let operationPlanMetadata = new OperationPlanMetadata({
			createdBy: operationPlanDto.metadata.createdBy,
			createdAt: new Date(operationPlanDto.metadata.createdAt),
			algorithmUsed: operationPlanDto.metadata.algorithmUsed
		});

		const operationPlan = new OperationPlan({
			dock: operationPlanDto.dock,
			relatedVVN: operationPlanDto.relatedVVN,
			operationSchedule: operationSchedule,
			metadata: operationPlanMetadata
		});

		return await this.operationPlanRepository.create(operationPlan);
	}

}

export const operationPlanService = new OperationPlanService();