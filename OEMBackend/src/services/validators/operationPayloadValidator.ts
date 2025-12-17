
export class PayloadValidator {

    payload: any;
    type: string;

    constructor(
        payload: Object,
        type: string
    ) {
        this.payload = payload;
        this.type = type;
    }

    // Validate of payload comes with the correct keys
    // We probably wont do this for any other operation type for now, since they will be
    // user defined, and thats not a user story :p
    validatePayloadForType() {

        let expectedKeys: string[] = [];
        switch (this.type) {
            case 'LOAD':
            case 'UNLOAD':
                expectedKeys = ['containerId', 'storageLocation'];
                break;
            case 'BERTH':
                expectedKeys = ['dock'];
                break;
            default:
                // Unrecognized operation type means empty payload is expected
                if (this.payload && Object.keys(this.payload).length > 0) {
                    throw new Error(`Payload is not expected for operation type: ${this.type}`);
                }
                break;
        }

        if (expectedKeys.length > 0) {
            for (const key of expectedKeys) {
                if (!(key in this.payload)) {
                    throw new Error(`Missing expected payload key '${key}' for operation type: ${this.type}`);
                }
            }
        }
    }
}