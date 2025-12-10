export class IncidentType {
	id: string;
	name: string
	parent?: IncidentType | undefined;
	children?: IncidentType[] | undefined;

	generateID(){
		// generates a unique ID with epoch time and randomness. 
		// generating the same id twice is statistically improbable.
		// Example: INC-8063FC586
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		return "INC-" + timestamp + random;
	}

	addChild(child: IncidentType) {
		if (!this.children) {
			this.children = [];
		}
		this.children.push(child);
		child.parent = this;
	}

	constructor(params: {
		name: string;
		parent?: IncidentType | undefined;
		children?: IncidentType[] | undefined;
	}) {
		this.id = this.generateID();
		this.name = params.name;
		this.parent = params.parent;
		this.children = params.children;
	}

}