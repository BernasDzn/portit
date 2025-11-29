import * as THREE from "three";
import {loadModel, loadModelRaw} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";
import { chunkIndexToPosition } from "./chunk_layout.ts";
import { findVesselPath } from "./helpers/path_find_helper.ts";
import PathFollower from "./helpers/spline_helper.ts";

function extrapolateForward(pos, distance) {
    // const forward = new THREE.Vector3();
    // model.getWorldDirection(forward); // vessel forward direction
    // forward.normalize();

    const forward = new THREE.Vector3(0, 0, -1);

    return pos.add(forward.multiplyScalar(distance)); 
}

export default class Vessel {
    name;
    model;
    position;
    scene;

    bouyanceAmplitude = 0.3;
    bouyanceSpeed = 0.002;

    label;

    path; arrivalPath;
    layout;

    docked = false;
    departed = false;
    readyToDie = false;
    // path;
    // curve;
    // currentPointIndex = 0;

    state = "unknown";

    constructor(name, model, position, layout) {
        this.name = name;
        this.model = model;
        this.position = position;
        this.position.y += 5.5;
        this.layout = layout;
    }

    onPathFinished() {
        console.log(`${this.name} - onPathFinished called. departed=${this.departed}, docked=${this.docked}`);
        if (this.departed) {
            this.readyToDie = true;
        } else {
            console.log(`${this.name} has docked.`);
            this.docked = true;
            this.state = "Docked"; 
        }
    }

    init(scene) {

        let distance = 500;
        distance += (Math.random() - 0.5) * 50; // random offset

        const startPosition = extrapolateForward(this.position.clone(), 500);

        this.model.position.copy(startPosition);
        this.model.scale.set(0.5,0.5,0.5);

        this.state = "Arriving";

        this.scene = scene;

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
        this.label.position.copy(this.model.position).add(new THREE.Vector3(0, 10, 0));
        scene.add(this.label);

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

        //this.path = new PathFollower(
        //    [
        //        new THREE.Vector3(this.position.x + 50, this.position.y, this.position.z + 50),
        //        new THREE.Vector3(this.position.x + 100, this.position.y, this.position.z + 30),
        //        new THREE.Vector3(this.position.x + 190, this.position.y, this.position.z + 180),
        //        new THREE.Vector3(this.position.x + 250, this.position.y, this.position.z + 100),
        //        new THREE.Vector3(this.position.x + 300, this.position.y, this.position.z + 150),
        //        new THREE.Vector3(this.position.x + 350, this.position.y, this.position.z + 50),
        //    ],
        //    this.facePoint,
        //    scene,
        //    this.model,
        //    this.layout,
        //    50
        //);

        this.arrivalPath = [
            startPosition,
            this.position
        ];
        this.path = new PathFollower(
            this.arrivalPath,
            this.facePoint,
            scene,
            this.model,
            this.layout,
            50,
            8.0,
            this.onPathFinished
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

    depart(){

        // if (!this.docked || this.departed) {
        //     console.log(`${this.name} cannot depart. docked=${this.docked}, departed=${this.departed}`);
        //     return;
        // }

        // Set a new path that is the reverse of the arrival path
        this.departed = true;
        this.state = "Departing";
        
        const departurePath = [...this.arrivalPath].reverse();
        // offset path on x
        for (let i = 0; i < departurePath.length; i++) {
            departurePath[i] = departurePath[i].clone();
            departurePath[i].x += 20;
        }

        // add transition point from current position to path start
        const transPoint = this.model.position.clone();
        transPoint.x += 10;
        transPoint.z += 10;
        departurePath.unshift(transPoint);
        // suffix current position to start of path
        departurePath.unshift(this.model.position.clone());

        console.log(`${this.name} departure path has ${departurePath.length} points`);
        this.path.setPath(departurePath);
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
    meta;

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
                
                const baseMeta = {
                    title: this.meta?.title || 'STS Crane',
                    description: `${this.meta?.description || (this.name + ' - Container crane')}\nPosition: (${this.position.x.toFixed(2)}, ${this.position.y.toFixed(2)}, ${this.position.z.toFixed(2)})\nRotation: ${((this.rotation || 0) * 180 / Math.PI).toFixed(1)}°`,
                    killable: true,
                    killFunction: () => { this.kill(); }
                };
                
                if (this.meta?.details) {
                    baseMeta.details = this.meta.details;
                }
                
                child.meta = baseMeta;
                
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

        // Make label billboard and add directly to scene (use world coords)
        this.label = makeBillboard(this.name, 16, 0xffffff);
        this.label.position.copy(this.model.position).add(new THREE.Vector3(0, 15, -15));
        scene.add(this.label);
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
            3.2,
            null,
            true
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
    meta;

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
                
                const baseMeta = {
                    title: this.meta?.title || 'Yard Crane',
                    description: `${this.meta?.description || (this.name + ' - Yard gantry crane')}\nPosition: (${this.position.x.toFixed(2)}, ${this.position.y.toFixed(2)}, ${this.position.z.toFixed(2)})\nRotation: ${((this.rotation || 0) * 180 / Math.PI).toFixed(1)}°`,
                    killable: true,
                    killFunction: () => { this.kill(); }
                };
                
                if (this.meta?.details) {
                    baseMeta.details = this.meta.details;
                }
                
                child.meta = baseMeta;
                
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });
        
        scene.add(this.model);

        // Make label billboard and add directly to scene (use world coords)
        this.label = makeBillboard(this.name, 16, 0xffffff);
        this.label.position.copy(this.model.position).add(new THREE.Vector3(0, 15, -15));
        scene.add(this.label);
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