export interface TaskCategoryDto {
    id: string | undefined;
    name: string;
    category: string;
    description: string;
}

export interface CreateTaskCategoryDto {
    name: string;
    category: string;
    description: string;
}