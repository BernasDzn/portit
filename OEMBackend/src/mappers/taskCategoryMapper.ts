import { TaskCategory } from "../domain/taskCategory";

export class TaskCategoryMapper {

    static fromSchema(doc: any): TaskCategory {
        return new TaskCategory({
            id: doc._id ? doc._id.toString() : undefined,
            category: doc.category,
            description: doc.description,
        });
    }
}
