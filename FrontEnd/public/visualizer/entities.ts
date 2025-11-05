import * as THREE from "three";
import {loadModel, loadModelRaw} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";

export default class Vessel {
    name;
    model;
    position;

    bouyanceAmplitude = 0.8;
    bouyanceSpeed = 0.002;

    label;

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

        // Make label billboard
        this.label = makeBillboard(this.name, 16, 0xffffff);

        const labelOffset = new THREE.Vector3(0, 10, 0);
        const vesselRoot = new THREE.Object3D();
        vesselRoot.position.copy(this.position).add(labelOffset);
        vesselRoot.add(this.label);

        scene.add(vesselRoot);
    }

    update() {
        
        // Simple boyance effect
        this.model.position.y = this.position.y + Math.sin(Date.now() * this.bouyanceSpeed) * this.bouyanceAmplitude;
        this.model.rotation.y = Math.sin(Date.now() * this.bouyanceSpeed) * (this.bouyanceAmplitude / 50) + Math.PI;
    }
} 
