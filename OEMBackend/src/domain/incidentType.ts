import { Entity } from "../core/domain/entity";

export interface IncidentTypeProps {
	name: string;
	parent?: IncidentType | undefined;
	children?: IncidentType[] | undefined;
}

export class IncidentTypeID{
	value: string;
	
	constructor() {
		const timestamp = Date.now().toString(16).substring(4, 7).toUpperCase();
		const random = Math.random().toString(16).substring(2, 8).toUpperCase();
		this.value = "INC-" + timestamp + random;
	}
}

export default class IncidentType extends Entity<IncidentTypeProps> {
	get id(): string { return this._id; }
	get name(): string { return this.props.name; }
	get parent(): IncidentType | undefined { return this.props.parent; }
	get children(): IncidentType[] | undefined { return this.props.children; }

	constructor(props: IncidentTypeProps) {
		super(
			new IncidentTypeID().value, 
			props
		);
	}

	addChild(child: IncidentType) {
		if (!this.props.children) {
			this.props.children = [];
		}
		this.props.children.push(child);
		child.props.parent = this;
	}

}