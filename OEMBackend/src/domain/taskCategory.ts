import { TaskCategoryDto } from "../dto/taskCategoryDto";
import { IdCode } from "./value/idcode";

export class TaskCategory {
    public id: string | undefined;
    public category: IdCode;
    public description: string;

    constructor(params: { id: string | undefined, category: string; description: string }) {
        this.id = params.id ?? undefined;
        this.category = new IdCode(params.category);
        this.description = params.description;
    }

    toDto(): TaskCategoryDto {
        return {
            id: this.id,
            category: this.category.getValue(),
            description: this.description,
        };
    }
}