import * as THREE from "three";
import {loadModel, loadModelRaw} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";
import { chunkIndexToPosition } from "./chunk_layout.ts";
import PathFollower from "./helpers/spline_helper.ts";

export default class Vessel {
    name;
    model;
    position;

    bouyanceAmplitude = 0.8;
    bouyanceSpeed = 0.002;

    label;

    path;
    layout;
    // path;
    // curve;
    // currentPointIndex = 0;

    constructor(name, model, position, layout) {
        this.name = name;
        this.model = model;
        this.position = position;
        this.position.y += 10;
        this.layout = layout;
    }

    init(scene) {
        this.model.position.copy(this.position);
        this.model.scale.set(1,1,1);

        // Vessel metadata
        const vesselMeta = {
            title: 'Vessel',
            description: `${this.name} is a vessel`,
            killable: true,
            killFunction: () => { this.kill(); }
        };

        // Enable shadows and add metadata to all child meshes
        this.model.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
                // Add metadata to each mesh so clicking any part shows vessel info
                child.meta = vesselMeta;
                child.userData.vesselName = this.name;
            }
        });

        this.model.receiveShadow = true;
        this.model.castShadow = true;

        this.model.isRoot = true;

        scene.add(this.model);

        // Make label billboard
        this.label = makeBillboard(this.name, 16, 0xffffff);

        const labelOffset = new THREE.Vector3(0, 10, 0);
        const vesselRoot = new THREE.Object3D();
        vesselRoot.position.copy(this.position).add(labelOffset);
        vesselRoot.add(this.label);

        scene.add(vesselRoot);

        // this.setPath(scene, [
        //     new THREE.Vector3(this.position.x + 50, this.position.y, this.position.z + 50),
        //     new THREE.Vector3(this.position.x + 100, this.position.y, this.position.z + 30),
        //     new THREE.Vector3(this.position.x + 190, this.position.y, this.position.z + 180),
        //     new THREE.Vector3(this.position.x + 250, this.position.y, this.position.z + 100),
        //     new THREE.Vector3(this.position.x + 300, this.position.y, this.position.z + 150),
        //     new THREE.Vector3(this.position.x + 350, this.position.y, this.position.z + 50),
        // ]);
        // this.makePathFromChunks([
        //     {x: 5, y: 4},
        //     {x: 5, y: 3},
        //     {x: 5, y: 4},
        //     {x: 3, y: 2},
        // ]);

        this.path = new PathFollower(
            [
                new THREE.Vector3(this.position.x + 50, this.position.y, this.position.z + 50),
                new THREE.Vector3(this.position.x + 100, this.position.y, this.position.z + 30),
                new THREE.Vector3(this.position.x + 190, this.position.y, this.position.z + 180),
                new THREE.Vector3(this.position.x + 250, this.position.y, this.position.z + 100),
                new THREE.Vector3(this.position.x + 300, this.position.y, this.position.z + 150),
                new THREE.Vector3(this.position.x + 350, this.position.y, this.position.z + 50),
            ],
            this.facePoint,
            scene,
            this.model,
            this.layout,
            50
        );
    }

    setPathVisible(visible) {
        if (this.path) {
            this.path.setVisible(visible);
        }
    }

    facePoint(targetPoint, model) {
        const target = targetPoint.clone();
        target.y = model.position.y; // Keep y level

        model.lookAt(target);
        //this.model.rotation.y += Math.PI / 2; // Adjust for model facing direction
    }

    update() {
        
        // Simple boyance effect
        this.model.position.y = this.position.y + Math.sin(Date.now() * this.bouyanceSpeed) * this.bouyanceAmplitude;
        this.model.rotation.y = Math.sin(Date.now() * this.bouyanceSpeed) * (this.bouyanceAmplitude / 50) + Math.PI;

        this.path.goOnAnAdventure();
        // update label position
        this.label.position.set(this.model.position.x, this.model.position.y + 10, this.model.position.z);
    }

    kill(){
        // Remove from scene
        if (this.model.parent) {
            this.model.parent.remove(this.model);
        }
        if (this.label.parent) {
            this.label.parent.remove(this.label);
        }
        if (this.path && this.path.parent) {
            this.path.parent.remove(this.path);
        }
    }
} 

export class Crane {
    name;
    model;
    position;
    rotation;

    label;
    meshes = [];

    constructor(name, model, position, rotation) {
        this.name = name;
        this.model = model;
        this.position = position;
        this.rotation = rotation;
    }

