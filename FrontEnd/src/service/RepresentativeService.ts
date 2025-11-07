import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Representative } from '@/model/Representative';
import type { IHttpService } from './IService/IHttpService';
import type { Page } from '@/model/Page';
import type { IRepresentativeService } from './IService/IRepresentativeService';

@injectable()
export class RepresentativeService implements IRepresentativeService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    async getAll(): Promise<Page<Representative>> {
        const response = await this.http.get<Page<Representative>>('/Representative');
        return response.data;
    }

    async getByEmail(emailAddress: string): Promise<Representative> {
        const response = await this.http.get<Representative>(`/Representative/email/${encodeURIComponent(emailAddress)}`);
        return response.data;
    }

    async getByCitizenId(citizenId: string): Promise<Representative> {
        const response = await this.http.get<Representative>(`/Representative/citizen/${encodeURIComponent(citizenId)}`);
        return response.data;
    }
}