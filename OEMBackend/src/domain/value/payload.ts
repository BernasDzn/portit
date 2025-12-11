import { PayloadDto } from "../../dto/value/payloadDto";

export class Payload {
    containerId: string | null;
    storageLocation: string | null;

    constructor(params: {
        containerId?: string;
        storageLocation?: string;
    }) {
        this.containerId = params.containerId ?? null;
        this.storageLocation = params.storageLocation ?? null;
    }

    toDto() : PayloadDto {
        return {
            containerId: this.containerId ?? "",
            storageLocation: this.storageLocation ?? ""
        };
    }

}