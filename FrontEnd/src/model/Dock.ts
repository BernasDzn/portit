export interface Dock {
    code: string;
    name: string;
    location: string;
    physicalCharacteristics: {
        length: number;
        depth: number;
        draft: number;
    };
    supportedVesselTypes: {name: string;}[];
}

export interface DockFilter{
    name?: string;
    location?: string;
    vesselTypeName?: string;
}