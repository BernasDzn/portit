export interface Schedule {
    status: string;
    comment: string;
    data: {
        loading_enter_time: number;
        loading_exit_time: number;
        name: string;
    }[];
    metrics?: {
        algorithm: string;
        totalDelay: number;
        computationTime: number;
        vesselCount: number;
    };
}