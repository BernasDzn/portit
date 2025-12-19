export default interface TaskCategoryDto {
    name: string;
    category: string;
    description: string;
}

export interface TaskCategoryFilter {
    name?: string;    
}