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