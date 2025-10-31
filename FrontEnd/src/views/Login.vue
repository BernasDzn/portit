<script setup lang="ts">
import LoginBox from "@/components/LoginBox.vue";
import "./../assets/login.css";

</script>

<template>
    <div class="login-layout">
        <div class="login-left">
            <img src="/PORTIT Dark Logo.svg" alt="Portit logo">
        </div>
        <div class="login-right">
            <LoginBox />
            <span class="login-footer">PorTiT© LAPR5 25/26 Group 3DJ-1</span>
        </div>
    </div>
</template>