<script setup lang="ts">
import Searchbar from './Searchbar.vue';
import UserProfile from './UserProfile.vue';

</script>

<template>
  <div class="topbar">

    <div class="topbar-left">
      
      <div class="topbar-logo"> 
        <RouterLink to="/"><img class="logo" src="/PORTIT Logo.svg" alt="logo"></RouterLink> 
      </div>
      
      <!-- Global search bar -->
      <Searchbar />

    </div>

    <div class="topbar-right">
    
      <!-- Profile -->
      <UserProfile />

    </div>

  </div>
</template>
