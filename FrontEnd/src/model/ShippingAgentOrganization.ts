import type { Address } from "./Address";
import type { Representative } from "./Representative";

export interface ShippingAgentOrganization {
    name: string;
    altNames: string[];
    taxNumber: string;
    address: Address;
    representatives: Representative[];
}