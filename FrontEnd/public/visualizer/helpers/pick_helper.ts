import * as THREE from "three";

export default class PickHelper {
    
    raycaster;
    mouse;

    constructor() {
        this.raycaster = new THREE.Raycaster();
        this.mouse = new THREE.Vector2();
    }

    pick(normalizedPosition, scene, camera) {
        this.mouse.x = normalizedPosition.x;
        this.mouse.y = normalizedPosition.y;

        this.raycaster.setFromCamera(this.mouse, camera);

        const intersectedObjects = this.raycaster.intersectObjects(scene.children);
        if (intersectedObjects.length > 0) {
            return intersectedObjects[0];
        } else {
            return null;
        }
    }

    pickFromList(normalizedPosition, scene, camera, objects) {
        this.mouse.x = normalizedPosition.x;
        this.mouse.y = normalizedPosition.y;

        this.raycaster.setFromCamera(this.mouse, camera);

        const intersectedObjects = this.raycaster.intersectObjects(objects);
        if (intersectedObjects.length > 0) {
            return intersectedObjects[0];
        } else {
            return null;
        }
    }
}