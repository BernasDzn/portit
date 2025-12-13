import { OperationPlanMetadataDto } from "../../dto/value/operationPlanMetadataDto";

export default class OperationPlanMetadata {
	createdBy: string;
	createdAt: Date;
	algorithmUsed: string;

	constructor(params: {
		createdBy: string;
		createdAt: Date;
		algorithmUsed: string;
	}) {
		this.createdBy = params.createdBy;
		this.createdAt = params.createdAt;
		this.algorithmUsed = params.algorithmUsed;
	}

	toDto(): OperationPlanMetadataDto {
		return {
			createdBy: this.createdBy,
			createdAt: this.createdAt.toISOString(),
			algorithmUsed: this.algorithmUsed
		};
	}
}