import type { STSCraneDto, TruckDto, YardCraneDto } from "@/model/dto/PhysicalResourceDto";
import type { Filter, Page } from "@/model/Page";
import type { PhysicalResource, PhysicalResourceFilter, STSCrane, Truck, YardCrane } from "@/model/PhysicalResource";

export interface IPhysicalResourceService {

    getPhysicalResources(filtering?: Filter<PhysicalResourceFilter>): Promise<Page<any>>;
    getPhysicalResourceById(id: string): Promise<PhysicalResource>;
    getNumberOfPhysicalResources(): Promise<number>;
    deactivatePhysicalResource(id: string): Promise<void>;

    addSTSCrane(value: STSCraneDto): Promise<STSCrane>;
    addYardCrane(value: YardCraneDto): Promise<YardCrane>;
    addTruck(value: TruckDto): Promise<Truck>;

    updateSTSCrane(value: STSCraneDto): Promise<STSCrane>;
    updateYardCrane(value: YardCraneDto): Promise<YardCrane>;
    updateTruck(value: TruckDto): Promise<Truck>;
}