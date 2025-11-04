import type { Filter, Page } from "@/model/Page";
import type { PhysicalResource, PhysicalResourceFilter, STSCrane, Truck, YardCrane } from "@/model/PhysicalResource";

export interface IPhysicalResourceService {
    getPhysicalResources(filtering?: Filter<PhysicalResourceFilter>): Promise<Page<any>>;
    getPhysicalResourceById(id: string): Promise<PhysicalResource>;
    deactivatePhysicalResource(id: string): Promise<void>;

    addSTSCrane(value: STSCrane): Promise<STSCrane>;
    addYardCrane(value: YardCrane): Promise<YardCrane>;
    addTruck(value: Truck): Promise<Truck>;

    updateSTSCrane(code: string, value: STSCrane): Promise<STSCrane>;
    updateYardCrane(code: string, value: YardCrane): Promise<YardCrane>;
    updateTruck(code: string, value: Truck): Promise<Truck>;
}