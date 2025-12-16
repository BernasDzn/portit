import { TaskCategoryDto } from "../dto/taskCategoryDto";
import { IdCode } from "./value/idcode";

export class TaskCategory {
    public id: string | undefined;
    public category: IdCode;
    public name: string;
    public description: string;

    constructor(params: { id: string | undefined, category: string; description: string; name: string }) {
        this.id = params.id ?? undefined;
        this.category = new IdCode(params.category);
        this.name = params.name;
        this.description = params.description;
    }

    toDto(): TaskCategoryDto {
        return {
            id: this.id,
            name: this.name,
            category: this.category.getValue(),
            description: this.description,
        };
    }
}