<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { locale, t } = useI18n()

const options = [
    { code: 'en', label: 'English' },
    { code: 'pt', label: 'Português' },
    { code: 'ca', label: 'Català' },
    { code: 'zh', label: '中文' },
    { code: 'bg', label: 'Български' },
    { code: 'cs', label: 'Čeština' },
    { code: 'da', label: 'Dansk' },
    { code: 'de', label: 'Deutsch' },
    { code: 'el', label: 'Ελληνικά' },
    { code: 'es', label: 'Español' },
    { code: 'et', label: 'Eesti' },
    { code: 'fi', label: 'Suomi' },
    { code: 'fr', label: 'Français' },
    { code: 'hr', label: 'Hrvatski' },
    { code: 'hu', label: 'Magyar' },
    { code: 'is', label: 'Íslenska' },
    { code: 'it', label: 'Italiano' },
    { code: 'lt', label: 'Lietuvių' },
    { code: 'lv', label: 'Latviešu' },
    { code: 'mt', label: 'Malti' },
    { code: 'nl', label: 'Nederlands' },
    { code: 'no', label: 'Norsk' },
    { code: 'pl', label: 'Polski' },
    { code: 'ro', label: 'Română' },
    { code: 'ru', label: 'Русский' },
    { code: 'sk', label: 'Slovenčina' },
    { code: 'sl', label: 'Slovenščina' },
    { code: 'sv', label: 'Svenska' },
    { code: 'uk', label: 'Українська' },
    { code: 'tr', label: 'Türkçe' },
    { code: 'ga', label: 'Gaeilge' },
    { code: 'sq', label: 'Shqip' },
    { code: 'mk', label: 'Македонски' },
    { code: 'sr', label: 'Српски' },
    { code: 'bs', label: 'Bosanski' },

]

const current = computed(() => options.find(o => o.code === locale.value) ?? options[0])

function setLocale(code: string) {
    locale.value = code
    try { localStorage.setItem('locale', code) } catch { }
}
</script>

<template>
    <div class="language-switcher">
        <sl-dropdown placement="bottom-end">
            <sl-button slot="trigger" size="small" style="color: white;" caret>
                <span class="label">{{ current?.label }}</span>
            </sl-button>

            <sl-menu>
                <sl-menu-item v-for="opt in options" :key="opt.code" @click="() => setLocale(opt.code)">
                    <span class="label opt">{{ opt.label }}</span>
                </sl-menu-item>
            </sl-menu>
        </sl-dropdown>
    </div>
</template>

<style scoped>
.language-switcher {
    display: inline-block;
    margin-right: 2rem;
}

.language-switcher .label {
    vertical-align: middle;
    color: white;
}

.language-switcher .label.opt {
    color: black;
}

sl-button::part(base) {
    background: transparent;
    color: inherit;
}
</style>
