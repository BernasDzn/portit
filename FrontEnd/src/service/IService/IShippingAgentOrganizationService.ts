import type { Filter, Page } from "@/model/Page";
import type { ShippingAgentOrganization } from "@/model/ShippingAgentOrganization";

export interface IShippingAgentOrganizationService {
    getAll(): Promise<Page<ShippingAgentOrganization>>
}