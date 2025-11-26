import * as THREE from 'three';

export default class EntitySpotlight {

	spotlight;
	target;
	distanceFromEntity = 120;

	constructor(color = 0xffffaa, intensity = 40000, distance = 0, angle = Math.PI / 5, penumbra = 0.5, decay = 2) {
		this.spotlight = new THREE.SpotLight(color, intensity, distance, angle, penumbra, decay);
		this.spotlight.castShadow = true;
		this.target = this.spotlight.target;
		this.turnOff();
	}

	pointToEntity(entity) {
		if (!entity || !entity.position) {
			this.turnOff();
			return;
		}

		let realPosition = new THREE.Vector3();
		console.log("Parent Entity position:", entity.parent.position);
		if(entity.parent.position.x != 0){
			realPosition = entity.parent.position;
		}else{
			realPosition = entity.position;
		}

		console.log("Pointing spotlight to entity at ", realPosition);
		
		
		this.spotlight.position.set(realPosition.x, realPosition.y + this.distanceFromEntity, realPosition.z);
		this.target.position.set(realPosition.x, realPosition.y, realPosition.z);
		this.turnOn();
	}

	turnOff() { this.spotlight.visible = false; }

	turnOn() { this.spotlight.visible = true; }

}