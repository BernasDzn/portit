<script setup lang="ts">

import { useSession } from '@/composables/session';
import { computed } from 'vue';
import SAORDashboard from './Dashboards/SAORDashboard.vue';
import MainDashboard from './Dashboards/MainDashboard.vue';

import "@/assets/dashboard.css"

const role = computed(() => useSession().authenticatedUser?.role ?? -1);

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