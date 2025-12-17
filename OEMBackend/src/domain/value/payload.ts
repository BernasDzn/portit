export class Payload {} // Payloads can be extended

export class LoadPayload extends Payload {
    containerId: string | null;
    storageLocation: string | null;

    constructor(params: {
        containerId?: string;
        storageLocation?: string;
    }) {
        super();
        this.containerId = params.containerId ?? null;
        this.storageLocation = params.storageLocation ?? null;
    }

    toDto() : any {
        return {
            containerId: this.containerId ?? "",
            storageLocation: this.storageLocation ?? ""
        };
    }

}

