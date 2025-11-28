<script setup lang="ts">
import { useI18n } from 'vue-i18n';

const { t } = useI18n();

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
                <sl-button>Download privacy policy</sl-button>
            </sl-card>
        </div>

        <div class="columns">
    
            <sl-card class="column">
                <h3>Terms of service</h3>
                <p>
                    Termos de serviço que não vamos escrever isto é só um template
                </p>
                <sl-button>Read terms of service</sl-button>
            </sl-card>
            <sl-card class="column">
                <h3>Cookie policy</h3>
                <p>
                    We use cookies to remember users this improving your experience on our site.
                    By using our site, you agree to our use of cookies.
                </p>
            </sl-card>

        </div>

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