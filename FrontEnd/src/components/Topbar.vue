<script setup lang="ts">
import { onMounted, onBeforeUnmount } from 'vue';
import Searchbar from './Searchbar.vue';
import UserProfile from './UserProfile.vue';
import Notifications from './Notifications.vue';
import LanguageSwitcher from './LanguageSwitcher.vue';
// @ts-ignore
import confetti from 'canvas-confetti';

import { useSession } from '@/composables/session';

const user = useSession().authenticatedUser;

function onLogoClick(_: MouseEvent) {
  try {
    const originX = 0.5;
    const originY = 0.5;

    confetti({
      particleCount: 60,
      spread: 70,
      origin: { x: originX, y: originY }
    });

    setTimeout(() => {
      confetti({
        particleCount: 30,
        spread: 120,
        origin: { x: originX, y: originY }
      });
    }, 150);
  } catch (err) {
  }
}

</script>

<template>
  <div class="topbar">

    <div class="topbar-left">

      <div class="topbar-logo">
        <RouterLink to="/"><img class="logo" src="/PORTIT Logo.svg" alt="logo" @click="onLogoClick"></RouterLink>
      </div>

      <!-- Global search bar -->
      <Searchbar />

    </div>

    <div class="topbar-right">
      <LanguageSwitcher />

      <!--Notifications-->
      <Notifications v-if="user?.role === 0"/>

      <!-- Profile -->
      <UserProfile />

    </div>

  </div>
</template>
