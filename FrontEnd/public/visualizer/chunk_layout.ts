import * as THREE from "three";
import {loadModel, loadModelRaw} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";
import Vessel from "./entities.ts";
import PickHelper from "./helpers/pick_helper.ts";
import { hideInfoText, setInfoText } from "./helpers/info_helper.ts";

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

export function chunkIndexToPosition(x, y, centered=false) {
    let position = new THREE.Vector3();
    position.x = chunkSize.x * x + worldOrigin.x;
    position.y = layoutY;
    position.z = -chunkSize.z * y + worldOrigin.z;

    if (centered) {
        position.x += chunkSize.x / 2;
        position.z -= chunkSize.z / 2;
    }

    return position;
}

class PortChunk {

    base;
    position;

    constructor(x,y) {

        // Determine position based on chunk index
        if (validChunkPositions[x][y] === 0) {
            console.warn(`Invalid chunk position at (${x}, ${y})`);
            return;
        }
        
        this.position = chunkIndexToPosition(x, y);
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

        this.base.meta = {
            title: "Land Chunk",
            description: "This is a land chunk.\n Located at (" + this.position.x.toFixed(2) + ", " + this.position.z.toFixed(2) + ").",
        };

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

class DockChunk extends PortChunk {
    
    dockLabel;

    constructor(x,y, label) {
        super(x,y);
        this.base = null;

        this.dockLabel = makeBillboard("Dock", 32, 0xffffff);
    }

    async init(scene) {
        this.base = new THREE.BoxGeometry(chunkSize.x / 2, chunkSize.y, chunkSize.z);
        const texture = new THREE.TextureLoader().load('/visualizer/textures/wood.png');
        texture.wrapS = THREE.RepeatWrapping;
        texture.wrapT = THREE.RepeatWrapping;

        let baseMesh = new THREE.MeshStandardMaterial({ map: texture });

        this.base = new THREE.Mesh(this.base, baseMesh);
        this.base.position.copy(this.position);

        this.base.meta = {
            title: "Dock Chunk",
            description: "This is a dock chunk where vessels can berth.\n Located at (" + this.position.x.toFixed(2) + ", " + this.position.z.toFixed(2) + ").",
        };

        this.base.castShadow = true;
        this.base.receiveShadow = true;
        scene.add(this.base);

        // Position dock label above the dock
        this.dockLabel.position.set(
            this.position.x,
            this.position.y + 30,
            this.position.z
        );

        scene.add(this.dockLabel);
    }
}

export default class PortLayout {

    terrain;
    lighthouse;
    lighthouseLight;

    picker;

    vesselList = []; // The vessels in the port
    chunkData = []; // The chunks that make up the port layout

    constructor(scene, camera) {
        
        this.picker = new PickHelper();

        window.addEventListener('click', (event) => {
            this.pick(scene, camera);
        });

        let portChunk = new LandChunk(4, 4);
        let buoyChunk = new BuoyChunk(4, 3);
        let dockChunk = new DockChunk(4, 5);

        this.chunkData.push(portChunk);
        this.chunkData.push(buoyChunk);
        this.chunkData.push(dockChunk);
     
        this.loadChunks(scene);
        this.loadTerrain(scene);
    }

    async loadChunks(scene) {
        for (let chunk of this.chunkData) {
            chunk.init(scene);
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

    selectedObject;
    pick(scene, camera) {
        const normalizedPosition = {
            x: (event.clientX / window.innerWidth) * 2 - 1,
            y: -(event.clientY / window.innerHeight) * 2 + 1
        };
    
        const objectlist = [
            ...this.chunkData.map(chunk => chunk.base),
            ...this.vesselList.map(vessel => vessel.model.children[1])
        ];

        console.log("Picking from", objectlist);
    
        const picked = this.picker.pickFromList(normalizedPosition, scene, camera, objectlist);
        let pickedObject = picked ? picked.object : null;
    
        const highlightMesh = (obj, color) => {
            if (obj && obj.material) {

                if (Array.isArray(obj.material)) {
                    obj.material.forEach((mat) => {
                        mat.emissive = new THREE.Color(color);
                    });
                } else
                    obj.material.emissive = new THREE.Color(color);
            }
        }

        // highlight picked
        if (pickedObject) {
            this.selectedObject = pickedObject;
            console.log("Picked object:", this.selectedObject);

            try {
                
                const userData = this.selectedObject.meta;
                setInfoText(userData.title || "Unknown Object", userData.description || "No description available.");
            } catch (error) {

                console.warn("No meta information available for selected object.");
            }
            highlightMesh(this.selectedObject, 0x444477);
        } else 
        {
            this.selectedObject = null;
            // Clear info text
            hideInfoText();
        }

        // unhighlight previous
        objectlist.forEach((obj) => {
            if (obj !== this.selectedObject) {
                highlightMesh(obj, 0x000000);
            }
        });
    }
    
}