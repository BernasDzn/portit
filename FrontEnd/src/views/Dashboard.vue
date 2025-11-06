<script setup lang="ts">

import Loading from '@/components/Loading.vue';
import Port3DHero from '@/components/Port3DHero.vue';
import { useSession } from '@/composables/session';
import router from '@/router';
import { ref } from 'vue';
import { useI18n } from 'vue-i18n'
import AdminDashboard from './Admin/AdminDashboard.vue';
import SAORDashboard from './Dashboards/SAORDashboard.vue';
import MainDashboard from './Dashboards/MainDashboard.vue';

import "@/assets/dashboard.css"

const role = ref(useSession().authenticatedUser!.role);

</script>

<template>
<div>
    <MainDashboard v-if="role === 0"/>
    <SAORDashboard v-else-if="role === 2"/>
    <div v-else>
        <h1 class="title">Dashboard</h1>
        <p class="subtitle">Your role does not have a specific dashboard yet! Don't mind the blank space</p>
    </div>
</div>
</template>