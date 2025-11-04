<script setup lang="ts">
import LoginBox from "@/components/LoginBox.vue";
import PolkaGrid from '@/components/PolkaCanvas.vue'
import "./../assets/login.css";
import "./../assets/navigation.css";
import NotificationQueue from "@/components/NotificationQueue.vue";

</script>

<template>
    <div class="login-layout">
        <div class="login-left">
      <PolkaGrid class="polka-bg" />
      <img src="/PORTIT Dark Logo.svg" alt="Portit logo" class="logo" />
        </div>
        <div class="login-right">
            <LoginBox />
            <span class="login-footer">PorTiT© LAPR5 25/26 Group 3DJ-1</span>
        </div>
        <NotificationQueue />
    </div>
</template>


<style>
.login-layout {
  position: relative;
  min-height: 100vh;
  overflow: hidden;
}

.polka-bg {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
  pointer-events: none;
}

.login-left {
  position: relative;
  overflow: hidden;
}

.login-right {
  position: relative;
}

.login-left > *:not(.polka-bg) {
  position: relative;
  z-index: 1;
}

.logo {
  position: relative;
  z-index: 1;
}
</style>