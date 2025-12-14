<script setup lang="ts">
import MarkdownRenderer from '@/components/MarkdownRenderer.vue';
import { container } from '@/inversify.config';
import TYPES from '@/inversify/types';
import type { PrivacyPolicy } from '@/model/PrivacyPolicy';
import type { IPrivacyPolicyService } from '@/service/IService/IPrivacyPolicyService';
import { onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';

const privacyPolicyService = container.get<IPrivacyPolicyService>(TYPES.privacyPolicyService);
const privacyPolicy = ref<PrivacyPolicy>();

onMounted(async () => {
    try {
        const data = (await privacyPolicyService.getActivePrivacyPolicy());
        privacyPolicy.value = data;
    } catch (error) {
        privacyPolicy.value = null;
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

const openLicense = () => {
    openDialog('License');
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

const closeLicense = () => {
    closeDialog('License');
};

const downloadPrivacyPolicy = () => {
    window.open('/privacy-policy-print', '_blank');
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
                    This project is provided under an academic-use software license for educational purposes only.
                </p>
                <sl-button variant="primary" @click="openLicense">
                    Read license agreement
                </sl-button>
            </sl-card>
            <sl-card class="column">
                <h3>Privacy policy</h3>
                <p>
                    This privacy policy explains how personal data is handled in accordance with the GDPR.
                </p>
                <sl-button variant="primary" @click="openPrivacyPolicy">
                    Read privacy policy
                </sl-button>
            </sl-card>
        </div>

        <div class="columns">
            <sl-card class="column">
                <h3>Cookie policy</h3>
                <p>
                    We use cookies to remember users this improving your experience on our site.
                    By using our site, you agree to our use of cookies.
                </p>
            </sl-card>

        </div>

        

    <sl-dialog Label="Privacy Policy" class="dialog-overview" style="--width: 50vw;">
        <sl-icon-button class="new-window" slot="header-actions" name="download" @click="downloadPrivacyPolicy"></sl-icon-button>
        <p slot="label" style="display: flex; align-items: center; gap: 8px; margin: 0;">
            <b>Privacy Policy</b>
            <sl-badge variant="primary" pill>Last updated: {{ privacyPolicy?.updatedOn ? new Date(privacyPolicy.updatedOn).toLocaleDateString() : '' }}</sl-badge>
        </p>
        <MarkdownRenderer :markdown="privacyPolicy?.content || ''" style="height: 40vh;" />
        <sl-button @click="closePrivacyPolicy" slot="footer" variant="primary">Ok</sl-button>
    </sl-dialog>

    <sl-dialog label="License" class="dialog-overview" style="--width: 50vw;">
        <p>
            <p>
                This website and its source code were created as part of a university project related to port management
            </p>
            <p>
                The software is intended solely for educational and demonstration purposes.
                No warranty is provided, and the authors are not responsible for any misuse or incorrect interpretation
                of the information presented.
            </p>
            <p>
                This project uses open-source libraries and frameworks.
                Their respective licenses apply and are acknowledged.
            </p>
            <p>
                All rights are reserved unless otherwise stated.
            </p>                    
        </p>
        <sl-button @click="closeLicense" slot="footer" variant="primary">Ok</sl-button>
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
    box-shadow: 0 4px 14px rgba(0, 0, 0, 0.06);
}

sl-card h3 {
    margin-top: 0;
    margin-bottom: 0.5rem;
    font-size: 1.2rem;
    font-weight: 600;
}

sl-card p {
    margin: 0.5rem 0;
    line-height: 1.5;
}

sl-button {
    margin-top: 0.5rem;
    margin-bottom: 0;
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

sl-dialog::part(header) {
    padding-bottom: 0;
}

sl-dialog::part(body) {
    padding-top: 0;
}

</style>