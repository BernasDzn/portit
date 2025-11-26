import * as THREE from "three";

export default class PickHelper {
    
    raycaster;
    mouse;
    controls;

    constructor(controls) {
        this.raycaster = new THREE.Raycaster();
        this.mouse = new THREE.Vector2();
        this.controls = controls;
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

    clamp = (val, min, max) => Math.min(Math.max(val, min), max);
    
    centerCameraOnObject(object, camera) {
        const box = new THREE.Box3().setFromObject(object);
        const center = box.getCenter(new THREE.Vector3());

        const cameraX = this.clamp(camera.position.x, center.x-500, center.x+500);
        const cameraY = this.clamp(camera.position.y, center.y-500, center.y+500);
        const cameraZ = this.clamp(camera.position.z, center.z+100, center.z+500);

        // camera.position.set(cameraX, cameraY, cameraZ);
        // camera.lookAt(center);

        // this.controls.target.copy(center);
        // this.controls.update();
        this.controls.animateTo(
            new THREE.Vector3(cameraX, cameraY, cameraZ),
            camera.quaternion,
            center
        );
    }


    pickFromList(normalizedPosition, scene, camera, objects) {
        this.mouse.x = normalizedPosition.x;
        this.mouse.y = normalizedPosition.y;

        this.raycaster.setFromCamera(this.mouse, camera);

        const intersectedObjects = this.raycaster.intersectObjects(objects);
        if (intersectedObjects.length > 0) {
            this.centerCameraOnObject(intersectedObjects[0].object, camera);
            return intersectedObjects[0];
        } else {
            return null;
        }
    }
}