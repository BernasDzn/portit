import type { StaffDto } from './dto/StaffDto';
import type { Qualification } from './Qualifications';
import type { OperationalWindow } from './values/OperationalWindow';

export class Staff {
    readonly mechanographicNumber: string;
	readonly name: string;
	readonly email: string;
	readonly phoneNumber: string;
	readonly status: number;
	readonly operationalWindow: OperationalWindow;
	readonly qualifications: Qualification[];

    constructor(params: {
        mechanographicNumber: string;
        name: string;
		email: string;
		phoneNumber: string;
		status: number;
		operationalWindow: OperationalWindow;
		qualifications: Qualification[];
    }) {
        this.mechanographicNumber = params.mechanographicNumber;
		this.name = params.name;
		this.email = params.email;
		this.phoneNumber = params.phoneNumber;
		this.status = params.status;
		this.operationalWindow = params.operationalWindow;
		this.qualifications = params.qualifications;
    }

    toDto(): StaffDto {
        return {
            mechanographicNumber: this.mechanographicNumber,
			name: this.name,
			email: this.email,
			phoneNumber: this.phoneNumber,
			status: this.status,
			operationalWindow: this.operationalWindow,
			qualificationsCodes: this.qualifications.map(q => q.idCode),
        };
    }
}