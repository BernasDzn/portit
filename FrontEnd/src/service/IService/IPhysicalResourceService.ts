import type { Filter, Page } from "@/model/Page";
import type { PhysicalResource, PhysicalResourceFilter } from "@/model/PhysicalResource";

export interface IPhysicalResourceService {
    getPhysicalResources(filtering?: Filter<PhysicalResourceFilter>): Promise<Page<any>>;
    getPhysicalResourceById(id: string): Promise<PhysicalResource>;
    addPhysicalResource(value: PhysicalResource): Promise<PhysicalResource>;
    updatePhysicalResource(id: string, value: PhysicalResource): Promise<PhysicalResource>;
    deactivatePhysicalResource(id: string): Promise<void>;
}