import { UserDto } from "../dto/userDto";

export enum UserRole {
	Administrator = "Administrator",
	PortAuthorityOfficer = "PortAuthorityOfficer",
	SAORepresentative = "SAORepresentative",
	LogisticsOperator = "LogisticsOperator"
}

export class User {
	id: string;
	emailAddress: string;
	name: string;
	userRole: UserRole;

	constructor(params: {
		id: string;
		emailAddress: string;
		name: string;
		userRole: UserRole;
	}) {
		this.id = params.id;
		this.emailAddress = params.emailAddress;
		this.name = params.name;
		this.userRole = params.userRole;
	}
	
	toDto(): UserDto {
		return {
			id: this.id,
			emailAddress: this.emailAddress,
			name: this.name,
			user_role: this.userRole,
		};
	}
}
