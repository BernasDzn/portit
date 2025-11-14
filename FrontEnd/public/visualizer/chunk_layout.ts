import * as THREE from "three";
import { loadModel, loadModelRaw } from "./helpers/model_helper.ts";
import { makeBillboard } from "./helpers/billboard_helper.ts";
import Vessel, { Crane, Seagull } from "./entities.ts";
import PickHelper from "./helpers/pick_helper.ts";
import { hideInfoText, setInfoText } from "./helpers/info_helper.ts";
import {TimedEvent} from "./time.ts";

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
    y: 20,
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

export function chunkIndexToPosition(x, y, centered = false) {
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

    constructor(x, y) {

        // Determine position based on chunk index
        if (validChunkPositions[x][y] === 0) {
            console.warn(`Invalid chunk position at (${x}, ${y})`);
            return;
        }

        this.position = chunkIndexToPosition(x, y);
        this.position.y += chunkSize.y / 2;
    }
}

class WarehouseChunk extends PortChunk {

    warehouseModel;
    warehouseLabel;
    pointLight;

    turnOffEvent;
    turnOnEvent;

    constructor(x, y) {
        super(x, y);
        this.base = null;
        this.warehouseLabel = makeBillboard("Warehouse", 32, 0xffffff);
    }

    turnOnLight() {
        console.log("Turning on warehouse light");
        if (this.pointLight) {
            this.pointLight.intensity = 2;
        }
    }

    turnOffLight() {
        console.log("Turning off warehouse light");
        if (this.pointLight) {
            this.pointLight.intensity = 0;
        }
    }

    async init(scene) {
        const model = new THREE.BoxGeometry(chunkSize.x, chunkSize.y, chunkSize.z);
        let baseMesh = new THREE.MeshStandardMaterial({ color: 0xaaaaaa });
        this.base = new THREE.Mesh(model, baseMesh);
        this.base.position.copy(this.position);
        this.base.castShadow = true;
        this.base.receiveShadow = true;

        this.base.meta = {
            title: "Warehouse Chunk",
            description: "This is a warehouse chunk.\n Located at (" + this.position.x.toFixed(2) + ", " + this.position.z.toFixed(2) + ").",
        };

        scene.add(this.base);

        this.warehouseModel = await loadModel("/visualizer/models/warehouse/warehouse.obj");
        this.warehouseModel.position.copy(this.position);
        this.warehouseModel.position.y += 21;
        this.warehouseModel.rotateY(Math.PI / 2);
        this.warehouseModel.scale.set(6, 6, 6);
        this.warehouseModel.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        scene.add(this.warehouseModel);

        this.warehouseLabel.position.set(
            this.position.x,
            this.position.y + 45,
            this.position.z - 20
        );

        scene.add(this.warehouseLabel);

        this.pointLight = new THREE.PointLight(0xfcfc95, 2, 100, 0);
        this.pointLight.position.set(this.position.x, this.position.y + 30, this.position.z);
        this.pointLight.castShadow = true;

        scene.add(this.pointLight);

        this.turnOffEvent = new TimedEvent(6, () => {
            this.turnOffLight();
        });

        this.turnOnEvent = new TimedEvent(20, () => {
            this.turnOnLight();
        });
    }
}

class LandChunk extends PortChunk {

