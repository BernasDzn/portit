import './assets/main.css'
import './assets/navigation.css'

// Import shoelace
import '@shoelace-style/shoelace/dist/themes/light.css';
import { setBasePath } from '@shoelace-style/shoelace'

setBasePath('https://cdn.jsdelivr.net/npm/@shoelace-style/shoelace@2.20.1/cdn/');

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import i18n from './composables/i18n'
import AxiosHttpService from './service/AxiosHttpService';
import { AuthService } from './service/AuthService';
import { useSession } from './composables/session';

// Handle authentication on app load
const checkForAuthorization = async () => {

    // Allow tests to bypass authentication by setting VITE_TEST_BYPASS_AUTH=true
    // This seeds the in-memory session with a dummy user so router guards permit access.
    try {
        if ((import.meta.env as any).VITE_TEST_BYPASS_AUTH === 'true') {
            const session = useSession();
            // Seed a test user with Administrator role (0) so tests can access all routes.
            session.authenticatedUser = {
                id: 'test-user',
                email: 'test@example.com',
                name: 'Playwright Test',
                role: 0,
            };
            session.authToken = 'test-token';
            startApp();
            return;
        }

        const http = new AxiosHttpService();
        const response = await new AuthService(http).whoAmI();

        const session = useSession();
        session.authenticatedUser = response;

    } catch (error) {

        // we have to call this again grrr
        startApp();

        router.push('/unauthorized');
    } finally {
        startApp();
    }
    
};

const startApp = () => {
    
    const app = createApp(App);
    
    app.use(router)
    app.use(i18n)    
    app.mount('#app');
}

checkForAuthorization();