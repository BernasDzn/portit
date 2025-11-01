<script setup lang="ts">
import { ref } from 'vue';

const notifications = ref([
    {
        name: 'Bernardo',
        email: 'bernardo@hopes.peak',
        message_type: 'New vessel added',
        route: '/Vessels'
    },
    {
        name: 'Francis',
        email: 'francis@hopes.peak',
        message_type: 'New dock added',
        route: '/Docks'
    },
    {
        name: 'Taiko',
        email: 'taiko@hopes.peak',
        message_type: 'New qualification added',
        route: '/Notifications'
    },
    {
        name: 'Kray',
        email: 'kray@hopes.peak',
        message_type: 'New qualification added',
        route: '/Notifications'
    }
]);
const moreInfo = ref(false);

const toggleMoreInfo = () => {
    moreInfo.value = !moreInfo.value;
};

</script>

<template>
    <div>
        <div @click="toggleMoreInfo" class="notifications-info">
            <sl-icon class="icon" name="bell"></sl-icon>
        </div>

        <div class="info-popup">
            <sl-popup placement="bottom" :active="moreInfo">
                <span slot="anchor"></span>
                <div class="box">
                    <ul class="notification-menu" scrollable>
                        <li v-for="item in notifications" class="notification-item">
                            <RouterLink :to="item.route" class="notification-menu-link">
                                <!--maybe add later-->
                                <!-- <sl-avatar>{{notification.avatar}}</sl-avatar> -->
                                <div class="name">{{ item.name }}</div>
                                <div class="email">({{ item.email }})</div>
                                <div class="message">{{ item.message_type }}</div>
                            </RouterLink>
                        </li>
                    </ul>
                </div>
            </sl-popup>
        </div>
    </div>
</template>