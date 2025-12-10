export class IdCode {
    private readonly value: string;

    constructor(value: string) {
        if (!this.isValid(value)) {
            throw new Error('Invalid IdCode format.');
        }
        this.value = value;
    }

    private isValid(value: string): boolean {
        // Only letters, undescopres and digits, length between 3 and 10
        const idCodeRegex = /^[A-Za-z0-9_]{3,10}$/;
        return idCodeRegex.test(value);
    }

    public getValue(): string {
        return this.value;
    }

    public toString(): string {
        return this.value;
    }
}