import type { PrivacyPolicy } from "@/model/PrivacyPolicy";

export interface IPrivacyPolicyService {

    getActivePrivacyPolicy(): Promise<PrivacyPolicy>;
    getAllPrivacyPolicies(): Promise<PrivacyPolicy[]>;
    updatePrivacyPolicy(content: PrivacyPolicy): Promise<PrivacyPolicy>;
}