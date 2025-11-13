import type { Qualification } from "./Qualifications";

export class Page<T> {

    constructor(params: { items: T[]; pageNumber: number; pageSize: number; pageCount: number; }) {
        this.items = params.items;
        this.pageNumber = params.pageNumber;
        this.pageSize = params.pageSize;
        this.pageCount = params.pageCount;
    }

    items: T[];
    pageNumber: number;
    pageSize: number;
    pageCount: number;
}

export interface Filter<T> {
    filter: Partial<T>;
    pageNumber?: number;
    pageSize?: number;
}