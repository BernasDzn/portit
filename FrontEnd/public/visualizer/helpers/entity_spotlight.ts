import * as THREE from 'three';

export default class EntitySpotlight {

	spotlight;
	target;
	distanceFromEntity = 120;

	_shadowsEnabledOnce = false;

	constructor(color = 0xffffaa, intensity = 40000, distance = 0, angle = Math.PI / 5, penumbra = 0.5, decay = 2) {
		this.spotlight = new THREE.SpotLight(color, intensity, distance, angle, penumbra, decay);

		this.spotlight.castShadow = true;
		this.spotlight.shadow.mapSize.width = 512;
		this.spotlight.shadow.mapSize.height = 512;
		this.spotlight.shadow.bias = -0.0001;
		this._shadowsEnabledOnce = true;
		this.target = this.spotlight.target;

		this.spotlight.visible = true;

		this.spotlight.position.set(0, this.distanceFromEntity + 400, 0);
		this.target.position.set(0, this.distanceFromEntity + 800, 0);
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
		if (!this._shadowsEnabledOnce) {
			setTimeout(() => {
				this.spotlight.castShadow = true;
				this._shadowsEnabledOnce = true;
			}, 100);
		}
	}

	turnOff() {
		this.spotlight.visible = true;
		this.spotlight.position.set(0, this.distanceFromEntity + 400, 0);
		this.target.position.set(0, this.distanceFromEntity + 800, 0);
	}

	turnOn() { this.spotlight.visible = true; }

}