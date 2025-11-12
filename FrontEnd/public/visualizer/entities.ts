import * as THREE from "three";
import {loadModel, loadModelRaw} from "./helpers/model_helper.ts";
import {makeBillboard} from "./helpers/billboard_helper.ts";
import { chunkIndexToPosition } from "./chunk_layout.ts";

const pathHeightOffset = 50;

export default class Vessel {
    name;
    model;
    position;

    bouyanceAmplitude = 0.8;
    bouyanceSpeed = 0.002;

    label;

    path;
    curve;
    currentPointIndex = 0;

    constructor(name, model, position) {
        this.name = name;
        this.model = model;
        this.position = position;
    }

    init(scene) {
        this.model.position.copy(this.position);
        this.model.scale.set(0.05, 0.05, 0.05);

        // Enable shadows for all child meshes
        this.model.traverse((child) => {
            if (child.isMesh) {
                child.castShadow = true;
                child.receiveShadow = true;
            }
        });

        this.model.receiveShadow = true;
        this.model.castShadow = true;

        this.model.isRoot = true;

        this.model.children[1].meta = {
            title: 'Vessel',
            description: `${this.name} is a vessel`,
            killable: true,
            killFunction: () => { this.kill(); }
        };

        scene.add(this.model);

        // Make label billboard
        this.label = makeBillboard(this.name, 16, 0xffffff);

        const labelOffset = new THREE.Vector3(0, 10, 0);
        const vesselRoot = new THREE.Object3D();
        vesselRoot.position.copy(this.position).add(labelOffset);
        vesselRoot.add(this.label);

        scene.add(vesselRoot);

        this.setPath(scene, [
            new THREE.Vector3(this.position.x + 50, this.position.y, this.position.z + 50),
            new THREE.Vector3(this.position.x + 100, this.position.y, this.position.z + 30),
            new THREE.Vector3(this.position.x + 190, this.position.y, this.position.z + 180),
            new THREE.Vector3(this.position.x + 250, this.position.y, this.position.z + 100),
            new THREE.Vector3(this.position.x + 300, this.position.y, this.position.z + 150),
            new THREE.Vector3(this.position.x + 350, this.position.y, this.position.z + 50),
        ]);
        // this.makePathFromChunks([
        //     {x: 5, y: 4},
        //     {x: 5, y: 3},
        //     {x: 5, y: 4},
        //     {x: 3, y: 2},
        // ]);
    }

    makePathFromChunks(chunkList) {
        let pathPoints = [];
        for (const chunk of chunkList) {
            let pos = chunkIndexToPosition(chunk.x, chunk.y, false);
            pos.y = this.position.y;
            pathPoints.push(pos);
        }
        
        this.setPath(this.model.parent, pathPoints);
    }

    facePoint(targetPoint) {
        const target = targetPoint.clone();
        target.y = this.model.position.y; // Keep y level

        this.model.lookAt(target);
        //this.model.rotation.y += Math.PI / 2; // Adjust for model facing direction
    }

    setPath(scene, path){

        path.forEach((point) => { point.y += pathHeightOffset; });
        path.unshift(new THREE.Vector3(this.position.x, this.position.y + pathHeightOffset, this.position.z)); // Add starting point
        this.curve = new THREE.CatmullRomCurve3(path);
        
        const points = this.curve.getPoints(50);
        const geometry = new THREE.BufferGeometry().setFromPoints(points);
        const material = new THREE.LineBasicMaterial({ color: 0x00ff00 });
        this.path = new THREE.Line(geometry, material);

        scene.add(this.path);
        this.facePoint(path[1]); // Face the first point in the path
    }

    goOnAnAdventure() {

        // Lepr through path points
        if (!this.path) return;

        if (this.currentPointIndex >= 100) {
            this.currentPointIndex = 0;
        }

        let point = this.curve.getPoint(this.currentPointIndex / 100);
        this.model.position.copy(point);
        this.model.position.y -= pathHeightOffset; // Adjust for height offset
    
        // Face next point
        const nextPoint = this.curve.getPoint((this.currentPointIndex + 5) / 100);
        this.facePoint(nextPoint);
    
        this.currentPointIndex += 0.1;
    }

    update() {
        
        // Simple boyance effect
        this.model.position.y = this.position.y + Math.sin(Date.now() * this.bouyanceSpeed) * this.bouyanceAmplitude;
        this.model.rotation.y = Math.sin(Date.now() * this.bouyanceSpeed) * (this.bouyanceAmplitude / 50) + Math.PI;

        this.goOnAnAdventure();
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

    init(scene) {
        this.model.position.copy(this.position);
        this.model.scale.set(0.1, 0.1, 0.1);
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
            }
        });
        
        scene.add(this.model);

        // Make label billboard
        this.label = makeBillboard(this.name, 16, 0xffffff);
        const labelOffset = new THREE.Vector3(0, 15, 0);
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