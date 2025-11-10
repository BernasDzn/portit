import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { ShippingAgentOrganization } from '@/model/ShippingAgentOrganization';
import type { IHttpService } from './IService/IHttpService';
import type { Page } from '@/model/Page';
import type { IShippingAgentOrganizationService } from './IService/IShippingAgentOrganizationService';

@injectable()
export class ShippingAgentOrganizationService implements IShippingAgentOrganizationService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getShippingAgentOrganizations(): Promise<Page<ShippingAgentOrganization>> {
        const response = await this.http.get<ShippingAgentOrganization[]>('/ShippingAgentOrganization');
        return {
            items: response.data,
            pageNumber: 1,
            pageSize: response.data.length,
            pageCount: 1
        };
    }
}