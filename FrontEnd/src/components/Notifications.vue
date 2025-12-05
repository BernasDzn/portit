<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IAdminService } from '@/service/IService/IAdminService';
import type { Logs } from '@/model/values/Logs';
import type { ISystemNotificationService } from '@/service/IService/ISystemNotificationService';
import type { SystemNotification } from '@/model/SystemNotification';
import { useSession } from '@/composables/session';

const user = useSession().authenticatedUser;

const adminService = container.get<IAdminService>(TYPES.adminService);
const notificationService = container.get<ISystemNotificationService>(TYPES.systemNotificationService);

const logs = ref<Logs[]>([]);
const notifications = ref<SystemNotification[]>([]);
const dropdownRef = ref<any>(null);

const activeTab = ref<'notifications' | 'logs'>('notifications');

const closeDropdown = () => {
    dropdownRef.value?.hide();
};

const onDropdownShow = () => {
    if (activeTab.value === 'logs') fetchLogs();
    else fetchNotifications();
};

const fetchLogs = async () => {
    try {
        const logList = await adminService.getLogs();
        logs.value = logList.slice(0, 10);
    } catch (err) {
        console.error('Failed to load audit logs', err);
    }
};

const fetchNotifications = async () => {
    try {
        notifications.value = await notificationService.getSystemNotifications();
    } catch (err) {
        console.error('Failed to load notifications', err);
    }
};

const markAllUnreadAsRead = async () => {
    try {
        const unreadIds = notifications.value
            .filter(n => !n.isRead)
            .map(n => n.id);

        if (unreadIds.length > 0) {
            for (const id of unreadIds) {
                console.log('Marking notification as read:', id);
                await notificationService.markAsRead(id);
            }
        }

        await fetchNotifications();
    } catch (err) {
        console.error('Failed to mark notifications as read', err);
    }
};

onMounted(async () => {
    await fetchNotifications();
});

const levelMap: Record<string, string> = {
    '1000': 'Create',
    '1001': 'Update',
    '1002': 'Delete',
    '1003': 'Deactivate',
    '1004': 'Retrieve',
    '1005': 'Filter'
};

const mappedLogs = computed(() =>
    logs.value.map(l => ({
        ...l,
        requestType: levelMap[String(l.requestId)] ?? String(l.requestId)
    }))
);

const groupedNotifications = computed(() => {
    const unread = notifications.value
        .filter(n => !n.isRead)
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    
    const read = notifications.value
        .filter(n => n.isRead)
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

    // Return a merged list where a "separator" item is inserted
    const list: any[] = [];

    if (unread.length > 0) {
        list.push(...unread);
    }

    if (read.length > 0) {
        // Only show separator if we also have unread
        if (unread.length > 0) {
            list.push({ __separator: true });
        }
        list.push(...read);
    }

    return list;
});

const unreadCount = computed(() =>
    notifications.value.filter(n => !n.isRead).length
);

const selectedNotification = ref<SystemNotification | null>(null);
const detailDialogRef = ref<any>(null);

const openNotification = async (notification: SystemNotification) => {
    selectedNotification.value = notification;
    
    if (!notification.isRead) {
        try {
            await notificationService.markAsRead(notification.id);
            await fetchNotifications();
        } catch (err) {
            console.error('Failed to mark notification as read', err);
        }
    }
    
    detailDialogRef.value?.show();
};

const closeDetailDialog = () => {
    detailDialogRef.value?.hide();
    selectedNotification.value = null;
};

const truncateText = (text: string, maxLength: number = 40) => {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
};

</script>

