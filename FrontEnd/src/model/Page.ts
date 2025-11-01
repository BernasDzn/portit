export interface Page<T> {
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