import mongoose from "mongoose";

const isEntity = (v: any): v is Entity<any> => {
	return v instanceof Entity;
};

export abstract class Entity<T> {
	public readonly _id: mongoose.Types.ObjectId;
	public readonly props: T;

	constructor (props: T, id?: mongoose.Types.ObjectId) {
		this._id = id ? id : new mongoose.Types.ObjectId();
		this.props = props;
	}

	public equals (object?: Entity<T>) : boolean {

		if (object == null || object == undefined) {
			return false;
		}

		if (this === object) {
			return true;
		}

		if (!isEntity(object)) {
			return false;
		}

		return this._id.equals(object._id);
	}
}