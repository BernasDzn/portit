import { ref, onMounted } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { ISystemNotificationService } from '@/service/IService/ISystemNotificationService';
import type { SystemNotification } from '@/model/SystemNotification';

export const useUrgentBroadcasts = () => {
    const notificationService = container.get<ISystemNotificationService>(TYPES.systemNotificationService);
    const broadcasts = ref<SystemNotification[]>([]);

    const fetchBroadcasts = async () => {
        try {
            const all = await notificationService.getSystemNotifications();
            broadcasts.value = all.filter(n => n.urgency === 1 && !n.isRead);
        } catch (err) {
            console.error('Failed to load urgent broadcasts', err);
        }
    };

    onMounted(fetchBroadcasts);

    return { broadcasts, fetchBroadcasts };
};