// @ts-nocheck
import * as THREE from "three";
import { loadModel, loadModelRaw } from "./helpers/model_helper.ts";
import { makeBillboard } from "./helpers/billboard_helper.ts";
import Vessel, { Crane, Seagull, GantryCrane } from "./entities.ts";
import PickHelper from "./helpers/pick_helper.ts";
import { hideInfoText, setInfoText } from "./helpers/info_helper.ts";
import { TimedEvent } from "./time.ts";
import { fetchPortLayout, fetchVesselPositions, ChunkType } from "./chunk_service.ts";

const worldBorder = 1000;

// 5x5 world chunks
const validChunkPositions = [
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
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
        if (validChunkPositions[x][y] === 1) {
            console.warn(`Invalid chunk position at (${x}, ${y})`);
            return;
        }

        this.position = chunkIndexToPosition(x, y);
        this.position.y += chunkSize.y / 2;
    }
}

// Shared model cache for all chunks
const sharedModelCache = {};

// Helper function to center a model
function centerModel(model) {
    const box = new THREE.Box3().setFromObject(model);
    const center = box.getCenter(new THREE.Vector3());
    
    // Offset all children to center the model at origin
    model.traverse((child) => {
        if (child.isMesh) {
            child.geometry.translate(-center.x, -center.y, -center.z);
        }
    });

    // offset children so origin is at the bottom
    const boxAfter = new THREE.Box3().setFromObject(model);
    const min = boxAfter.min;
    model.traverse((child) => {
        if (child.isMesh) {
            child.geometry.translate(0, -min.y, 0);
        }
    });
    
    return model;
}

function makeAsphaltMaterial() {
    const loader = new THREE.TextureLoader();
    const asphalt = loader.load('/visualizer/textures/asphalt.jpg');
    const asphaltNormal = loader.load('/visualizer/textures/maps/asphalt_normal.png');
    const stone = loader.load('/visualizer/textures/stone.png');
    stone.wrapS = THREE.RepeatWrapping;
    stone.wrapT = THREE.RepeatWrapping;

    stone.repeat.set(1, 0.01);
    const materials = [
        new THREE.MeshStandardMaterial({ map: stone }),  // right
        new THREE.MeshStandardMaterial({ map: stone }),  // left
        new THREE.MeshStandardMaterial({                 // top (asphalt)
            color: new THREE.Color(0x666666),
            map: asphalt,
            normalMap: asphaltNormal
        }),
        new THREE.MeshStandardMaterial({ map: stone }),  // bottom
        new THREE.MeshStandardMaterial({ map: stone }),  // front
        new THREE.MeshStandardMaterial({ map: stone })   // back
    ];

    return materials;
}

class WarehouseChunk extends PortChunk {

    warehouseModel;
    warehouseLabel;
    pointLight;
    warehouseName;

    turnOffEvent;
    turnOnEvent;

