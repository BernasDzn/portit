import * as THREE from "three";
import { Water } from 'three/examples/jsm/objects/Water.js'; // Water tutorial

const waterLevel = -5;
const waterColor = 0x00008B;

const fogColor = 0xcce0ff;

const waterNormalsPath = '/visualizer/textures/maps/waternormals.jpg';

async function createWaterFromPlaneGeometry(object, scene) {
    return new Promise((resolve) => {
        object.geometry.computeBoundingBox();
    
        const boundingBox = object.geometry.boundingBox;
    
        const width = boundingBox.max.x - boundingBox.min.x;
        const height = boundingBox.max.y - boundingBox.min.y;
        const waterGeometry = new THREE.PlaneGeometry(width, height);
    
        new THREE.TextureLoader().load(waterNormalsPath, (texture) => {
            texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
        
            let waterOptions = {
                textureWidth: 512,
                textureHeight: 512,
                waterNormals: texture,
                sunDirection: new THREE.Vector3(),
                sunColor: 0xffffff,
                waterColor: waterColor,
                distortionScale: 3.7,
                fog: scene.fog !== undefined,
            }
                    
            const water = new Water(waterGeometry, waterOptions);
        
            water.rotation.x = -Math.PI / 2;
            water.position.copy(object.position);
            resolve(water);
        });
    });
}

export default class Environment {

    ambientLight;
    directionalLight;
    skybox;

    water;

    
    constructor() {
        this.ambientLight = null;
        this.directionalLight = null;
        this.skybox = null;
    }

    async init(scene) {
        // Ambient light
        this.ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
        scene.add(this.ambientLight);

        // Directional light
        this.directionalLight = new THREE.DirectionalLight(0xffffff, 1.0);
        this.directionalLight.position.set(5, 10, 7.5);
        this.directionalLight.castShadow = true;

        scene.add(this.directionalLight);

        // Fog
        // scene.fog = new THREE.Fog(fogColor, 10,180);

        // Skybox
        let materialArray = [
            new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_ft.jpg")}),
            new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_bk.jpg")}),
            new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_up.jpg")}),
            new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_dn.jpg")}),
            new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_rt.jpg")}),
            new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_lf.jpg")}),
        ];

        // Set the side of each material to BackSide, so we see the inside of the box
        for (let i = 0; i < materialArray.length; i++)
            materialArray[i].side = THREE.BackSide;

        let letSkyboxGeometry = new THREE.BoxGeometry(200, 200, 200);
        this.skybox = new THREE.Mesh(letSkyboxGeometry, materialArray);
        scene.add(this.skybox);

        // Water plane
        let waterGeometry = new THREE.PlaneGeometry(200, 200);

        let waterPlane = new THREE.Mesh(waterGeometry);
        waterPlane.rotation.x = -Math.PI / 2;
        waterPlane.position.y = waterLevel;
        waterPlane.name = "WaterPlane";
        scene.add(waterPlane);

        // Make water from water plane
        this.water = await createWaterFromPlaneGeometry(waterPlane, scene);
        scene.remove(waterPlane);
        scene.add(this.water);
    }

    update() {

        // https://github.com/ClassOutside/ThreeJS_Water_Object/tree/main
        if (this.water) {
            this.water.material.uniforms['time'].value += 1.0 / 360.0;
        }
    }
}