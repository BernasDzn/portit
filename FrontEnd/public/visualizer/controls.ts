import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import * as THREE from "three";

const minZoomDistance = 50; // Minimum distance for zooming
const maxZoomDistance = 3000; // Maximum distance for zooming

const zoomIncrement = 10;
const moveDistance = 20; // Distance to move per key press

export default class Controls {

    originalRotation;
    originalPosition;

    camera;
    controls;
    clock;

    raycaster = new THREE.Raycaster();

    constructor(camera, domElement) {

        this.clock = new THREE.Clock();

        this.camera = camera;

        this.controls = new OrbitControls(this.camera, domElement);
        this.controls.autoRotate = false;
        this.controls.enableDamping = true;
        this.controls.enablePan = true;
        this.controls.enableDamping = false;
        this.controls.minDistance = minZoomDistance;
        this.controls.maxDistance = maxZoomDistance;
        this.controls.maxPolarAngle = Math.PI / 2;

        this.controls.mouseButtons = {
            LEFT: THREE.MOUSE.PAN,  // drag with LMB
            MIDDLE: THREE.MOUSE.DOLLY,
            RIGHT: THREE.MOUSE.ROTATE
        };

        // Zoom speed control
        this.controls.zoomSpeed = zoomIncrement;
                  
        this.camera.position.set(493, 411, -766);
        this.camera.rotation.set(-2.6, 0.5, 2.8);

        this.originalRotation = this.camera.rotation.clone();
        this.originalPosition = this.camera.position.clone();

        // Add reset keybind
        window.addEventListener('keydown', (event) => {
            if (event.key === 'r' || event.key === 'R') {
                this.reset();
            }
        });
    }

    reset() {
        // this.camera.rotation.copy(this.originalRotation);
        // this.camera.position.copy(this.originalPosition);

        // this.controls.target.set(0, 0, 0);
        // this.controls.update();
        this.animateTo(
            this.originalPosition,
            new THREE.Quaternion().setFromEuler(this.originalRotation),
            new THREE.Vector3(0, 0, 0),
            1.0
        );
    }

    animateTo(targetPos, targetQuat, targetLookAt, duration = 1.0) {
        this.isAnimating = true;
        this.animationDuration = duration;
    
        this.startPos = this.camera.position.clone();
        this.startQuat = this.camera.quaternion.clone();
        this.startTarget = this.controls.target.clone();
    
        this.endPos = targetPos.clone();
        this.endQuat = targetQuat.clone();
        this.endTarget = targetLookAt.clone();
    
        this.animationTime = 0;
    }

    update() {
        const dt = this.clock.getDelta();
    
        if (this.isAnimating) {
            this.animationTime += dt;
            const t = Math.min(this.animationTime / this.animationDuration, 1);
    
            // Lerp camera position
            this.camera.position.lerpVectors(this.startPos, this.endPos, t);
    
            // Slerp camera rotation
            this.camera.quaternion.slerp(this.endQuat, t);
    
            // Lerp OrbitControls target
            this.controls.target.lerpVectors(this.startTarget, this.endTarget, t);
    
            this.controls.update();
    
            if (t >= 1)
                this.isAnimating = false;

            return;
        }
    
        this.controls.update();
    }
    
}