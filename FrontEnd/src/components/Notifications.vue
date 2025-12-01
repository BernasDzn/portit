<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IAdminService } from '@/service/IService/IAdminService';
import type { Logs } from '@/model/values/Logs';

const adminService = container.get<IAdminService>(TYPES.adminService);

const logs = ref<Logs[]>([]);
const moreInfo = ref(false);

const toggleMoreInfo = () => {
    moreInfo.value = !moreInfo.value;
};

const fetchLogs = async () => {
    try {
        const logList: Logs[] = await adminService.getLogs();
        logs.value = logList
            .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
            .slice(0, 10);
    } catch (err) {
        console.error('Failed to load audit logs', err);
    }
};

onMounted(() => {
    void fetchLogs();
});

watch(moreInfo, (isOpen) => {
    if (isOpen) {
        void fetchLogs();
    }
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

</script>

<template>
    <div>
        <div @click="toggleMoreInfo" class="notifications-info">
            <sl-icon name="bell"></sl-icon>
        </div>

        <div class="info-popup">
            <sl-popup placement="bottom" :active="moreInfo">
                <span slot="anchor"></span>
                <div class="box">
                    <div class="notification-header">
                        <h3>Recent Audit Logs</h3>
                        <RouterLink to="/admin/audit-logs" class="view-all-link">View All</RouterLink>
                    </div>
                    <ul class="notification-menu" scrollable>
                        <li v-for="log in mappedLogs" :key="log.requestId + log.timestamp" class="notification-item">
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
                </div>
            </sl-popup>
        </div>
    </div>
</template>

<style scoped>
.info-popup {
    z-index: 1000;
}

.box {
    z-index: 1000;
}

.notification-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 1rem;
    border-bottom: 1px solid #e0e0e0;
}

.notification-header h3 {
    margin: 0;
    font-size: 1rem;
    font-weight: 600;
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
    align-items: center;
    gap: 0.5rem;
}

.log-content {
    flex: 1;
    min-width: 0;
}

.log-type {
    display: flex;
    align-items: center;
    flex-shrink: 0;
}

.log-message {
    font-size: 0.875rem;
    color: #333;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.log-timestamp {
    font-size: 0.75rem;
    color: #666;
}

.notification-menu {
    max-height: 400px;
    overflow-y: auto;
}

.notification-item {
    padding: 0.75rem 1rem;
    border-bottom: 1px solid #f0f0f0;
}

.notification-item:last-child {
    border-bottom: none;
}

.notification-item:hover {
    background-color: #f5f5f5;
}
</style>