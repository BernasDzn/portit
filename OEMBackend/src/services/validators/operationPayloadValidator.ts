
export class PayloadValidator {

    payload: any;
    type: string;

    constructor(
        payload: any,
        type: string
    ) {
        this.payload = payload;
        this.type = type;
    }

    validatePayloadForType() {

        console.log(`Validating payload for operation type: ${this.type}`);

        switch (this.type) {
            case 'LOAD':
            case 'UNLOAD':
                break;
            case 'BERTH':
                break;
            default:
                // Unrecognized operation type means empty payload is expected
                if (this.payload && Object.keys(this.payload).length > 0) {
                    throw new Error(`Payload is not expected for operation type: ${this.type}`);
                }
                break;
        }
    }
}