export interface StorageAreaDto {
    nameCode: string;
    location: string;
    type : number;
    capacity: number;
    currentOccupancy: number;
    dockServices: DockRelationDto[];
}

export interface DockRelationDto {
    dockCode: string;
    distance?: number;
    isServingDock: boolean;
}