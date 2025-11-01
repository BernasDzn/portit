<script setup lang="ts">
import { ref } from 'vue';

const user = ref({
    name: 'Monokuma',
    email: 'monoemail@hopes.peak',
    role: 'Admin'
});
const moreInfo = ref(false);

const toggleMoreInfo = () => {
    moreInfo.value = !moreInfo.value;
};

</script>

<template>
<div>
    <div @click="toggleMoreInfo" class="user-info">
        <sl-avatar 
            
            image="monouser.png"
            label="User avatar"
            loading="lazy"
        ></sl-avatar>
        <p>{{user.name}}</p>
        <sl-icon class="icon" name="chevron-down"></sl-icon>
    </div>

    <div class="info-popup">
        <sl-popup placement="bottom-start" :active="moreInfo" >
            <span slot="anchor"></span>
            <div class="box">
                <sl-badge variant="primary" pill>{{user.role}}</sl-badge>
                <p>{{user.name}} <span>({{ user.email }})</span> </p>
            </div>
        </sl-popup>
    </div>
</div>
</template>

<style scoped>

.icon {
    margin: auto 0 auto 10px;
}

</style>