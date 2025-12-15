import { TaskCategory } from "../domain/taskCategory";

export class TaskCategoryMapper {
    static toSchema(taskCategory: TaskCategory) {
        return {
            name: taskCategory.name,
            category: taskCategory.category,
            description: taskCategory.description,
        };
    }

    static fromSchema(doc: any): TaskCategory | null {
        if (!doc) {
            return null;
        }
        return new TaskCategory({
            id: doc._id ? doc._id.toString() : undefined,
            name: doc.name,
            category: doc.category,
            description: doc.description,
        });
    }
}
