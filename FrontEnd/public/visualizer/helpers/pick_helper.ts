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

    clamp(val, min, max) {
        const v = Number(val);
        let a = Number(min);
        let b = Number(max);

        if (Number.isNaN(v)) return v;
        if (Number.isNaN(a) || Number.isNaN(b)) return v;

        if (a > b) {
            const tmp = a;
            a = b;
            b = tmp;
        }

        if (v < a) return a;
        if (v > b) return b;
        return v;
    }
    
    centerCameraOnObject(object, camera) {
        const box = new THREE.Box3().setFromObject(object);
        const center = box.getCenter(new THREE.Vector3());

        const cameraX = this.clamp(camera.position.x, center.x-500, center.x+500);
        const cameraY = this.clamp(camera.position.y, center.y+450, center.y+550);
        const cameraZ = this.clamp(camera.position.z, center.z-500, center.z+500);

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

        // print position
        console.log("PickHelper pick at ", normalizedPosition, " found ", intersectedObjects.length, " objects.");

        if (intersectedObjects.length > 0) {
            this.centerCameraOnObject(intersectedObjects[0].object, camera);
            return intersectedObjects[0];
        } else {
            return null;
        }
    }
}