    init(scene, scaleMultiplier = 1.0) {
        this.model.position.copy(this.position);
        const baseScale = 2 * scaleMultiplier;
        this.model.scale.set(baseScale, baseScale, baseScale);
        this.model.rotation.y = this.rotation || 0;
        
        this.model.traverse((child) => {
            if (child.isMesh) {
                this.meshes.push(child);
                child.userData.craneId = this.name;
                child.meta = {
                    title: 'Crane',
                    description: `${this.name} - Container crane\nPosition: (${this.position.x.toFixed(2)}, ${this.position.y.toFixed(2)}, ${this.position.z.toFixed(2)})\nRotation: ${((this.rotation || 0) * 180 / Math.PI).toFixed(1)}°`,
                    killable: true,
                    killFunction: () => { this.kill(); }
                };
                
                if (child.material) {
                    child.material.depthTest = true;
                    child.material.depthWrite = true;
                    
                    if (child.material.transparent && child.material.opacity >= 1.0) {
                        child.material.transparent = false;
                    }
                    
                    child.material.polygonOffset = true;
                    child.material.polygonOffsetFactor = 1;
                    child.material.polygonOffsetUnits = 1;
                    
                    child.material.needsUpdate = true;
                }

                child.castShadow = true;
                child.receiveShadow = true;
            }
        });
        
        scene.add(this.model);

        // Make label billboard
        this.label = makeBillboard(this.name, 16, 0xffffff);
        const labelOffset = new THREE.Vector3(0, 15, -15);
        const craneRoot = new THREE.Object3D();
        craneRoot.position.copy(this.position).add(labelOffset);
        craneRoot.add(this.label);
        scene.add(craneRoot);
    }

    update() {
        // update label position
        this.label.position.set(this.model.position.x, this.model.position.y + 15, this.model.position.z);
    }

    kill(){
        // Remove from scene
        if (this.model.parent) {
            this.model.parent.remove(this.model);
        }
        if (this.label.parent) {
            this.label.parent.remove(this.label);
        }
    }
}

export class Seagull {
    model;
    position;
    path;
    layout;

    constructor(model, position, layout) {
        this.model = model;
        this.position = position;
        this.layout = layout;
    }

    init(scene) {
        this.model.position.copy(this.position);
        this.model.scale.set(1.2, 1.2, 1.2);

        // Enable shadows for all child meshes
        this.model.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        this.model.receiveShadow = true;
        this.model.castShadow = true;

        // Make a random looping 3d path
        const rPath = [];
        for (let i = 0; i < 10; i++) {
            const offsetX = (Math.random() - 0.5) * 100 * 8;
            const offsetY = (Math.random() - 0.5) * 20 + 20;
            const offsetZ = (Math.random() - 0.5) * 100 * 8;
            rPath.push(new THREE.Vector3(this.position.x + offsetX, this.position.y + offsetY, this.position.z + offsetZ));
        }

        this.path = new PathFollower(
            rPath,
            this.facePoint,
            scene,
            this.model,
            this.layout,
            0,
            3.2
        );

        console.log(this.model)
        scene.add(this.model);
    }

    setPathVisible(visible) {
        if (this.path) {
            this.path.setVisible(visible);
        }
    }

    facePoint(targetPoint, model) {
        const target = targetPoint.clone();
        //target.y = model.position.y; // Keep y level

        model.lookAt(target);
        //this.model.rotation.y += Math.PI / 2; // Adjust for model facing direction
    }

    update() {
        
        this.path.goOnAnAdventure();
    }
}

export class GantryCrane {
    name;
    model;
    position;
    rotation;

    label;
    meshes = [];

    constructor(name, model, position, rotation) {
        this.name = name;
        this.model = model;
        this.position = position;
        this.rotation = rotation;
    }

    init(scene, scaleMultiplier = 1.0) {
        this.model.position.copy(this.position);
        const baseScale = 3 * scaleMultiplier;
        this.model.scale.set(baseScale, baseScale, baseScale);
        this.model.rotation.y = this.rotation || 0;
        
        this.model.traverse((child) => {
            if (child.isMesh) {
                this.meshes.push(child);
                child.userData.craneId = this.name;
                child.meta = {
                    title: 'Yard Gantry Crane',
                    description: `${this.name} - Yard gantry crane\nPosition: (${this.position.x.toFixed(2)}, ${this.position.y.toFixed(2)}, ${this.position.z.toFixed(2)})\nRotation: ${((this.rotation || 0) * 180 / Math.PI).toFixed(1)}°`,
                    killable: true,
                    killFunction: () => { this.kill(); }
                };
                
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });
        
        scene.add(this.model);

        // Make label billboard
        this.label = makeBillboard(this.name, 16, 0xffffff);
        const labelOffset = new THREE.Vector3(0, 15, -15);
        const craneRoot = new THREE.Object3D();
        craneRoot.position.copy(this.position).add(labelOffset);
        craneRoot.add(this.label);
        scene.add(craneRoot);
    }

    update() {
        // update label position
        this.label.position.set(this.model.position.x, this.model.position.y + 15, this.model.position.z);
    }

    kill(){
        // Remove from scene
        if (this.model.parent) {
            this.model.parent.remove(this.model);
        }
        if (this.label.parent) {
            this.label.parent.remove(this.label);
        }
    }
}