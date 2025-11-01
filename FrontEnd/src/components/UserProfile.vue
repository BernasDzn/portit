<script setup lang="ts">
import { onMounted, ref } from 'vue';

const user = ref({
    name: 'Monokuma',
    email: 'monoemail@hopes.peak',
    avatar: '/monouser.png',
    role: 'Admin'
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
    // fetch user info
    
    const token = localStorage.getItem('authToken');
    const res = await fetch('https://localhost:5001/Login/me', {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });

    if (res.ok) {
        const data = await res.json();
        user.value.name = data.name;
        user.value.email = data.email;
        user.value.avatar = data.picture;
        console.log(user.value.avatar);
    } else {
        console.error('Failed to fetch user info', res.status);
    }
});

</script>

<template>
<div>
    <div @click="toggleMoreInfo" class="user-info">
        <sl-avatar 
            shape="rounded" 
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
                <p class="title"><sl-badge variant="primary" pill>{{user.role}}</sl-badge> {{user.name}} </p>
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