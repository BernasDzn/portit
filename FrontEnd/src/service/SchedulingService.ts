import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Representative } from '@/model/Representative';
import type { IHttpService } from './IService/IHttpService';
import type { ISchedulingService } from './IService/ISchedulingService';
import type { Schedule } from '@/model/Schedule';

@injectable()
export class SchedulingService implements ISchedulingService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) { }

    scheduleForDay(day: Date): Promise<Schedule> {
        return this.http.get<Schedule>('/schedule').then(res => res.data);
    }
}