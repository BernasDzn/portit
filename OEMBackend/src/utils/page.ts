export interface Page<T> {
    pageNumber: number;
    pageSize: number;
    pageCount: number;
    items: T[];
}

export interface Pageable {
    pageNumber: number;
    pageSize: number;
}

export function mapPageItems<S, D>(sourcePage: Page<S>, mapFn: (source: S) => D): Page<D> {
    return {
        pageNumber: sourcePage.pageNumber,
        pageSize: sourcePage.pageSize,
        pageCount: sourcePage.pageCount,
        items: sourcePage.items.map(mapFn),
    };
}