    constructor(x, y) {
        super(x, y);
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

    pointLight;

    constructor(x, y) {
        super(x, y);
        this.base = null;
    }

    async init(scene) {
        const model = await loadModel("/visualizer/models/buoy.obj");

        // position and scale adjustments
        this.position.y -= 7;
        model.position.copy(this.position);
        model.scale.set(3, 3, 3);
        model.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        this.base = model;
        scene.add(this.base);

        this.pointLight = new THREE.PointLight(0xff0000, 10, 100, 0);
        this.pointLight.position.set(this.position.x + chunkSize.x / 2, this.position.y + 10, this.position.z - chunkSize.z / 2);

        scene.add(this.pointLight);
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

    constructor(x, y, label) {
        super(x, y);
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
    lighthousePointLight;

    showPaths = false;

    picker;

    vesselList = []; // The vessels in the port
    craneList = []; // The cranes in the port
    seagullList = []; // The seagulls in the port
    chunkData = []; // The chunks that make up the port layout

    constructor(scene, camera) {

        this.picker = new PickHelper();

        window.addEventListener('click', (event) => {
            this.pick(scene, camera);
        });

        let portChunk = new LandChunk(4, 4);
        let buoyChunk = new BuoyChunk(4, 3);
        let dockChunk = new DockChunk(4, 5);
        let warehouseChunk = new WarehouseChunk(3, 4);

        this.chunkData.push(warehouseChunk);
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

    togglePaths(visible) {
        this.showPath = visible;

        this.vesselList.forEach((vessel) => {
            vessel.setPathVisible(visible);
        });

        this.seagullList.forEach((seagull) => {
            seagull.setPathVisible(visible);
        });
    }

    turnOffEvent;
    turnOnEvent;

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
        this.lighthouse = await loadModel("/visualizer/models/lighthouse.obj");
        this.lighthouse.scale.set(0.4, 0.4, 0.4);
        this.lighthouse.position.set(-250, layoutY + 10, 250);

        this.lighthouse.traverse((child) => {
            if (child.isMesh) {
                console.log(child.material);
            }
        });

        scene.add(this.lighthouse);

        this.lighthouseLight = new THREE.SpotLight(0xffffaa, 75000, 0, Math.PI / 5, 0.2, 2);
        this.lighthouseLight.position.set(-249.2, layoutY + 130, 239.8);
        this.lighthouseLight.castShadow = true;
        
        this.lighthouseLight.shadow.mapSize.width = 2048;
        this.lighthouseLight.shadow.mapSize.height = 2048;
        this.lighthouseLight.shadow.camera.near = 10;
        this.lighthouseLight.shadow.camera.far = 1000;
        this.lighthouseLight.shadow.camera.fov = 60;
        this.lighthouseLight.shadow.bias = -0.0001;
        
        this.lighthouseLight.target.position.set(-249.2, layoutY + 130, 300);
        scene.add(this.lighthouseLight.target);

        scene.add(this.lighthouseLight);

        // Add point light to lighthouse structure
        this.lighthousePointLight = new THREE.PointLight(0xffffaa, 10, 50, 0);
        this.lighthousePointLight.position.set(-249.2, layoutY + 125, 239.8);
        scene.add(this.lighthousePointLight);

        this.turnOffEvent = new TimedEvent(6, () => {
            this.turnOffLighthouse();
        });

        this.turnOnEvent = new TimedEvent(20, () => {
            this.turnOnLighthouse();
        });
    }

    turnOnLighthouse() {
        if (this.lighthouseLight) {
            this.lighthouseLight.intensity = 75000;
            this.lighthousePointLight.intensity = 10;
        }
    }

    turnOffLighthouse() {
        if (this.lighthouseLight) {
            this.lighthouseLight.intensity = 0;
            this.lighthousePointLight.intensity = 0;
        }
    }

    async addSeagull(scene) {

        const model = await loadModel("/visualizer/models/seagull.obj");
        let seagull = new Seagull(model, new THREE.Vector3(0, 0, 0), this);
        seagull.init(scene);

        this.seagullList.push(seagull);
    }


    async addVessel(name, position, scene) {

        const model = await loadModel("/visualizer/models/vessel/12219_boat_v2_L2.obj");
        let vessel = new Vessel(name, model, position, this);
        vessel.init(scene);

        this.vesselList.push(vessel);
    }

    async addCrane(name, position, scene, rotation = 0) {

        const model = await loadModel("/visualizer/models/crane/scene.gltf");
        const rotationRadians = rotation * (Math.PI / 180);
        let crane = new Crane(name, model, position, rotationRadians);
        crane.init(scene);

        this.craneList.push(crane);
    }

    removeCrane(crane) {
        const index = this.craneList.indexOf(crane);
        if (index > -1) {
            this.craneList[index].kill();
            this.craneList.splice(index, 1);
        }
    }


    removeVessel(vessel) {
        const index = this.vesselList.indexOf(vessel);
        if (index > -1) {
            this.vesselList[index].kill();
            this.vesselList.splice(index, 1);
        }
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

        this.seagullList.forEach((seagull) => {
            seagull.update();
        });

        if (this.lighthouseLight && this.lighthouseLight.target) {
            const time = Date.now() * 0.0005;
            const lighthouseCenter = { x: -249.2, z: 239.8 };
            const lightRadius = 5;
            const targetRadius = 200;
            
            const lightX = lighthouseCenter.x + Math.cos(time) * lightRadius;
            const lightZ = lighthouseCenter.z + Math.sin(time) * lightRadius;
            this.lighthouseLight.position.set(lightX, layoutY + 125, lightZ);
            
            const targetX = lighthouseCenter.x + Math.cos(time) * targetRadius;
            const targetZ = lighthouseCenter.z + Math.sin(time) * targetRadius;
            this.lighthouseLight.target.position.set(targetX, layoutY + 125, targetZ);
            this.lighthouseLight.target.updateMatrixWorld();
        }
    }

    selectedObject;
    pick(scene, camera) {
        const normalizedPosition = {
            x: (event.clientX / window.innerWidth) * 2 - 1,
            y: -(event.clientY / window.innerHeight) * 2 + 1
        };

        const craneMeshes = [];
        this.craneList.forEach(crane => {
            crane.model.traverse((child) => {
                if (child.isMesh) {
                    craneMeshes.push(child);
                }
            });
        });

        const objectlist = [
            ...this.chunkData.map(chunk => chunk.base),
            ...this.vesselList.map(vessel => vessel.model.children[1]),
            ...craneMeshes
        ];

        const picked = this.picker.pickFromList(normalizedPosition, scene, camera, objectlist);
        let pickedObject = picked ? picked.object : null;

        const highlightMesh = (obj, color) => {
            if (obj && obj.material) {
                if (Array.isArray(obj.material)) {
                    obj.material.forEach((mat) => {
                        mat.emissive = new THREE.Color(color);
                    });
                } else {
                    obj.material.emissive = new THREE.Color(color);
                }
            }
        }

        let clickedCrane = null;
        if (pickedObject && pickedObject.userData && pickedObject.userData.craneId) {
            clickedCrane = this.craneList.find(crane => crane.name === pickedObject.userData.craneId);
        }

        objectlist.forEach((obj) => {
            highlightMesh(obj, 0x000000);
        });

        if (pickedObject) {
            this.selectedObject = pickedObject;
            console.log("Picked object:", this.selectedObject);

            try {
                const userData = this.selectedObject.meta;
                setInfoText(userData);
            } catch (error) {
                console.warn("No meta information available for selected object.");
            }

            if (clickedCrane) {
                clickedCrane.meshes.forEach((mesh) => {
                    highlightMesh(mesh, 0x444477);
                });
            } else {
                highlightMesh(this.selectedObject, 0x444477);
            }
        } else {
            this.selectedObject = null;
            hideInfoText();
        }
    }

}