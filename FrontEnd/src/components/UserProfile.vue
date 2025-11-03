<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import type { User } from '@/model/User';
import { AuthService } from '@/service/AuthService';
import AxiosHttpService from '@/service/AxiosHttpService';
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

const http = new AxiosHttpService();
const authService = new AuthService(http);

const notifications = useAlerts();
const router = useRouter();

const user = ref<User>({
    id: '1',
    name: 'Monokuma',
    email: 'monoemail@hopes.peak',
    avatar: '/monouser.png'
});
const moreInfo = ref(false);

const toggleMoreInfo = () => {
    moreInfo.value = !moreInfo.value;
    animateChevron();
};

const animateChevron = () => {
    const icon = document.querySelector('.icon') as HTMLElement;
    if (moreInfo.value) {
        icon.style.transform = 'rotate(-180deg)';
        icon.style.transition = 'transform 0.2s ease';
    } else {
        icon.style.transform = 'rotate(0deg)';
        icon.style.transition = 'transform 0.2s ease';
    }
};

onMounted(async () => {

    try {
        user.value = await authService.whoAmI();
    } catch (error: any) {

        if (error.status === 401)
            notifications.enqueueNotification('Session could not be found', notifications.notificationTypes.WARNING);
        else
            notifications.enqueueNotification(`Unexpected error fetching user data: ${error.message}`, notifications.notificationTypes.DANGER);

        router.push('/unauthorized');
    }
});

</script>

<template>
<div>
    <div @click="toggleMoreInfo" class="user-info">
        <sl-avatar 
            :image="user.avatar"
            label="User avatar"
            loading="lazy"
        ></sl-avatar>
        <p>{{user.name}}</p>
        <sl-icon class="icon" name="chevron-down"></sl-icon>
    </div>

    <div class="info-popup">
        <sl-popup placement="bottom-start" shift shift-padding="10" :active="moreInfo" >
            <span slot="anchor"></span>
            <div class="box">
                <p class="title"><sl-badge variant="primary" pill>Admin</sl-badge> {{user.name}} </p>
                <p class="subtitle">{{ user.email }}</p>
                <div class="logout-box">
                    <RouterLink to="/login">
                        <sl-button class="logout-button" variant="danger" outline>Logout</sl-button>
                    </RouterLink>
                </div>
            </div>
        </sl-popup>
    </div>
</div>
</template>
