import mongoose from "mongoose";

const TaskCategorySchema = new mongoose.Schema({
    category: String,
    description: String,
});

export const TaskCategoryModel = mongoose.model('TaskCategory', TaskCategorySchema);