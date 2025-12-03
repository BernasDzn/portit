import { inject, injectable } from 'inversify';
import { TYPES } from '@/inversify/types';

import type { Representative } from '@/model/Representative';
import type { IHttpService } from './IService/IHttpService';
import type { ISchedulingService } from './IService/ISchedulingService';
import type { Schedule } from '@/model/values/Schedule';
import jsPDF from 'jspdf';
import { useSession } from '@/composables/session';

@injectable()
export class SchedulingService implements ISchedulingService {

    constructor(
        @inject(TYPES.api)
        private http: IHttpService
    ) {}

    async getQueueState(): Promise<any[]> {
        const res = await this.http.get<any[]>(`/oem/schedule/queueState`);
        return res.data;
    }

    calculateDateOffset(baseDate: Date, offsetHours: number): Date {
        const newDate = new Date(baseDate.getTime() + offsetHours * 60 * 60 * 1000);
        return newDate;
    }    

    async generateSchedulePDF(schedule: Schedule, date: Date): Promise<Uint8Array> {
        const session = useSession();
        const doc = new jsPDF();
        
        // Title
        doc.setFontSize(18);
        doc.text('Schedule Report', 14, 22);
        doc.setFontSize(12);
        doc.text(`Concern: All docks, on day ${date.toDateString()}`, 14, 32);
        
        // Overall metrics
        if (schedule.metrics && schedule.metrics.length > 0) {
            const totalVessels = schedule.metrics.reduce((sum: number, m: any) => sum + (m.vesselCount || 0), 0);
            const totalDelay = schedule.metrics.reduce((sum: number, m: any) => sum + (m.totalDelay || 0), 0);
            const avgComputationTime = schedule.metrics.reduce((sum: number, m: any) => sum + (m.computationTime || 0), 0) / schedule.metrics.length;
            const algorithm = schedule.metrics[0]?.algorithm || 'unknown';
            
            doc.setFontSize(10);
            doc.text(`Algorithm: ${algorithm}`, 14, 40);
            doc.text(`Total Delay: ${totalDelay} hours`, 14, 46);
            doc.text(`Avg Computation Time: ${(avgComputationTime * 1000).toFixed(2)} ms`, 14, 52);
            doc.text(`Total Vessels Scheduled: ${totalVessels}`, 14, 58);
        }
        
        let yPosition = schedule.metrics ? 68 : 42;
        
        // Iterate through each dock
        schedule.data.forEach((dockData: any, index: number) => {
            // Check if we need a new page
            if (yPosition > 250) {
                doc.addPage();
                yPosition = 20;
            }
            
            // Dock header
            doc.setFontSize(14);
            doc.setFont('helvetica', 'bold');
            doc.text(`Dock: ${dockData.dock}`, 14, yPosition);
            yPosition += 8;
            
            // Dock-specific metrics
            if (schedule.metrics && schedule.metrics[index]) {
                const metrics = schedule.metrics[index];
                doc.setFontSize(9);
                doc.setFont('helvetica', 'normal');
                doc.text(
                    `Vessels: ${metrics.vesselCount} | Delay: ${metrics.totalDelay}h | Strategy: ${metrics.strategy} | Time: ${(metrics.computationTime * 1000).toFixed(2)}ms`,
                    14,
                    yPosition
                );
                yPosition += 8;
            }
            
            // Table headers
            doc.setFontSize(10);
            doc.setFont('helvetica', 'bold');
            doc.text('Ship Name', 14, yPosition);
            doc.text('Arrival', 70, yPosition);
            doc.text('Departure', 120, yPosition);
            doc.text('Cranes', 170, yPosition);
            yPosition += 6;
            
            // Table content
            doc.setFont('helvetica', 'normal');
            dockData.schedule.forEach((entry: any) => {
                // Check if we need a new page
                if (yPosition > 280) {
                    doc.addPage();
                    yPosition = 20;
                    
                    // Re-add headers on new page
                    doc.setFont('helvetica', 'bold');
                    doc.text('Ship Name', 14, yPosition);
                    doc.text('Arrival', 70, yPosition);
                    doc.text('Departure', 120, yPosition);
                    doc.text('Cranes', 170, yPosition);
                    yPosition += 6;
                    doc.setFont('helvetica', 'normal');
                }
                
                const shipName = entry.name.replace(/_\d+$/, '');
                const arrival = this.calculateDateOffset(date, entry.loading_enter_time).toLocaleString();
                const departure = this.calculateDateOffset(date, entry.loading_exit_time).toLocaleString();
                const cranes = entry.cranes 
                    ? (Array.isArray(entry.cranes) ? entry.cranes.join(', ') : String(entry.cranes))
                    : '-';
                
                // Truncate long text if needed
                doc.text(this.truncateText(shipName, 50), 14, yPosition);
                doc.text(this.truncateText(arrival, 45), 70, yPosition);
                doc.text(this.truncateText(departure, 45), 120, yPosition);
                doc.text(this.truncateText(cranes, 25), 170, yPosition);
                
                yPosition += 8;
            });
            
            yPosition += 10; // Extra spacing between docks
        });
        
        // Footer on last page
        const pageCount = doc.getNumberOfPages();
        doc.setPage(pageCount);
        doc.setFontSize(8);
        doc.text(
            `Generated by ${session.authenticatedUser?.name || 'Unknown user'} on ${new Date().toLocaleString()}`,
            14,
            290
        );
        
        const arrayBuffer = doc.output('arraybuffer') as ArrayBuffer;
        return new Uint8Array(arrayBuffer);
    }
    
    // Helper method to truncate text
    private truncateText(text: string, maxLength: number): string {
        if (text.length <= maxLength) return text;
        return text.substring(0, maxLength - 3) + '...';
    }

    async scheduleForDay(day: Date, alg: string, daysAhead: number = 2): Promise<any> {
        const dayString = day.toISOString().split('T')[0];
        const res = await this.http.getWithoutCredentials(`/oem/schedule/request?day=${dayString}&alg=${alg}&daysAhead=${daysAhead}`);
        
        return res.data;
    }
}