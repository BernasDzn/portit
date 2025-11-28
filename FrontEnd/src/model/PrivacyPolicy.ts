import type PrivacyPolicyDto from "./dto/PrivacyPolicyDto";

export class PrivacyPolicy {

    public content: string;
    public updatedOn: Date;
    public active: boolean;

    constructor(
        params: {
            content: string;
            updatedOn: Date;
            active: boolean;
        }
    ) {
        if (!params.content) throw new Error('Content cannot be null or empty.');

        this.content = params.content;
        this.updatedOn = params.updatedOn;
        this.active = params.active;
    }

    isActive(): boolean {
        return this.active;
    }

    updateContent(content: string): void {
        if (!content) throw new Error('Content cannot be null or empty.');
        this.content = content;
    }

    deactivate(): void {
        this.active = false;
    }

    toDto(): PrivacyPolicyDto {
        return {
            content: this.content,
        };
    }
}