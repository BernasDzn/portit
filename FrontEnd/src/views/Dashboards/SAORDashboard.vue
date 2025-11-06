<script setup lang="ts">

import Loading from '@/components/Loading.vue';
import Port3DHero from '@/components/Port3DHero.vue';
import { useI18n } from 'vue-i18n'
import { useSession } from '@/composables/session';
import { ref } from 'vue';
import RecentVVNS from '@/components/RecentVVNS.vue';

const name = ref(useSession().authenticatedUser!.name);
const { t } = useI18n();

</script>

<template>
<div>
    <h1 class="title"> {{t("dashboard.welcome").replace("[name]", name!)}}</h1>
    <p class="subtitle">{{ t('dashboard.SAOR.subtitle') }}</p>

    <RecentVVNS/>
</div>
</template>