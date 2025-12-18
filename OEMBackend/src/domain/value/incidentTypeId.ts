export class IncidentTypeID{
	value: string;
	
	constructor() {
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		this.value = "INC-" + timestamp + random;
	}
}