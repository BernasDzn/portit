// Administrator = 0,
// PortAuthorityOfficer = 1,
// SAORepresentative = 2,
// LogisticsOperator = 3

export enum UserRole {
    Administrator = "Administrator",
    PortAuthorityOfficer = "PortAuthorityOfficer",
    SAORepresentative = "SAORepresentative",
    LogisticsOperator = "LogisticsOperator"
}

export interface UserDto {
    id: string;
    emailAddress: string;
    name: string;
    user_role: UserRole;
}