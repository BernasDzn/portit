<script setup lang="ts">
import { useAlerts } from '@/composables/alerts';
import { useSession } from '@/composables/session';
import type { User } from '@/model/User';
import { AuthService } from '@/service/AuthService';
import AxiosHttpService from '@/service/AxiosHttpService';
import type { AppJWTResponse } from '@/service/IService/IAuthService';
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';

const http = new AxiosHttpService();
const authService = new AuthService(http);

const session = useSession();
const router = useRouter();

const notifications = useAlerts();

const loginFinished = (res: AppJWTResponse) => {
    // Backend now returns an integer `role`. Parse it robustly and fall back to -1.
    let numericRole = -1;
    if (res && res.user) {
        if (typeof res.user.role === 'number') {
            numericRole = res.user.role;
        } else {
            const parsed = parseInt(String(res.user.role ?? ''), 10);
            if (!isNaN(parsed)) numericRole = parsed;
        }
    }

    const sessionUser: User = {
        id: res.user.id,
        name: res.user.name,
        email: res.user.email,
        avatar: res.user.picture,
        role: numericRole
    };

    session.setSession(sessionUser, res.token, res.expiresIn);

    // Goto dashbaotd
    router.push('/');
}

const errorCallback = (err: any) => {
    console.error('Google Sign-In error:', err);
    notifications.enqueueNotification('Error during Google Sign-In. Please try again.', notifications.notificationTypes.DANGER);
}

onMounted(() => {
    /* Load Google script dynamically if not already loaded */
    const existingScript = document.getElementById('google-client-script')
    if (!existingScript) {
        const script = document.createElement('script')
        script.src = 'https://accounts.google.com/gsi/client'
        script.async = true
        script.defer = true
        script.id = 'google-client-script'
        script.onload = () => authService.initGoogleSignIn(loginFinished, errorCallback)
        document.head.appendChild(script)
    } else {
        authService.initGoogleSignIn(loginFinished, errorCallback)
    }
});

const openHelp = () => {
    const dialog = document.querySelector('.dialog-overview') as any;
    dialog.show();
};

const closeHelp = () => {
    const dialog = document.querySelector('.dialog-overview') as any;
    dialog.hide();
};

</script>

<template>
    <div class="login-box">
        <p>Login</p>
        <div id="google-signin-btn"></div>
        <span class="help" @click="openHelp"> <u> Can't sign in? </u></span>

        <!-- Help box -->

        <sl-dialog label="Login help" class="dialog-overview">
            I dont car
            <sl-button @click="closeHelp" slot="footer" variant="primary">Ok</sl-button>
        </sl-dialog>      

    </div>
  </template>
  
<style scoped>

.help {
    font-size: 0.8em;
    color: #666;
    cursor: pointer;
    margin-top: 10px;
    display: inline-block;
}

</style>