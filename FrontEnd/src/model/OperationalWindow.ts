export interface OperationalWindow {
	shifts? : Shift[];
}

export interface Shift{
	day : number;
	startTime : string;
	endTime : string;
}

export function FullWeek() : OperationalWindow {
	const shifts : Shift[] = [];
	for (let day = 0; day < 7; day++) {
		shifts.push({
			day: day,
			startTime: '00:00',
			endTime: '23:59'
		});
	}
	return { shifts: shifts };
}