import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';
import type { IPrivacyPolicyService } from './IService/IPrivacyPolicyService';
import type { IHttpService } from './IService/IHttpService';
import type { PrivacyPolicy } from '@/model/PrivacyPolicy';

@injectable()
export class PrivacyPolicyService implements IPrivacyPolicyService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getActivePrivacyPolicy(): Promise<PrivacyPolicy> {
        
        const res = await this.http.get<PrivacyPolicy>(`/PrivacyPolicy/active`);
        return res.data;
    }

    async updatePrivacyPolicy(content: PrivacyPolicy): Promise<PrivacyPolicy> {
     
        const res = await this.http.post<PrivacyPolicy>(`/PrivacyPolicy`, content.toDto());
        return res.data;
    }

}