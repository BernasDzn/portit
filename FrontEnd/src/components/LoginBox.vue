<script setup lang="ts">
import { onMounted } from 'vue';

const handleCredentialResponse = async (response: any) => {
    const idToken = response.credential;

    let res = await fetch('https://localhost:5001/Login/google', {
        headers: {
            'Content-Type': 'application/json'
        },
        method: 'POST',
        body: JSON.stringify({ token: idToken })
    });

    if (res.ok) {
        let token = await res.text();
        token = JSON.parse(token).token;
        
        // NAO FAZER ISTO !!
        // ATENÇAO CODIGO MAL FEITO
        localStorage.setItem('authToken', token);
        window.location.href = '/';

    } else {
        console.error('uh oh', res.status)
    }
};

onMounted(() => {
    /* Load Google script dynamically if not already loaded */
    const existingScript = document.getElementById('google-client-script')
    if (!existingScript) {
        const script = document.createElement('script')
        script.src = 'https://accounts.google.com/gsi/client'
        script.async = true
        script.defer = true
        script.id = 'google-client-script'
        script.onload = () => initGoogleSignIn()
        document.head.appendChild(script)
    } else {
        initGoogleSignIn()
    }

    function initGoogleSignIn() {
        console.log('Initializing Google Sign-In');
        window.google.accounts.id.initialize({
            client_id: '28670621917-0p4e3s7it08to15g7c591b4vjtvo9eaq.apps.googleusercontent.com',
            callback: handleCredentialResponse
        })

        window.google.accounts.id.renderButton(
            document.getElementById('google-signin-btn'),
            { theme: 'outline', size: 'large' }
        )
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