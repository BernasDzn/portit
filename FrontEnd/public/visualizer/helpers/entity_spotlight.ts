import * as THREE from 'three';

export default class EntitySpotlight {

	spotlight;
	target;
	distanceFromEntity = 120;

	constructor(color = 0xffffaa, intensity = 40000, distance = 0, angle = Math.PI / 5, penumbra = 0.5, decay = 2) {
		this.spotlight = new THREE.SpotLight(color, intensity, distance, angle, penumbra, decay);
		// Defer enabling shadows to avoid an expensive synchronous shadow map build
		// on the first interaction which can freeze the UI for a second.
		this.spotlight.castShadow = false;
		this.spotlight.shadow.mapSize.width = 512; // smaller by default for lighter first load
		this.spotlight.shadow.mapSize.height = 512;
		this.spotlight.shadow.bias = -0.0001;
		this._shadowsEnabledOnce = false;
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
		// Turn on immediately but defer enabling expensive shadow rendering for
		// a short time so the first click doesn't cause a blocking shadow map build.
		this.turnOn();
		if (!this._shadowsEnabledOnce) {
			// Enable shadows after a short delay (two frames) to let the UI update
			setTimeout(() => {
				this.spotlight.castShadow = true;
				this._shadowsEnabledOnce = true;
			}, 100);
		}
	}

	turnOff() { this.spotlight.visible = false; }

	turnOn() { this.spotlight.visible = true; }

}