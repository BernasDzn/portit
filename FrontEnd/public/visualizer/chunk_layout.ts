import * as THREE from "three";
import {loadModel, loadModelRaw} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";
import Vessel from "./entities.ts";

const worldBorder = 1000;

// 5x5 world chunks
const validChunkPositions = [
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
    [1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
];

const layoutY = -20;
const chunkSize = {
    x: worldBorder / validChunkPositions.length,
    y: 40,
    z: worldBorder / validChunkPositions[0].length
}

const worldOrigin = new THREE.Vector3(
    -worldBorder / 2,
    0,
    worldBorder / 2
);

const chunkTypes = Object.freeze({
    LAND: 0,
    DOCK: 1,
    BUOY: 2,
    WAREHOUSE: 3,
});

class PortChunk {

    base;
    position;

    constructor(x,y) {

        // Determine position based on chunk index
        if (validChunkPositions[x][y] === 0) {
            console.warn(`Invalid chunk position at (${x}, ${y})`);
            return;
        }
        
        this.position = new THREE.Vector3();
        this.position.x = chunkSize.x * x + worldOrigin.x;
        this.position.y = layoutY;
        this.position.z = -chunkSize.z * y + worldOrigin.z;
    }
}

class LandChunk extends PortChunk {

    constructor(x,y) {
        super(x,y);
        this.base = null;
    }

    async init(scene) {
        this.base = new THREE.BoxGeometry(chunkSize.x, chunkSize.y, chunkSize.z);
        let baseMesh = new THREE.MeshStandardMaterial({ color: 0x808080 });

        this.base = new THREE.Mesh(this.base, baseMesh);
        this.base.position.copy(this.position);

        this.base.castShadow = true;
        this.base.receiveShadow = true;
        scene.add(this.base);
    }
}

class BuoyChunk extends PortChunk {
    constructor(x, y) {
        super(x, y);
        this.base = null;
    }

    async init(scene) {
        const model = await loadModel("/visualizer/models/buoy.obj");

        // position and scale adjustments
        model.position.copy(this.position);
        model.position.y += 10;
        model.scale.set(3, 3, 3);
        model.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        this.base = model;
        scene.add(this.base);
    }

    update() {
        if (!this.base) return; // This crap is causing null errors

        // Buoy animation can be added here
        this.base.rotation.y += 0.001;
        this.base.position.y = this.position.y + 10 + Math.sin(Date.now() * 0.002) * 0.5;
    }
}

export default class PortLayout {

    terrain;
    lighthouse;
    lighthouseLight;

    vesselList = []; // The vessels in the port
    chunkData = []; // The chunks that make up the port layout

    constructor(scene) {
        
        let portChunk = new LandChunk(4, 4);
        let buoyChunk = new BuoyChunk(4, 3);

        this.chunkData.push(portChunk);
        this.chunkData.push(buoyChunk);
     
        this.loadChunks(scene);
        this.loadTerrain(scene);
    }

    async loadChunks(scene) {
        for (let chunk of this.chunkData) {
            await chunk.init(scene);
        }
    }

    // Load terrain
    async loadTerrain(scene) {
        this.terrain = await loadModelRaw("/visualizer/models/terrain.obj");

        this.terrain.scale.set(300, 300, 300);
        this.terrain.position.y = layoutY - 2;

        // move to the border
        this.terrain.position.x = -300;
        this.terrain.position.z = 300;

        this.terrain.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        scene.add(this.terrain);

        // Lighthouse
        this.lighthouse = await loadModelRaw("/visualizer/models/lighthouse.obj");
        this.lighthouse.scale.set(0.4, 0.4, 0.4);
        this.lighthouse.position.set(-250, layoutY + 10, 250);   

        scene.add(this.lighthouse);

        // Lighthouse light
        this.lighthouseLight = new THREE.PointLight(0xffffff, 2, 200); 
        this.lighthouseLight.position.set(-250, layoutY, 250);
        this.lighthouseLight.castShadow = true;

        scene.add(this.lighthouseLight);
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

        this.chunkData.forEach((chunk) => {
            if (chunk instanceof BuoyChunk) {
                chunk.update();
            }
        });
    }
}