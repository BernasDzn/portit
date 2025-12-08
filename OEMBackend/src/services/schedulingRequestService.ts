import config from "../config/config";
import { ScheduleQueueItem } from "../domain/scheduleQueue";
import { OperationPlanDto } from "../dto/operationPlanDto";
import { workQueueRepository } from "../repository/workQueueRepository";
import { OperationPlanService } from "./operationPlanService";

export class SchedulingRequestService {
    
    async scheduleRequest(day: string, alg: string, daysAhead: number = 2, issuer: string): Promise<any> {
        
        const number = await workQueueRepository.enqueueRequest(day, alg, daysAhead, 0, issuer);
        this.getToWork().catch((err) => {
            console.error("Error processing scheduling request:", err);
        });
        
        return {
            message: "Your scheduling request has been queued. You are the number " + (number + 1) + " in the queue."
        };
    }

    async getQueueState(): Promise<ScheduleQueueItem[]> {
        const queueState = await workQueueRepository.getQueueState();
        return queueState;
    }

    async acceptRequest(id: string, issuer: string): Promise<ScheduleQueueItem | null> {

        const item = await workQueueRepository.getById(id);
        if (!item){
            throw new Error(`No scheduling request found with ID ${id}`);
        }

        if (item.status !== 'completed') {
            throw new Error(`Scheduling is not completed. Current status: ${item.status}`);
        }

        if (item.issuer !== issuer) {
            throw new Error(`Scheduling request was not issued by you`);
        }

        const scheduleData = item.result;
        if (!scheduleData) {
            throw new Error(`No schedule data found for request ID ${id}`);
        }
        
        const savedPlan = await new OperationPlanService().createPlans(scheduleData, item.issuer);
        if (!savedPlan || savedPlan.length === 0) {
            throw new Error(`Failed to save operation plans for request ID ${id}`);
        }

        console.log(`Operation Plans saved successfully for request ID ${id}`);
        await workQueueRepository.finishRequest(id, 'accepted');
        return item;
    }

    async rejectRequest(id: string, issuer: string): Promise<ScheduleQueueItem | null> {

        const item = await workQueueRepository.getById(id);
        if (!item){
            throw new Error(`No scheduling request found with ID ${id}`);
        }

        if (item.status !== 'completed') {
            throw new Error(`Scheduling request ID ${id} is not yet completed. Current status: ${item.status}`);
        }

        if (item.issuer !== issuer) {
            throw new Error(`Scheduling request ID ${id} was not issued by you`);
        }

        await workQueueRepository.finishRequest(id, 'rejected');
        return item;
    }

    async getToWork(): Promise<OperationPlanDto | null> {

        const nextItem = await workQueueRepository.dequeueRequest();
        if (!nextItem) // End queue cycle (list is empty)
            return null;

        console.log(`Processing scheduling request ID ${nextItem.id} for day ${nextItem.data.day}`);

        const url = `${config.schedulingServer}/schedule?day=${nextItem.data.day}&alg=${nextItem.data.alg}&daysAhead=${nextItem.data.daysAhead}`;
        
        try {
    
            const res = await fetch(url);
            console.log(res);
            if (!res.ok) {
                // Set result as failed in the queue
                await workQueueRepository.finishRequest(nextItem.id, 'failed');
                return await this.getToWork(); 
            }

            const scheduleData = await res.json();
        
            // Save the schedule
            try {
                // const savedPlan = await new OperationPlanService().createPlans(scheduleData, nextItem.issuer);
                // console.log(`Operation Plans saved successfully for request ID ${nextItem.id}`);

            } catch (error) {
                console.error(`Failed to save schedule:`, error);
                await workQueueRepository.finishRequest(nextItem.id, 'failed');
                return await this.getToWork();
            }
            
            // Mark request as complete
            await workQueueRepository.finishRequest(nextItem.id, 'completed', scheduleData);
            return await this.getToWork();
            
        } catch (error) {

            console.error("Error contacting scheduling server:", error);
            // Set result as failed in the queue
            await workQueueRepository.finishRequest(nextItem.id, 'unavailable');
            
        } finally {
            return await this.getToWork();
        }
    }
}

export const schedulingRequestService = new SchedulingRequestService();