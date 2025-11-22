import type { STSCraneDto, TruckDto, YardCraneDto } from "@/model/dto/PhysicalResourceDto";
import type { Filter, Page } from "@/model/Page";
import type { PhysicalResource, PhysicalResourceFilter, STSCrane, Truck, YardCrane } from "@/model/PhysicalResource";

export interface IPhysicalResourceService {

    getPhysicalResources(filtering?: Filter<PhysicalResourceFilter>): Promise<Page<any>>;
    getPhysicalResourceById(id: string): Promise<PhysicalResource>;
    getNumberOfPhysicalResources(): Promise<number>;
    deactivatePhysicalResource(id: string): Promise<void>;

    addSTSCrane(value: STSCrane): Promise<STSCrane>;
    addYardCrane(value: YardCrane): Promise<YardCrane>;
    addTruck(value: Truck): Promise<Truck>;

    updateSTSCrane(value: STSCrane): Promise<STSCrane>;
    updateYardCrane(value: YardCrane): Promise<YardCrane>;
    updateTruck(value: Truck): Promise<Truck>;
}