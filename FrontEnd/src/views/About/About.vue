<script setup lang="ts">
import MarkdownRenderer from '@/components/MarkdownRenderer.vue';
import { useAlerts } from '@/composables/alerts';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { IPrivacyPolicyService } from '@/service/IService/IPrivacyPolicyService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const privacyPolicyService = container.get<IPrivacyPolicyService>(TYPES.privacyPolicyService);
const privacyPolicy = ref<string>('');

onMounted(async () => {
    try {
        const data = (await privacyPolicyService.getActivePrivacyPolicy());
        privacyPolicy.value = data.content;
    } catch (error) {
        privacyPolicy.value = '<p>Failed to load privacy policy.</p>';
    }
});

const { t } = useI18n();

const openDialog = (dialogLabel: string) => {
    const dialog = document.querySelector(`sl-dialog[label="${dialogLabel}"]`) as any;
    if (dialog) {
        dialog.show();
    }
};

const openPrivacyPolicy = () => {
    openDialog('Privacy Policy');
};

const openTermsOfService = () => {
    openDialog('TOS');
};

const closeDialog = (dialogLabel: string) => {
    const dialog = document.querySelector(`sl-dialog[label="${dialogLabel}"]`) as any;
    if (dialog) {
        dialog.hide();
    }
};

const closePrivacyPolicy = () => {
    closeDialog('Privacy Policy');
};

const closeTermsOfService = () => {
    closeDialog('TOS');
};

</script>

<template>
    <div>
        <h1 class="title">{{ t('about.title') }}</h1>
        <p class="subtitle"> v3.0.0 <br> portit.dev@gmail.com </p>
        
        <div class="columns">
            <sl-card class="column">
                <h3>Software license agreement</h3>
                <p>
                    Licensas para software educativo quais são???    
                </p>
            </sl-card>
            <sl-card class="column">
                <h3>Privacy policy</h3>
                <p>
                    Escrever RGPD emoji mão a escrever
        
                </p>
                <div style="display: flex; gap: 10px">
                    <sl-button variant="primary"
                        @click="openPrivacyPolicy"
                    >Read privacy policy</sl-button>
                    <sl-button>Download privacy policy</sl-button>
                </div>
            </sl-card>
        </div>

        <div class="columns">
    
            <sl-card class="column">
                <h3>Terms of service</h3>
                <p>
                    Termos de serviço que não vamos escrever isto é só um template
                </p>
                <sl-button
                    @click="openTermsOfService"
                >Read terms of service</sl-button>
            </sl-card>
            <sl-card class="column">
                <h3>Cookie policy</h3>
                <p>
                    We use cookies to remember users this improving your experience on our site.
                    By using our site, you agree to our use of cookies.
                </p>
            </sl-card>

        </div>

        

    <sl-dialog label="Privacy Policy" class="dialog-overview">
        <MarkdownRenderer :markdown="privacyPolicy" />
        <sl-button @click="closePrivacyPolicy" slot="footer" variant="primary">Ok</sl-button>
    </sl-dialog>

    <sl-dialog label="TOS" class="dialog-overview">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        <sl-button @click="closeTermsOfService" slot="footer" variant="primary">Ok</sl-button>
    </sl-dialog>

    </div>
</template>

<style scoped>

div {
    max-width: 900px;
    margin: 0 auto;
    padding: 2rem 1rem;
    font-family: Inter, system-ui, sans-serif;
}

.title {
    font-size: 2.2rem;
    font-weight: 700;
    margin-bottom: 0.25rem;
    text-align: center;
}

.subtitle {
    font-size: 1rem;
    color: #888;
    text-align: center;
    margin-bottom: 2rem;
}

.columns {
    display: flex;
    gap: 1.5rem;
    margin: 1.5rem 0;
}

.column {
    flex: 1;
}

sl-card::part(base) {
    border-radius: 12px;
    padding: 1.5rem;
    padding-bottom: 0;
    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.06);
}

sl-card h3 {
    margin-top: 0;
    font-size: 1.2rem;
    font-weight: 600;
}

sl-card p {
    margin: 0.5rem 0 1rem;
    line-height: 1.5;
}

sl-button {
    margin-top: 0.5rem;
}

p:last-of-type {
    margin-top: 0.5rem;
    font-weight: 500;
}

@media (max-width: 750px) {
    .columns {
        flex-direction: column;
    }
}


</style>