<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import Logout from './Logout.vue';
import { useSession } from '@/composables/session';
import type { User } from '@/model/SystemUser';

const session = useSession();
const router = useRouter();

const user = computed<User | null>(() => session.authenticatedUser ?? null);

const moreInfo = ref(false);
const roles = [
    { value: 0, label: 'Admin', color: 'primary'},
    { value: 1, label: 'Port Authority Officer', color: 'success'},
    { value: 2, label: 'SAO Representative', color: 'warning'},
    { value: 3, label: 'Logistics operator', color: 'danger'}
]

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

const navigateToDataRights = () => {
    moreInfo.value = false;
    router.push({ name: 'My Data & Privacy' });
};

</script>

<template>
<div>
    <div @click="toggleMoreInfo" class="user-info">
        <sl-avatar 
            :image="user?.avatar"
            label="User avatar"
            loading="lazy"
        ></sl-avatar>
        <p>{{ user?.name || 'Guest' }}</p>
        <sl-icon class="icon" name="chevron-down"></sl-icon>
    </div>

    <div class="info-popup">
        <sl-popup placement="bottom-start" shift shift-padding="10" :active="moreInfo" >
            <span slot="anchor"></span>
            <div class="box">
                <!-- <p class="title"><sl-badge variant="primary" pill>Admin</sl-badge> {{user.name}} </p> -->
                <div class="opposed">
                    <p class="title">{{ user?.name || 'Guest' }}</p>
                    <sl-badge 
                        class="role"
                        :variant="roles.find(r => r.value === user?.role)?.color || 'default'" 
                        pill
                    >
                        {{ roles.find(r => r.value === user?.role)?.label || 'Unknown Role' }}
                    </sl-badge> 
                </div>

                <p class="subtitle">{{ user?.email || '' }}</p>
                <div class="profile-links">
                    <sl-button 
                        variant="text" 
                        size="small"
                        @click="navigateToDataRights"
                    >
                        <sl-icon slot="prefix" name="shield-lock"></sl-icon>
                        My Data & Privacy
                    </sl-button>
                </div>
                <div class="logout-box">
                    <Logout />
                </div>
            </div>
        </sl-popup>
    </div>
</div>
</template>
<style scoped>
.profile-links {
    margin: 10px 0;
    border-top: 1px solid var(--sl-color-neutral-200);
    padding-top: 10px;
}

.data-rights-link {
    display: block;
    text-decoration: none;
}

.data-rights-link sl-button {
    width: 100%;
}

.data-rights-link sl-button::part(base) {
    justify-content: flex-start;
}
</style>
