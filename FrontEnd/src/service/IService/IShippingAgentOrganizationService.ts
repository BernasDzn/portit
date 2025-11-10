import type { Filter, Page } from "@/model/Page";
import type { ShippingAgentOrganization } from "@/model/ShippingAgentOrganization";

export interface IShippingAgentOrganizationService {
    getShippingAgentOrganizations(): Promise<Page<ShippingAgentOrganization>>
}