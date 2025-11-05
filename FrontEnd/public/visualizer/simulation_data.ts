import * as THREE from "three";
import {loadModel} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";

const layoutY = -20;
const chunkSize = {
    x: 100,
    y: 40,
    z: 50
}

class PortChunk {

    base;

    constructor(position) {

        this.base = new THREE.BoxGeometry(chunkSize.x, chunkSize.y, chunkSize.z);
        let baseMesh = new THREE.MeshStandardMaterial({ color: 0x808080 });

        this.base = new THREE.Mesh(this.base, baseMesh);
        this.base.position.copy(position);

        this.base.castShadow = true;
        this.base.receiveShadow = true;
    }
}

export default class PortLayout {

    vesselList = []; // The vessels in the port
    chunkData = []; // The chunks that make up the port layout

    constructor(scene) {
        
        let portChunk = new PortChunk(new THREE.Vector3(0, layoutY, 0));
        this.chunkData.push(portChunk);

        scene.add(portChunk.base);
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
