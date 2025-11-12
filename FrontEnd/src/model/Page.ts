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

    mapItems<U>(mapper: (item: T) => U): Page<U> {
        const mappedItems = this.items.map(mapper);
        return new Page<U> ({
            items: mappedItems,
            pageNumber: this.pageNumber,
            pageSize: this.pageSize,
            pageCount: this.pageCount,
        });
    }
}

export interface Filter<T> {
    filter: Partial<T>;
    pageNumber?: number;
    pageSize?: number;
}