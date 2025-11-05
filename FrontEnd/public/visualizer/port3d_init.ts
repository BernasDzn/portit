import * as THREE from "three";
import Environment from "/visualizer/environment.ts";
import PortLayout from "./simulation_data.ts";
import Controls from "./controls.ts";

const clearColor = 0x000000;

export default class Port3D {

    scene;
    camera;
    renderer;
    controls;

    environment;
    portLayout;

    constructor(parameters) {
        // Create the scene
        this.scene = new THREE.Scene();
        this.scene.background = new THREE.Color(clearColor);

        // Create a perspective camera
        // this.camera = new THREE.PerspectiveCamera(
        //     60.0,
        //     window.innerWidth / window.innerHeight,
        //     0.1,
        //     1000.0
        // );
        this.camera = new THREE.PerspectiveCamera(
            15,
            window.innerWidth / window.innerHeight,
            1,
            3000
        );

        // Create the renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true });
        this.renderer.setPixelRatio(window.devicePixelRatio);
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        
        // Enable shadows
        this.renderer.shadowMap.enabled = true;
        this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;

        document.body.appendChild(this.renderer.domElement);

        // Create the orbit controls
        this.controls = new Controls(this.camera, this.renderer.domElement);

        // Listen for resize
        window.addEventListener("resize", () => this.onWindowResize());

        // Add environment
        this.environment = new Environment();
        this.environment.init(this.scene);

        // Add port base
        this.portLayout = new PortLayout(this.scene);
        this.portLayout.addVessel("Vessel 1", new THREE.Vector3(0, -12, -40), this.scene);

        this.animate();
    }

    onWindowResize() {
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(window.innerWidth, window.innerHeight);
    }

    animate() {
        requestAnimationFrame(() => this.animate());
        this.render();
    }

    render() {
        this.controls.update();
        this.environment.update();
        this.portLayout.update();

        this.renderer.render(this.scene, this.camera);

        // Print camera data
        // console.log(`Camera position: x=${this.camera.position.x}, y=${this.camera.position.y}, z=${this.camera.position.z}`);
        // console.log(`Camera rotation: x=${this.camera.rotation.x}, y=${this.camera.rotation.y}, z=${this.camera.rotation.z}`);
    }
}
