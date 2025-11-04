import * as THREE from "three";
import {loadModel} from "./helpers/model_helper.ts";

const layoutY = -20;
const layoutSize = {
    x: 10,
    y: 80,
    z: 5
}

export default class PortLayout {

    base;
    vesselList = [];

    constructor(scene) {
        
        this.base = new THREE.BoxGeometry(10, 0.5, 10);
        let baseMesh = new THREE.MeshStandardMaterial({ color: 0x808080 });

        this.base = new THREE.Mesh(this.base, baseMesh);
        this.base.position.set(0, layoutY, 0);
        this.base.scale.set(layoutSize.x, layoutSize.y, layoutSize.z);

        this.base.castShadow = true;
        this.base.receiveShadow = true;

        scene.add(this.base);
    }

    async addVessel(name, position, scene) {

        const model = await loadModel("/visualizer/models/vessel/12219_boat_v2_L2.obj");
        let vessel = new Vessel(name, model, position);
        vessel.init(scene);

        this.vesselList.push(vessel);
    }

    update() {
        this.vesselList.forEach((vessel) => {
            vessel.update();
        });
    }
}

class Vessel {
    name;
    model;
    position;

    bouyanceAmplitude = 0.8;
    bouyanceSpeed = 0.002;

    constructor(name, model, position) {
        this.name = name;
        this.model = model;
        this.position = position;
    }

    init(scene) {
        this.model.position.copy(this.position);
        this.model.scale.set(0.05, 0.05, 0.05);
        this.model.rotation.y = Math.PI;
        this.model.rotation.x = Math.PI / 2;

        // Enable shadows for all child meshes
        this.model.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        this.model.receiveShadow = true;
        this.model.castShadow = true;

        scene.add(this.model);
    }

    update() {
        
        // Simple boyance effect
        this.model.position.y = this.position.y + Math.sin(Date.now() * this.bouyanceSpeed) * this.bouyanceAmplitude;
        this.model.rotation.y = Math.sin(Date.now() * this.bouyanceSpeed) * (this.bouyanceAmplitude / 50) + Math.PI;
    }
} 