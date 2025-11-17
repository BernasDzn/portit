import * as THREE from "three";

import { EffectComposer } from "three/addons/postprocessing/EffectComposer.js";
import { RenderPass } from "three/addons/postprocessing/RenderPass.js";
import { ShaderPass } from "three/addons/postprocessing/ShaderPass.js";
import { UnrealBloomPass } from "three/addons/postprocessing/UnrealBloomPass.js";
import { RenderPixelatedPass } from 'three/addons/postprocessing/RenderPixelatedPass.js';

import Environment from "/visualizer/environment.ts";
import PortLayout from "./chunk_layout.ts";
import Controls from "./controls.ts";
import setupGUI from "./hud.ts";
import initTime from "./time.ts";

const clearColor = 0x000000;

export default class Port3D {

    scene;
    camera;
    renderer;
    controls;

    timeScale = 1.0;
    paused = false;

    environment;
    portLayout;

    composer; // Post-processing composer
    portsProcessing = {
        bloom: {
            strength: 0.5,
            radius: 0.8,
            threshold: 0.6
        }
    }

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
            10000
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

        // Setup post-processing
        this.composer = new EffectComposer(this.renderer);

        // Create a basic render pass
        const renderPass = new RenderPass(this.scene, this.camera);
        this.composer.addPass(renderPass);

        // Bloom pass
        const bloomPass = new UnrealBloomPass(
            new THREE.Vector2(window.innerWidth, window.innerHeight),
            this.portsProcessing.bloom.strength,
            this.portsProcessing.bloom.radius,
            this.portsProcessing.bloom.threshold
        );
        this.composer.addPass(bloomPass);

        // const renderPixelatedPass = new RenderPixelatedPass( 4, this.scene, this.camera );
        // this.composer.addPass( renderPixelatedPass );

        // Add environment
        this.environment = new Environment();
        this.environment.init(this.scene, this.camera);

        // Add port base
        this.portLayout = new PortLayout(this.scene, this.camera);
        this.portLayout.addVessel("Vessel 1", new THREE.Vector3(0, -12, -40), this.scene);

        for (let i = 0; i < 3; i++) 
            this.portLayout.addSeagull(this.scene);

        // Setup time manager
        initTime(this);

        // Setup GUI
        setupGUI(this);

        this.animate();
    }

    onWindowResize() {
        this.camera.aspect = window.innerWidth / window.innerHeight;
        this.camera.updateProjectionMatrix();
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        this.composer.setSize(window.innerWidth, window.innerHeight);
    }
    
    animate() {
        requestAnimationFrame(() => this.animate());
        this.render();
    }

    render() {
        this.controls.update();
        this.environment.update();
        this.portLayout.update();

        //this.renderer.render(this.scene, this.camera);

        // Render using post-processing pipeline
        this.composer.render();

        // Print camera data
        // console.log(`Camera position: x=${this.camera.position.x}, y=${this.camera.position.y}, z=${this.camera.position.z}`);
        // console.log(`Camera rotation: x=${this.camera.rotation.x}, y=${this.camera.rotation.y}, z=${this.camera.rotation.z}`);
    }

    updatePostProcessing() {
        // Update bloom pass settings
        const bloomPass = this.composer.passes.find(pass => pass instanceof UnrealBloomPass);
        if (bloomPass) {
            bloomPass.strength = this.portsProcessing.bloom.strength;
            bloomPass.radius = this.portsProcessing.bloom.radius;
            bloomPass.threshold = this.portsProcessing.bloom.threshold;
        }
    }
}
