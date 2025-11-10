export interface Schedule {
    status: string;
    data: {
        loadingEnterTime: number;
        loadingExitTime: number;
        name: string;
    }[];
}