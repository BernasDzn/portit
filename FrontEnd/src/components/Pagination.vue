<script setup lang="ts">
import { useI18n } from 'vue-i18n';


const {t} = useI18n();

const props = defineProps<{
    totalPages: number
    currentPage: number
}>();

const emits = defineEmits<{
    (e: 'page-changed', page: number): void
}>();

const setPage = (page: number) => {
    if (page >= 1 && page <= props.totalPages && page !== props.currentPage) {
        emits('page-changed', page);
    }
};

</script>

<template>
    <div class="pagination">
        <sl-button :disabled="props.currentPage==1" @click="() => {
            setPage(props.currentPage - 1);
        }">
            {{ t('buttons.pagination.previous') }}
        </sl-button>

        <template v-for="page in props.totalPages" :key="page">
            <sl-button
                :variant="page === props.currentPage ? 'primary' : 'default'"
                @click="() => {
                    setPage(page);
                }"
            >
                {{ page }}
            </sl-button>
        </template>

        <sl-button :disabled="props.currentPage==props.totalPages" @click="() => {
            setPage(props.currentPage + 1);
        }">
            {{ t('buttons.pagination.next') }}
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