    constructor(x, y, name = "Warehouse") {
        super(x, y);
        this.base = null;
        this.warehouseName = name;
        this.warehouseLabel = makeBillboard(name, 32, 0xffffff);
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
        let baseMesh = makeAsphaltMaterial();
        this.base = new THREE.Mesh(model, baseMesh);
        this.base.position.copy(this.position);
        this.base.castShadow = true;
        this.base.receiveShadow = true;

        this.base.meta = {
            title: this.warehouseName,
            description: `Warehouse: ${this.warehouseName}\nLocated at (${this.position.x.toFixed(2)}, ${this.position.z.toFixed(2)}).`,
        };

        scene.add(this.base);

        const modelPath = "/visualizer/models/lowpoly/building1.obj";
        
        // Load model once and cache it
        if (!sharedModelCache[modelPath]) {
            sharedModelCache[modelPath] = await loadModel(modelPath);
        }
        
        // Clone the cached model for this warehouse instance
        this.warehouseModel = sharedModelCache[modelPath].clone();
        this.warehouseModel.position.copy(this.position);
        this.warehouseModel.position.y -= 1;
        this.warehouseModel.rotateY(Math.PI / 2);
        this.warehouseModel.scale.set(2, 2, 2);
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
        this.pointLight.position.set(this.position.x, this.position.y+12, this.position.z - 8);
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

    x;
    y;

    constructor(x, y) {
        super(x, y);
        this.x = x;
        this.y = y;
        this.base = null;
    }

    async init(scene) {
        this.base = new THREE.BoxGeometry(chunkSize.x, chunkSize.y, chunkSize.z);
        let baseMesh = makeAsphaltMaterial();

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

const modelCache = {};

class BuoyChunk extends PortChunk {

    pointLight;

    constructor(x, y) {
        super(x, y);
        this.base = null;
    }

    async init(scene) {
        var modelPath = "/visualizer/models/buoy.obj";

        if (!modelCache[modelPath]) {
            modelCache[modelPath] = await loadModel(modelPath);
            centerModel(modelCache[modelPath]);
        }
        const model = modelCache[modelPath].clone();
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
    dockName;

    constructor(x, y, label = "Dock") {
        super(x, y);
        this.base = null;
        this.dockName = label;
        this.dockLabel = makeBillboard(label, 32, 0xffffff);
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
            title: this.dockName,
            description: `Dock: ${this.dockName}\nLocated at (${this.position.x.toFixed(2)}, ${this.position.z.toFixed(2)}).`,
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

    async addContainers(scene, modelCache, cranePositions = []) {
        const dockHalfWidth = chunkSize.x / 4;
        
        const craneBuffer = 20;
        
        const borderBuffer = 5;
        
        // invalid zone
        const minZ = this.position.z - chunkSize.z / 2 + borderBuffer;
        const maxZ = this.position.z + chunkSize.z / 2 - borderBuffer;
        
        const containerModels = [];
        for (let i = 1; i <= 4; i++) {
            const modelPath = `/visualizer/models/lowpoly/container${i}.obj`;
            if (!modelCache[modelPath]) {
                modelCache[modelPath] = await loadModel(modelPath);
                centerModel(modelCache[modelPath]);
            }
            containerModels.push(modelCache[modelPath]);
        }
        
        const craneExclusionBoxes = cranePositions.map(cranePos => {
            return new THREE.Box3(
                new THREE.Vector3(cranePos.x - craneBuffer, cranePos.y - 50, cranePos.z - craneBuffer),
                new THREE.Vector3(cranePos.x + craneBuffer, cranePos.y + 50, cranePos.z + craneBuffer)
            );
        });
        
        const containerSize = { x: 15, y: 8, z: 15 };
        
        const placedContainers = [];
        
        const targetContainers = Math.floor(Math.random() * 3) + 2;
        const maxAttempts = 30;
        
        for (let i = 0; i < targetContainers && placedContainers.length < targetContainers; i++) {
            let placed = false;
            let attempts = 0;
            
            while (!placed && attempts < maxAttempts) {
                attempts++;
                
                const xOffset = (Math.random() - 0.5) * dockHalfWidth;
                const zPos = minZ + Math.random() * (maxZ - minZ);
                const posX = this.position.x + xOffset;
                const posY = this.position.y + chunkSize.y / 2;
                
                const approxBox = new THREE.Box3(
                    new THREE.Vector3(posX - containerSize.x/2, posY - containerSize.y/2, zPos - containerSize.z/2),
                    new THREE.Vector3(posX + containerSize.x/2, posY + containerSize.y/2, zPos + containerSize.z/2)
                );
                
                let collidesWithCrane = false;
                for (const craneBox of craneExclusionBoxes) {
                    if (approxBox.intersectsBox(craneBox)) {
                        collidesWithCrane = true;
                        break;
                    }
                }
                
                if (collidesWithCrane) {
                    continue;
                }
                
                let hasCollision = false;
                for (const existingBox of placedContainers) {
                    if (approxBox.intersectsBox(existingBox)) {
                        hasCollision = true;
                        break;
                    }
                }
                
                if (!hasCollision) {
                    const containerModel = containerModels[Math.floor(Math.random() * 4)];
                    const container = containerModel.clone();
                    
                    container.position.set(posX, posY, zPos);
                    container.rotation.y = (Math.random() - 0.5) * 0.3;
                    container.scale.set(1.5, 1.5, 1.5);
                    
                    container.traverse((child) => {
                        if (child.isMesh) {
                            child.castShadow = true;
                            child.receiveShadow = true;
                        }
                    });
                    
                    scene.add(container);
                    placedContainers.push(approxBox);
                    placed = true;
                }
            }
        }
    }
}

class YardChunk extends PortChunk {

    yardName;
    yardLabel;
    fences = [];

    constructor(x, y, label = "Yard") {
        super(x, y);
        this.base = null;
        this.yardLabel = makeBillboard(label, 32, 0xffffff);
        this.yardName = label;
    }

    async init(scene) {
        this.base = new THREE.BoxGeometry(chunkSize.x, chunkSize.y, chunkSize.z);
        let baseMesh = makeAsphaltMaterial();
        this.base = new THREE.Mesh(this.base, baseMesh);
        this.base.position.copy(this.position);
        this.base.castShadow = true;
        this.base.receiveShadow = true;
        this.base.meta = {
            title: "Yard Chunk",
            description: "This is a yard chunk.\n Located at (" + this.position.x.toFixed(2) + ", " + this.position.z.toFixed(2) + ").",
        };
        scene.add(this.base);

        this.yardLabel.position.set(
            this.position.x,
            this.position.y + 30,
            this.position.z
        );

        scene.add(this.yardLabel);
    }

    async addFences(scene) {
        const fenceModelPath = "/visualizer/models/lowpoly/fence.obj";
        if (!sharedModelCache[fenceModelPath]) {
            const model = await loadModel(fenceModelPath);
            centerModel(model);
            sharedModelCache[fenceModelPath] = model;
        }
        const fenceModel = sharedModelCache[fenceModelPath];
        const fenceScale = 2.0;
        const buffer = 1;
        const tempFence = fenceModel.clone();
        tempFence.scale.set(fenceScale, fenceScale, fenceScale);
        tempFence.updateMatrixWorld(true);
        const fenceBox = new THREE.Box3().setFromObject(tempFence);
        const fenceLength = fenceBox.max.z - fenceBox.min.z;
        const perimeterX = chunkSize.x - (buffer * 2);
        const perimeterZ = chunkSize.z - (buffer * 2);
        const numFencesX = Math.max(1, Math.round(perimeterX / fenceLength));
        const numFencesZ = Math.max(1, Math.round(perimeterZ / fenceLength));
        const halfChunkX = chunkSize.x / 2;
        const halfChunkZ = chunkSize.z / 2;
        const fenceY = this.position.y + chunkSize.y / 2;

        // Determine chunk grid position
        const chunkX = Math.round((this.position.x - worldOrigin.x) / chunkSize.x);
        const chunkY = Math.round(-(this.position.z - worldOrigin.z) / chunkSize.z);

        // Hif adjacent chunk is water or out of bounds
        function isWaterOrOutOfBounds(x, y) {
            if (x < 0 || x >= validChunkPositions.length || y < 0 || y >= validChunkPositions[0].length) {
                return true;
            }
            return validChunkPositions[x][y] === 0;
        }

        function isGap(i, numFences) {
            const mid = Math.floor(numFences / 2);
            if (numFences % 2 === 0) {
                return i === mid || i === mid - 1;
            } else {
                return i === mid;
            }
        }

        // NORTH side (-Z)
        for (let i = 0; i < numFencesX; i++) {
            const northInBounds = (chunkY - 1 >= 0);
            const northIsNotWater = northInBounds && validChunkPositions[chunkX][chunkY - 1] !== 0;
            if (northIsNotWater && isGap(i, numFencesX)) continue;
            const fence = fenceModel.clone();
            const xPos = this.position.x - halfChunkX + buffer + i * fenceLength + fenceLength / 2;
            fence.position.set(xPos, fenceY, this.position.z + halfChunkZ - buffer);
            fence.rotation.y = Math.PI / 2;
            fence.scale.set(fenceScale, fenceScale, fenceScale);
            fence.traverse((child) => {
                if (child.isMesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            scene.add(fence);
            this.fences.push(fence);
        }

        // SOUTH side (+Z)
        for (let i = 0; i < numFencesX; i++) {
            const southInBounds = (chunkY + 1 < validChunkPositions[0].length);
            const southIsNotWater = southInBounds && validChunkPositions[chunkX][chunkY + 1] !== 0;
            if (southIsNotWater && isGap(i, numFencesX)) continue;
            const fence = fenceModel.clone();
            const xPos = this.position.x - halfChunkX + buffer + i * fenceLength + fenceLength / 2;
            fence.position.set(xPos, fenceY, this.position.z - halfChunkZ + buffer);
            fence.rotation.y = -Math.PI / 2;
            fence.scale.set(fenceScale, fenceScale, fenceScale);
            fence.traverse((child) => {
                if (child.isMesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            scene.add(fence);
            this.fences.push(fence);
        }

        // West side 
        for (let i = 0; i < numFencesZ; i++) {
            const adjacentInBounds = (chunkX - 1 >= 0);
            if (adjacentInBounds && validChunkPositions[chunkX - 1][chunkY] !== 0 && isGap(i, numFencesZ)) continue;
            const fence = fenceModel.clone();
            const zPos = this.position.z - halfChunkZ + buffer + i * fenceLength + fenceLength / 2;
            fence.position.set(this.position.x - halfChunkX + buffer, fenceY, zPos);
            fence.rotation.y = Math.PI;
            fence.scale.set(fenceScale, fenceScale, fenceScale);
            fence.traverse((child) => {
                if (child.isMesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            scene.add(fence);
            this.fences.push(fence);
        }

        // East side 
        for (let i = 0; i < numFencesZ; i++) {
            const adjacentInBounds = (chunkX + 1 < validChunkPositions.length);
            if (adjacentInBounds && validChunkPositions[chunkX + 1][chunkY] !== 0 && isGap(i, numFencesZ)) continue;
            const fence = fenceModel.clone();
            const zPos = this.position.z - halfChunkZ + buffer + i * fenceLength + fenceLength / 2;
            fence.position.set(this.position.x + halfChunkX - buffer, fenceY, zPos);
            fence.rotation.y = 0;
            fence.scale.set(fenceScale, fenceScale, fenceScale);
            fence.traverse((child) => {
                if (child.isMesh) {
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            scene.add(fence);
            this.fences.push(fence);
        }
    }

    async addContainers(scene, modelCache, hasYardCrane = false) {
        const stackHeight = Math.floor(Math.random() * 3) + 2;
        const piles = hasYardCrane ? 1 : Math.round(Math.random()*3 + 1);
        const pilesPerRow = Math.ceil(Math.sqrt(piles));
        
        const containerWidth = 10;
        const containerHeight = 5;
        
        const containerModels = [];
        for (let i = 1; i <= 4; i++) {
            const modelPath = `/visualizer/models/lowpoly/container${i}.obj`;
            if (!modelCache[modelPath]) {
                modelCache[modelPath] = await loadModel(modelPath);
                centerModel(modelCache[modelPath]);
            }
            containerModels.push(modelCache[modelPath]);
        }
        
        const maxPileWidth = containerWidth * 4;
        const spacingBetweenPiles = 15;
        
        const totalWidth = pilesPerRow * maxPileWidth + (pilesPerRow - 1) * spacingBetweenPiles;
        const startX = this.position.x - totalWidth / 2;
        const startZ = this.position.z - totalWidth / 2;
        
        let pileIndex = 0;
        for (let pileRow = 0; pileRow < pilesPerRow && pileIndex < piles; pileRow++) {
            for (let pileCol = 0; pileCol < pilesPerRow && pileIndex < piles; pileCol++) {
                const pileX = startX + pileCol * (maxPileWidth + spacingBetweenPiles) + maxPileWidth / 2;
                const pileZ = startZ + pileRow * (maxPileWidth + spacingBetweenPiles) + maxPileWidth / 2;
                
                const gridWidth = Math.floor(Math.random() * 4) + 1;
                const gridDepth = Math.floor(Math.random() * 4) + 1;
                
                const grid = [];
                for (let h = 0; h < stackHeight; h++) {
                    grid[h] = [];
                    for (let d = 0; d < gridDepth; d++) {
                        grid[h][d] = [];
                        for (let c = 0; c < gridWidth; c++) {
                            grid[h][d][c] = false;
                        }
                    }
                }
                
                // bottom to top
                for (let height = 0; height < stackHeight; height++) {
                    // first layer
                    if (height === 0) {
                        const positions = [];
                        for (let d = 0; d < gridDepth; d++) {
                            for (let c = 0; c < gridWidth; c++) {
                                positions.push({ depth: d, col: c });
                            }
                        }
                        for (let i = positions.length - 1; i > 0; i--) {
                            const j = Math.floor(Math.random() * (i + 1));
                            [positions[i], positions[j]] = [positions[j], positions[i]];
                        }
                        // 75 to 100% of positions
                        const numToPlace = Math.floor(positions.length * (0.75 + Math.random() * 0.25));
                        for (let i = 0; i < numToPlace; i++) {
                            const pos = positions[i];
                            grid[height][pos.depth][pos.col] = true;
                        }
                    } else {
                        // only place if container below
                        // layer 1=70%, layer 2=50%, layer 3=30%, layer 4=15%
                        const stackProbability = Math.max(0.005, 0.9 - (height * 0.2));
                        for (let depth = 0; depth < gridDepth; depth++) {
                            for (let col = 0; col < gridWidth; col++) {
                                // if container below
                                if (grid[height - 1][depth][col]) {
                                    if (Math.random() < stackProbability) {
                                        grid[height][depth][col] = true;
                                    }
                                }
                            }
                        }
                    }
                    
                    // place containers
                    for (let depth = 0; depth < gridDepth; depth++) {
                        for (let col = 0; col < gridWidth; col++) {
                            if (grid[height][depth][col]) {
                                const containerModel = containerModels[Math.floor(Math.random() * 4)];
                                const container = containerModel.clone();
                                
                                container.position.set(
                                    pileX - (gridWidth - 1) * (containerHeight-1) / 2 + col * (containerHeight-1),
                                    this.position.y + chunkSize.y / 2 + height * containerHeight,
                                    pileZ - (gridDepth - 1) * containerWidth / 2 + depth * containerWidth
                                );
                                
                                container.rotation.y = (Math.random() - 0.5) * 0.1;
                                container.scale.set(1.5, 1.5, 1.5);
                                
                                container.traverse((child) => {
                                    if (child.isMesh) {
                                        child.castShadow = true;
                                        child.receiveShadow = true;
                                    }
                                });
                                
                                scene.add(container);
                            }
                        }
                    }
                }
                
                pileIndex++;
            }
        }
    }
}

/**
 * Generates chunk instances from API data
 * Creates appropriate chunk objects based on ChunkType
 */
function generateChunkLayoutFromAPI(portChunks) {
    const chunks = [];
    const containerCranePositions = []; // Track STS crane positions
    const yardCranePositions = []; // Track yard gantry crane positions

    for (const portChunk of portChunks) {
        let chunk = null;

        switch (portChunk.type) {

            case ChunkType.Land:
                chunk = new LandChunk(portChunk.x, portChunk.y);
                // Set as occupied
                validChunkPositions[portChunk.x][portChunk.y] = 1;
                chunks.push(chunk);
                break;

            case ChunkType.Warehouse:
                chunk = new WarehouseChunk(portChunk.x, portChunk.y, portChunk.name);
                // Set as occupied
                validChunkPositions[portChunk.x][portChunk.y] = 1;
                chunks.push(chunk);
                break;

            case ChunkType.Yard:
                chunk = new YardChunk(portChunk.x, portChunk.y, portChunk.name);
                // Set as occupied
                validChunkPositions[portChunk.x][portChunk.y] = 1;
                chunks.push(chunk);
                break;

            case ChunkType.Dock:
                chunk = new DockChunk(portChunk.x, portChunk.y, portChunk.name);
                // Set as occupied
                validChunkPositions[portChunk.x][portChunk.y] = 1;
                chunks.push(chunk);
                break;

            case ChunkType.STSCrane:
                containerCranePositions.push({
                    name: portChunk.name,
                    x: portChunk.x,
                    y: portChunk.y,
                    type: portChunk.type
                });
                break;
            case ChunkType.YardCrane:
                yardCranePositions.push({
                    name: portChunk.name,
                    x: portChunk.x,
                    y: portChunk.y,
                    type: portChunk.type
                });
                break;
        }
    }

    // Add some funny buoys randomly for fun
    for (let i = 0; i < 5; i++) {

        let validPosition = false;
        let buoyX = 0;
        let buoyY = 0;

        while (!validPosition) {
            buoyX = Math.floor(Math.random() * validChunkPositions.length);
            buoyY = Math.floor(Math.random() * validChunkPositions[0].length);

            // Check if position is water (0)
            if (validChunkPositions[buoyX][buoyY] === 0) {
                validPosition = true;
            }
            
        }

        const buoyChunk = new BuoyChunk(buoyX, buoyY);
        chunks.push(buoyChunk);
    }

    return { chunks, containerCranePositions, yardCranePositions };
}

export default class PortLayout {

    terrain;
    lighthouse;
    lighthouseLight;
    lighthousePointLight;

    showPaths = false;

    picker;

    /** @type {any[]} */
    vesselList = []; // The vessels in the port

    /** @type {Array<Crane|GantryCrane>} */
    craneList = []; // The cranes in the port (both STS and gantry cranes)

    /** @type {any[]} */
    seagullList = []; // The seagulls in the port

    /** @type {any[]} */
    chunkData = []; // The chunks that make up the port layout

    /** @type {Record<string, any>} */
    // Model cache to avoid reloading the same models
    modelCache = {};

    /**
     * Lightweight schedule of vessel events fetched at startup.
     * @type {Array<{vesselId:string,dockId:string,arrival:Date,departure:Date}>}
     */
    vesselSchedule = [];


    constructor(scene, camera, controls) {

        this.picker = new PickHelper(controls.controls);
        // Use right-click (context menu) for picking; prevent default browser menu
        window.addEventListener('contextmenu', (event) => {
            event.preventDefault();
            this.pick(scene, camera, event);
        });

        // Load chunks dynamically from API
        this.loadChunksFromAPI(scene);
        this.loadTerrain(scene);
        console.log('PortLayout initialized');
    }

    async loadChunksFromAPI(scene) {
        try {
            const portChunks = await fetchPortLayout();
            console.log('Loaded port layout data:', portChunks);

            const { chunks, containerCranePositions, yardCranePositions } = generateChunkLayoutFromAPI(portChunks);
            this.chunkData = chunks;

            for (let chunk of this.chunkData) {
                await chunk.init(scene);
            }

            // add fences to all yard chunks
            for (let chunk of this.chunkData) {
                if (chunk instanceof YardChunk) {
                    await chunk.addFences(scene);
                }
            }
            
            // Group cranes by chunk position
            const containerCranesByChunk = this.groupCranesByChunk(containerCranePositions);
            const yardCranesByChunk = this.groupCranesByChunk(yardCranePositions);
            
            // Track actual crane positions in world space
            const craneWorldPositions = {};
            
            // Add container cranes with spacing
            for (const [chunkKey, cranes] of Object.entries(containerCranesByChunk)) {
                const positions = await this.addCranesAtChunk(cranes, scene, 'container', 90);
                craneWorldPositions[chunkKey] = positions;
            }

            // Add yard cranes with spacing
            for (const [chunkKey, cranes] of Object.entries(yardCranesByChunk)) {
                await this.addCranesAtChunk(cranes, scene, 'yard', 0);
            }
            
            // Add containers to dock chunks, avoiding crane positions
            await this.addDockContainers(scene, craneWorldPositions);
            
            // Add containers to yard chunks after cranes are placed
            await this.addYardContainers(scene, yardCranePositions);

            // Fetch vessel positions once and record a lightweight schedule
            // Do NOT instantiate heavy vessel models here to avoid startup lag.
            try {
                const vesselPositions = await fetchVesselPositions();

                // vesselSchedule stores entries for when vessels should appear/disappear.
                // Format: { vesselId: string, dockId: string, arrival: Date, departure: Date }
                this.vesselSchedule = [];

                for (const vp of vesselPositions || []) {
                    const arrivalRaw = vp.ArrivalTime ?? vp.arrivalTime;
                    const departureRaw = vp.DepartureTime ?? vp.departureTime;
                    const dockId = vp.DockId ?? vp.dockId ?? vp.dock;
                    const vesselId = vp.VesselId ?? vp.vesselId ?? vp.vessel;

                    if (!arrivalRaw || !departureRaw || !dockId || !vesselId) continue;

                    const arrival = new Date(arrivalRaw);
                    const departure = new Date(departureRaw);

                    this.vesselSchedule.push({ vesselId: String(vesselId), dockId: String(dockId), arrival, departure });
                }

                // Log the parsed vessel schedule for debugging
                console.log('Vessel schedule:', this.vesselSchedule);
            } catch (err) {
                console.error('Failed to fetch vessel positions for scheduling:', err);
            }

            // Note: vessel positions are fetched once at startup and used thereafter.
        } catch (error) {
            console.error('Failed to load port layout from API:', error);
        }
    }

    /**
     * Group cranes by their chunk coordinate key.
     * @param {Array<any>} cranes
     */
    groupCranesByChunk(cranes) {
        const grouped = {};
        for (const crane of cranes) {
            const key = `${crane.x}_${crane.y}`;
            if (!grouped[key]) {
                grouped[key] = [];
            }
            grouped[key].push(crane);
        }
        return grouped;
    }

    /**
     * @param {Array<any>} cranes
     * @param {any} scene
     * @param {string} type
     * @param {number} baseRotation
     */
    async addCranesAtChunk(cranes, scene, type, baseRotation) {
        const numCranes = cranes.length;
        if (numCranes === 0) return [];

        const firstCrane = cranes[0];
        const basePosition = chunkIndexToPosition(firstCrane.x, firstCrane.y, false);
        basePosition.y += chunkSize.y;

        // Calculate scale based on number of cranes (scale down if more than 1)
        let scaleMultiplier = 1.0;
        if (numCranes > 1) {
            scaleMultiplier = Math.max(0.6, 1.0 / numCranes); // Min scale 0.6
        }

        // Calculate spacing along X axis within the chunk
        const spacing = chunkSize.x / (numCranes + 1);
        
        const positions = [];

        for (let i = 0; i < numCranes; i++) {
            const crane = cranes[i];
            const position = basePosition.clone();
            
            // Offset along y axis to spread cranes across the chunk
            position.z += spacing * (i + 1) - chunkSize.z / 2;

            // For STS cranes, each crane calculates its own rotation based on closest water
            //let rotation = baseRotation;
            //if (type === 'container') {
            //    rotation = this.calculateWaterFacingRotationFromPosition(position);
            //}

            if (type === 'container') {
                position.y -= 1; // Slightly lower container cranes
                await this.addContainerCrane(crane.name, position, scene, 180, scaleMultiplier);
            } else if (type === 'yard') {
                await this.addYardGantryCrane(crane.name, position, scene, 0, scaleMultiplier);
            }
            
            positions.push({ x: position.x, y: position.y, z: position.z });
        }
        
        return positions;
    }

    /**
     * @param {{x:number,y:number,z:number}} position
     * @returns {number}
     */
    calculateWaterFacingRotationFromPosition(position) {
        // Find which chunk grid cell this position is closest to
        const centerX = position.x - worldOrigin.x;
        const centerZ = -(position.z - worldOrigin.z);
        
        const chunkX = Math.floor(centerX / chunkSize.x);
        const chunkY = Math.floor(centerZ / chunkSize.z);
        
        // Check all 4 cardinal directions and find closest water
        const directions = [
            { dx: 0, dy: -1, rotation: 0, name: 'North' },
            { dx: 1, dy: 0, rotation: 90, name: 'East' },
            { dx: 0, dy: 1, rotation: 180, name: 'South' },
            { dx: -1, dy: 0, rotation: 270, name: 'West' }
        ];

        // Find the closest water direction
        for (const dir of directions) {
            const checkX = chunkX + dir.dx;
            const checkY = chunkY + dir.dy;
            
            // Check if out of bounds (water) or invalid chunk (water)
            if (checkX < 0 || checkX >= validChunkPositions.length ||
                checkY < 0 || checkY >= validChunkPositions[0].length ||
                validChunkPositions[checkX][checkY] === 0) {
                return dir.rotation;
            }
        }
        
        // Default rotation if no water found
        return 90;
    }

    /**
     * @param {any} scene
     * @param {Record<string, Array<any>>} craneWorldPositions
     */
    async addDockContainers(scene, craneWorldPositions) {
        // Add containers to dock chunks, passing crane positions for collision avoidance
        for (let chunk of this.chunkData) {
            if (chunk instanceof DockChunk) {
                // Get chunk coordinates
                const chunkX = Math.round((chunk.position.x - worldOrigin.x) / chunkSize.x);
                const chunkY = Math.round(-(chunk.position.z - worldOrigin.z) / chunkSize.z);
                const chunkKey = `${chunkX}_${chunkY}`;
                
                // Get crane positions for this chunk (if any)
                const cranePositions = craneWorldPositions[chunkKey] || [];
                
                await chunk.addContainers(scene, this.modelCache, cranePositions);
            }
        }
    }
    
    /**
     * @param {any} scene
     * @param {Array<any>} yardCranePositions
     */
    async addYardContainers(scene, yardCranePositions) {
        // Add containers to yard chunks, checking for crane presence
        for (let chunk of this.chunkData) {
            if (chunk instanceof YardChunk) {
                // Check if this chunk has a crane
                const chunkX = Math.round((chunk.position.x - worldOrigin.x) / chunkSize.x);
                const chunkY = Math.round(-(chunk.position.z - worldOrigin.z) / chunkSize.z);
                const hasCrane = yardCranePositions.some(crane => crane.x === chunkX && crane.y === chunkY);
                
                await chunk.addContainers(scene, this.modelCache, hasCrane);
            }
        }
    }

    togglePaths(visible) {
        this.showPath = visible;

        // this.vesselList.forEach((vessel) => {
        //     vessel.setPathVisible(visible);
        // });

        this.seagullList.forEach((seagull) => {
            seagull.setPathVisible(visible);
        });
    }

    turnOffEvent;
    turnOnEvent;

    // Load terrain
    async loadTerrain(scene) {

        const terrainX = -625;
        const terrainZ = 530;

        this.terrain = await loadModel("/visualizer/models/terrain.obj");
        // Add bump map
        const bumpTexture = new THREE.TextureLoader().load('/visualizer/textures/maps/bump.jpg');
        this.terrain.traverse((child) => {
            if (child.isMesh) {
                child.material.bumpMap = bumpTexture;
                child.material.bumpScale = 5;
            }
        });

        this.terrain.scale.set(300, 300, 300);
        this.terrain.position.y = layoutY - 2;

        // move to the border
        this.terrain.position.x = terrainX;
        this.terrain.position.z = terrainZ; 

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
        this.lighthouse.position.set(-250 - 370, layoutY + 10, 10 + 190);

        this.lighthouse.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        this.lighthouse.castShadow = true;
        this.lighthouse.receiveShadow = true;

        scene.add(this.lighthouse);

        this.lighthouseLight = new THREE.SpotLight(0xffffaa, 75000, 0, Math.PI / 5, 0.2, 2);
        this.lighthouseLight.position.set(-249.2 - 370, layoutY + 130, 10 + 190);
        this.lighthouseLight.castShadow = true;
        
        this.lighthouseLight.shadow.mapSize.width = 2048;
        this.lighthouseLight.shadow.mapSize.height = 2048;
        this.lighthouseLight.shadow.camera.near = 10;
        this.lighthouseLight.shadow.camera.far = 1000;
        this.lighthouseLight.shadow.camera.fov = 60;
        this.lighthouseLight.shadow.bias = -0.0001;
        
        this.lighthouseLight.target.position.set(-249.2 - 370, layoutY + 130, 300 + 190)
        scene.add(this.lighthouseLight.target);

        scene.add(this.lighthouseLight);

        // Add point light to lighthouse structure
        this.lighthousePointLight = new THREE.PointLight(0xffffaa, 10, 50, 0);
        this.lighthousePointLight.position.set(-249.2 - 370, layoutY + 125, 10 + 190);
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
        const modelPath = "/visualizer/models/seagull.obj";
        
        // Load model once and cache it
        if (!this.modelCache[modelPath]) {
            this.modelCache[modelPath] = await loadModel(modelPath);
            centerModel(this.modelCache[modelPath]);
        }
        
        // Clone the cached model for this crane instance
        const model = this.modelCache[modelPath].clone();
        let seagull = new Seagull(model, new THREE.Vector3(0, 0, 0), this);
        seagull.init(scene);

        this.seagullList.push(seagull);
    }


    async addVessel(name, position, scene) {
        const modelPath = "/visualizer/models/lowpoly/ship.obj";
        // Load model once and cache it
        if (!this.modelCache[modelPath]) {
            this.modelCache[modelPath] = await loadModel(modelPath);
            centerModel(this.modelCache[modelPath]);
        }
        // Clone the cached model for this vessel instance
        const model = this.modelCache[modelPath].clone();
        position.y -= 2; // Slightly lower vessel into water
        let vessel = new Vessel(name, model, position, this);
        vessel.init(scene);
        this.vesselList.push(vessel);
    }

    async addContainerCrane(name, position, scene, rotation = 0, scaleMultiplier = 1.0) {
        const modelPath = "/visualizer/models/lowpoly/crane1.obj";
        
        // Load model once and cache it
        if (!this.modelCache[modelPath]) {
            this.modelCache[modelPath] = await loadModel(modelPath);
        }
        
        // Clone the cached model for this crane instance with unique materials
        const model = this.modelCache[modelPath].clone();
        model.traverse((child) => {
            if (child.isMesh) {
                // Adjust mesh position to fix rotation pivot point
                child.position.z -= 25;
                child.position.y -= 5;
                child.position.x += 15;
                
                
                if (child.material) {
                // Clone materials to avoid shared material references
                    if (Array.isArray(child.material)) {
                        child.material = child.material.map(mat => mat.clone());
                    } else {
                        child.material = child.material.clone();
                    }
                }
            }
        });
        
        const rotationRadians = rotation * (Math.PI / 180);
        let crane = new Crane(name, model, position, rotationRadians);
        crane.init(scene, scaleMultiplier);

        this.craneList.push(crane);
    }

    async addYardGantryCrane(name, position, scene, rotation = 0, scaleMultiplier = 1.0) {
        const modelPath = "/visualizer/models/lowpoly/crane2.obj";
        
        // Load model once and cache it
        if (!this.modelCache[modelPath]) {
            this.modelCache[modelPath] = await loadModel(modelPath);
            centerModel(this.modelCache[modelPath]);
        }
        
        // Clone the cached model for this crane instance with unique materials
        const model = this.modelCache[modelPath].clone();
        
        model.traverse((child) => {
            if (child.isMesh) {
                // Clone materials to avoid shared material references
                if (child.material) {
                    if (Array.isArray(child.material)) {
                        child.material = child.material.map(mat => mat.clone());
                    } else {
                        child.material = child.material.clone();
                    }
                }
            }
        });
        
        const rotationRadians = rotation * (Math.PI / 180);
        let crane = new GantryCrane(name, model, position, rotationRadians);
        crane.init(scene, scaleMultiplier);

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

    /**
     * Return scheduled vessel entries active at the given time.
     * @param {Date} time
     * @returns {Array<{vesselId:string,dockId:string,arrival:Date,departure:Date}>}
     */
    getActiveVesselsAt(time) {
        if (!this.vesselSchedule || !Array.isArray(this.vesselSchedule)) return [];
        const active = this.vesselSchedule.filter(entry => {
            try {
                return entry.arrival <= time && time <= entry.departure;
            } catch (e) {
                return false;
            }
        });
        return active;
    }

    /**
     * Ensure vessels visible at simulation time: add arriving vessels and remove departed ones.
     * This method only instantiates vessels that are scheduled to be present at `simTime`.
     * @param {Date} simTime
     * @param {any} scene
     */
    async ensureVesselsVisibleAt(simTime, scene) {
        // Determine which vessel ids should be active
        const active = this.getActiveVesselsAt(simTime || new Date());
        const activeIds = new Set(active.map(a => String(a.vesselId)));

        // Remove vessels that are present but no longer active
        // Also remove any duplicate vessels for the same vesselId
        const seen = new Set();
        const toRemove = [];
        for (const v of this.vesselList) {
            const id = String(v.name);
            if (!activeIds.has(id)) {
                toRemove.push(v);
            } else if (seen.has(id)) {
                // Duplicate vessel, remove
                toRemove.push(v);
            } else {
                seen.add(id);
            }
        }
        for (const v of toRemove) {
            this.removeVessel(v);
        }

        // Add vessels that are active but not yet instantiated
        for (const entry of active) {
            const vesselId = String(entry.vesselId);
            if (this.vesselList.some(v => String(v.name) === vesselId)) {
                continue;
            }

            // Find the dock chunk for this schedule entry
            const dockChunk = this.chunkData.find(c => (c instanceof DockChunk) && (String(c.dockName) === String(entry.dockId) || String(c.dockName) === entry.dockId));
            if (!dockChunk) {
                continue;
            }

            // Determine water-facing rotation and compute spawn position
            const rotationDeg = this.calculateWaterFacingRotationFromPosition(dockChunk.position);
            const vesselOffset = 20;
            const pos = dockChunk.position.clone();
            const spanRand = (Math.random() - 0.5) * (chunkSize.x / 4);
            switch (rotationDeg) {
                case 0:
                    pos.z -= chunkSize.z / 2 + vesselOffset;
                    pos.x += spanRand;
                    break;
                case 90:
                    pos.x += chunkSize.x / 2 + vesselOffset;
                    pos.z += spanRand;
                    break;
                case 180:
                    pos.z += chunkSize.z / 2 + vesselOffset;
                    pos.x += spanRand;
                    break;
                case 270:
                    pos.x -= chunkSize.x / 2 + vesselOffset;
                    pos.z += spanRand;
                    break;
                default:
                    pos.z -= chunkSize.z / 2 + vesselOffset;
                    pos.x += spanRand;
            }
            pos.y = layoutY + (chunkSize.y / 2) - 2;

            await this.addVessel(vesselId, pos, scene);
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
            const lighthouseCenter = { x: -249.2 - 370, z: 10 + 190 };
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
    pick(scene, camera, event) {
        const evt = event || window.event;
        const normalizedPosition = {
            x: (evt.clientX / window.innerWidth) * 2 - 1,
            y: -(evt.clientY / window.innerHeight) * 2 + 1
        };

        const craneMeshes = [];
        this.craneList.forEach(crane => {
            crane.model.traverse((child) => {
                if (child.isMesh) {
                    craneMeshes.push(child);
                }
            });
        });

        const vesselMeshes = [];
        this.vesselList.forEach(vessel => {
            vessel.model.traverse((child) => {
                if (child.isMesh) {
                    vesselMeshes.push(child);
                }
            });
        });

        const objectlist = [
            ...this.chunkData.map(chunk => chunk.base).filter(base => base),
            ...vesselMeshes,
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

            if(pickedObject.userData && pickedObject.userData.vesselName) {
                const clickedVessel = this.vesselList.find(vessel => vessel.name === pickedObject.userData.vesselName);
                if (clickedVessel) {
                    clickedVessel.model.traverse((child) => {
                        if (child.isMesh) {
                            highlightMesh(child, 0x444477);
                        }
                    });
                }
            } else {
                highlightMesh(this.selectedObject, 0x444477);
            }

            if (pickedObject.userData && pickedObject.userData.craneId) {
                const clickedCrane = this.craneList.find(crane => crane.name === pickedObject.userData.craneId);
                if (clickedCrane) {
                    clickedCrane.meshes.forEach((mesh) => {
                        highlightMesh(mesh, 0x444477);
                    });
                } else {
                    highlightMesh(this.selectedObject, 0x444477);
                }
            } else {
                highlightMesh(this.selectedObject, 0x444477);
            }
        } else {
            this.selectedObject = null;
            hideInfoText();
        }
    }

}