<template>
    <sl-dropdown ref="dropdownRef" placement="bottom" @sl-show="onDropdownShow">
        <div slot="trigger" class="notifications-info">
            <sl-icon name="bell"></sl-icon>
            <sl-badge v-if="unreadCount > 0" class="counter" variant="danger" pill pulse>{{unreadCount}}</sl-badge>
        </div>

        <sl-menu class="dropdown-content">

            <div class="notification-header">
                <div class="tabs">
                    <buttonz
                        class="tab-btn"
                        :class="{ active: activeTab === 'notifications' }"
                        @click="activeTab = 'notifications'; fetchNotifications()"
                    >
                        Notifications
                    </buttonz>

                    <button
                        class="tab-btn"
                        :class="{ active: activeTab === 'logs' }"
                        @click="activeTab = 'logs'; fetchLogs()"
                        v-if="user?.role === 0"
                    >
                        Logs
                    </button>
                </div>

                <RouterLink
                    v-if="activeTab === 'logs'"
                    to="/admin/audit-logs"
                    class="view-all-link"
                    @click="closeDropdown"
                >
                    View All
                </RouterLink>

                <a
                    v-else
                    href="#"
                    class="view-all-link"
                    @click.prevent="markAllUnreadAsRead"
                > 
                    Mark all as read
                </a>
            </div>

            <ul v-if="activeTab === 'notifications'" class="notification-menu">

                <template v-for="n in groupedNotifications" :key="n.__separator ? 'sep' : n.id">

                    <li v-if="n.__separator" class="separator-item">
                        <div class="separator">Read</div>
                    </li>

                    <li
                        v-else
                        class="notification-item"
                        :class="{ unread: !n.isRead }"
                        @click="openNotification(n)"
                    >
                        <div class="log-entry">
                            <div class="log-content">
                                <div class="log-message">{{ n.title }}</div>
                                 <sl-format-date class="log-timestamp" :date="new Date(n.createdAt)" month="long" day="numeric" year="numeric"></sl-format-date>
                                <div class="log-message small">{{ truncateText(n.message) }}</div>
                            </div>

                            <sl-badge variant="danger" v-if="n.urgency !== 0">
                                Urgent!
                            </sl-badge>
                        </div>
                    </li>

                </template>
            </ul>

            <ul v-if="activeTab === 'logs'" class="notification-menu">
                <li
                    v-for="(log, index) in mappedLogs"
                    :key="`${index}-${log.requestId}-${log.timestamp}-${log.message}`"
                    class="notification-item"
                >
                    <div class="log-entry">
                        <div class="log-content">
                            <div class="log-message">{{ log.message }}</div>
                            <div class="log-timestamp">{{ log.timestamp }}</div>
                        </div>
                        <div class="log-type">
                            <sl-badge :variant="log.level === 'INF' ? 'primary' : log.level === 'WRN' ? 'warning' : 'danger'">
                                {{ log.requestType }}
                            </sl-badge>
                        </div>
                    </div>
                </li>
            </ul>

        </sl-menu>
    </sl-dropdown>

    <sl-dialog ref="detailDialogRef" label="" class="notification-detail-dialog">
        <div v-if="selectedNotification" class="notification-detail">
            <div class="detail-title-row">
                <h3>{{ selectedNotification.title }}</h3>
                <sl-badge variant="danger" v-if="selectedNotification.urgency !== 0">
                    Urgent
                </sl-badge>
            </div>
            <sl-format-date class="detail-timestamp" :date="new Date(selectedNotification.createdAt)" month="long" day="numeric" year="numeric" hour="numeric" minute="numeric"></sl-format-date>
            <p class="detail-message">{{ selectedNotification.message }}</p>
        </div>
        <sl-button slot="footer" variant="primary" @click="closeDetailDialog">Close</sl-button>
    </sl-dialog>
</template>

<style scoped>
.dropdown-content {
    min-width: 400px;
    max-width: 500px;
}

.separator-item {
    padding: 0;
    border-bottom: none;
}

.separator {
    padding: 0.35rem 1rem;
    background: #f0f0f0;
    color: #555;
    font-size: 0.75rem;
    text-transform: uppercase;
    font-weight: 600;
    border-top: 1px solid #e0e0e0;
}

/* Tabs */
.tabs {
    display: flex;
    gap: 0.5rem;
}

.tab-btn {
    padding: 0.3rem 0.75rem;
    background: transparent;
    border: 1px solid #ccc;
    border-radius: 6px;
    cursor: pointer;
    font-size: 0.85rem;
}

.tab-btn.active {
    background: var(--accent-1);
    color: white;
    border-color: var(--accent-1);
}

.notification-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    border-bottom: 1px solid #e0e0e0;
}

.view-all-link {
    color: var(--accent-1);
    text-decoration: none;
    font-size: 0.875rem;
}

.view-all-link:hover {
    text-decoration: underline;
}

.log-entry {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: 0.5rem;
}

.log-content {
    flex: 1;
    min-width: 0;
    overflow: hidden;
}

.log-message {
    font-size: 0.875rem;
    color: #333;
}

.log-message.small {
    font-size: 0.75rem;
    color: #666;
}

.log-timestamp {
    font-size: 0.75rem;
    color: #999;
    margin-top: 0.25rem;
}

.notification-item {
    cursor: pointer;
}

.notification-item:hover {
    background-color: #f5f5f5;
}

.notification-item.unread {
    background-color: #f0f7ff;
}

.notification-item.unread:hover {
    background-color: #e6f2ff;
}

.notification-detail {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.detail-title-row {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.detail-title-row h3 {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
}

.detail-timestamp {
    font-size: 0.85rem;
    color: #999;
    margin-bottom: 0.5rem;
}

.detail-message {
    margin: 0;
    line-height: 1.6;
    color: #333;
}

.notification-menu {
    max-height: 400px;
    overflow-y: auto;
}

.counter {
    position: relative;
    top: -10px;
    right: -1px;
    font-size: 0.65rem;
    width: 0;
}

.counter::part(base) {
    width: auto;
    min-width: 16px;
    height: 16px;
    padding: 10px;
    border: none;
}


</style>
