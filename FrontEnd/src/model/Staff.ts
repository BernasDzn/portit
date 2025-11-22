import type { StaffDto } from './dto/StaffDto';
import type { Qualification } from './Qualifications';
import type { OperationalWindow } from './values/OperationalWindow';

export enum StaffStatus {
	Available = 0,
	Unavailable = 1,
	TemporarilyReassigned = 2
}

export class Staff {
    private _mechanographicNumber: string;
	private _name: string;
	private _email: string;
	private _phoneNumber: string;
	private _status: number;
	private _operationalWindow: OperationalWindow;
	private _qualifications: Qualification[];
	private _isActive: boolean = true;

    constructor(params: {
        mechanographicNumber: string;
        name: string;
		email: string;
		phoneNumber: string;
		status: number;
		operationalWindow: OperationalWindow;
		qualifications: Qualification[];
    }) {
        this._mechanographicNumber = params.mechanographicNumber;
		this._name = params.name;
		this._email = params.email;
		this._phoneNumber = params.phoneNumber;
		this._status = params.status;
		this._operationalWindow = params.operationalWindow;
		this._qualifications = params.qualifications;
    }

	get mechanographicNumber(): string { return this._mechanographicNumber; }
	get name(): string { return this._name; }
	get email(): string { return this._email; }
	get phoneNumber(): string { return this._phoneNumber; }
	get status(): number { return this._status; }
	get operationalWindow(): OperationalWindow { return this._operationalWindow; }
	get qualifications(): Qualification[] { return this._qualifications; }
	get isActive(): boolean { return this._isActive; }

	update(
		newName: string,
		newEmail: string,
		newPhoneNumber: string,
		newStatus: number,
		newOperationalWindow: OperationalWindow,
		newQualifications: Qualification[]
	): void {
		this.updateName(newName);
		this.updateEmail(newEmail);
		this.updatePhoneNumber(newPhoneNumber);
		this.updateStatus(newStatus);
		this.updateOperationalWindow(newOperationalWindow);
		this.updateQualifications(newQualifications);
	}

	updateName(name: string): void {
		this._name = name;
	}

	updateEmail(email: string): void {
		this._email = email;
	}

	updatePhoneNumber(phoneNumber: string): void {
		this._phoneNumber = phoneNumber;
	}

	updateOperationalWindow(operationalWindow: OperationalWindow): void {
		this._operationalWindow = operationalWindow;
	}

	updateQualifications(newQualifications: Qualification[]): void {
		this._qualifications = newQualifications;
	}

	updateStatus(status: number): void {
		const validStatuses = [StaffStatus.Available, StaffStatus.Unavailable, StaffStatus.TemporarilyReassigned];
		if (!validStatuses.includes(status)) {
			throw new Error('Trying to update to invalid Status.');
		}
		this._status = status;
	}

	deactivate(): void {
		this._status = StaffStatus.Unavailable;
		this._isActive = false;
	}

    toDto(): StaffDto {
        return {
            mechanographicNumber: this._mechanographicNumber,
			name: this._name,
			email: this._email,
			phoneNumber: this._phoneNumber,
			status: this._status,
			operationalWindow: this._operationalWindow,
			qualificationsCodes: this._qualifications.map(q => q.idCode),
        };
    }
}