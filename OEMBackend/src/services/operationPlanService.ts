import mongoose from "mongoose";
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
import { TaskCategory } from "../domain/taskCategory";
import config from "../config/config";
import { Payload } from "../domain/value/payload";


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

	async createPlans(plansData: any, createdBy: string): Promise<OperationPlanDto[]> {
		const savedPlans: OperationPlanDto[] = [];
		const scheduleDataDto = ScheduleDataMapper.toDto(plansData);
		
		const baseDate = new Date(scheduleDataDto.date);

        const unloadCategory = await taskCategoryRepository.getCategoryByCode('UNLOAD');
        const loadCategory = await taskCategoryRepository.getCategoryByCode('LOAD');

        if (!unloadCategory || !loadCategory) {
            throw new Error('Required task categories UNLOAD or LOAD not found');
        }
		
		for (const dockData of scheduleDataDto.data) {
			const dockCode = dockData.dock;
			
			for (const vesselSchedule of dockData.schedule) {
				const operationSchedule = new LinkedList<Operation>();
				
				const craneResources = vesselSchedule.cranes.map(craneName => 
					new Resource({
						name: craneName,
						type: ResourceType.Crane
					})
				);
				
				const unloadStartTime = new Date(baseDate.getTime() + vesselSchedule.unloading_enter_time * 60 * 60 * 1000);
				const unloadEndTime = new Date(baseDate.getTime() + vesselSchedule.unloading_exit_time * 60 * 60 * 1000);
				
				const unloadOperation = new Operation({
					operationType: unloadCategory!,
					startTime: unloadStartTime,
					endTime: unloadEndTime,
					resources: craneResources,
					payload: new Payload({}) // Set each attribute if really needed
				});
				operationSchedule.insertAtEnd(unloadOperation);
				
				const loadStartTime = new Date(baseDate.getTime() + vesselSchedule.loading_enter_time * 60 * 60 * 1000);
				const loadEndTime = new Date(baseDate.getTime() + vesselSchedule.loading_exit_time * 60 * 60 * 1000);
				
				const loadOperation = new Operation({
					operationType: loadCategory!,
					startTime: loadStartTime,
					endTime: loadEndTime,
					resources: craneResources,
					payload: new Payload({})
				});
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