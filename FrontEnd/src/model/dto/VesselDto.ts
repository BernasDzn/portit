export interface VesselDto {
    name: string;
    imoNumber: string;
    type: string;
    owner: string;
    physicalCharacteristics: {
        length: number;
        depth: number;
        draft: number;
    };
}

export interface VesselCreateDto {
    name: string;
    imoNumber: string;
    type: string;
    owner: string;
    length: number;
    depth: number;
    draft: number;
}

export interface VesselFilter {
    name?: string;
    imoNumber?: string;
    taxNumber?: string;
}