import type TaskCategoryDto from "@/model/dto/TaskCategoryDto";

const categoryColorMap = {
    'LOAD': 'success',
    'UNLOAD': 'warning',
};

function colorMapCategory(cat: TaskCategoryDto) {
    return categoryColorMap[cat.category.value] || 'primary';
}

export function useTaskCategories() {
    return {
        colorMapCategory,
    };
}