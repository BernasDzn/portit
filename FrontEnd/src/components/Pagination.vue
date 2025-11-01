<script setup lang="ts">

defineProps<{
    totalPages: number
    currentPage: number
}>();

</script>

<template>
    <div class="pagination">
        <sl-button variant="default">
        Prev
        </sl-button>

        <template v-for="page in totalPages" :key="page">
            <sl-button>
                {{ page }}
            </sl-button>
        </template>

        <sl-button>
        Next
        </sl-button>
    </div>
</template>
  
<style scoped>
.pagination {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    justify-content: center;
    flex-wrap: wrap;
}
</style>
