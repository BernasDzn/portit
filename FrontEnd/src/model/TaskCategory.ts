import type TaskCategoryDto from "./dto/TaskCategoryDto";

export class TaskCategory {
    private _name: string;
    private _category: string;
    private _description: string;

    constructor(params: {
        name: string;
        category: string;
        description: string;
    }) {
        if (!params.name) throw new Error('Name cannot be null or empty.');
        if (!params.category) throw new Error('Category cannot be null or empty.');
        if (!params.description) throw new Error('Description cannot be null or empty.');

        this._name = params.name;
        this._category = params.category;
        this._description = params.description;
    }

    get name(): string { return this._name; }
    get category(): string { return this._category; }
    get description(): string { return this._description; }

    toDto(): TaskCategoryDto {
        return {
            name: this._name,
            category: this._category,
            description: this._description
        };
    }

    static fromDto(dto: TaskCategoryDto): TaskCategory {
        return new TaskCategory({
            name: dto.name,
            category: dto.category,
            description: dto.description
        });
    }
}
