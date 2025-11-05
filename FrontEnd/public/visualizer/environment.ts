import * as THREE from "three";
import { Water } from 'three/examples/jsm/objects/Water.js'; // Water tutorial
import { THREEx } from './helpers/threex.daynight.ts';

const waterLevel = -5;
const waterColor = 0x00008B;

const fogColor = 0xcccccc;

const waterNormalsPath = '/visualizer/textures/maps/waternormals.jpg';
const worldBorder = 1000;

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

    sunSphere;
    sunAngle = 1.5;
    sunLight;
    skydom;
    starField;
    
    constructor() {
        this.ambientLight = null;
        this.directionalLight = null;
        this.skybox = null;
    }

    camera;

    async init(scene, camera) {

        this.camera = camera;

        // Ambient light
        this.ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
        scene.add(this.ambientLight);

        // Directional light
        this.directionalLight = new THREE.DirectionalLight(0xffffff, 1.0);
        this.directionalLight.position.set(5, 10, 7.5);
        this.directionalLight.castShadow = true;

        scene.add(this.directionalLight);

        // Day night cycle 
        this.sunAngle = 1.9;
        this.sunSphere = new THREEx.DayNight.SunSphere();
        scene.add(this.sunSphere.object3d);
        this.sunLight = new THREEx.DayNight.SunLight();
        scene.add(this.sunLight.object3d);
        this.skydom	= new THREEx.DayNight.Skydom()
        scene.add(this.skydom.object3d);
        this.starField= new THREEx.DayNight.StarField();
        //scene.add(this.starField.object3d);

        // Fog
        //scene.fog = new THREE.Fog(fogColor, 200, 1200);

        // Skybox
        // let materialArray = [
        //     new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_ft.jpg")}),
        //     new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_bk.jpg")}),
        //     new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_up.jpg")}),
        //     new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_dn.jpg")}),
        //     new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_rt.jpg")}),
        //     new THREE.MeshBasicMaterial({map: new THREE.TextureLoader().load("/visualizer/textures/skybox/barren_lf.jpg")}),
        // ];

        // // Set the side of each material to BackSide, so we see the inside of the box
        // for (let i = 0; i < materialArray.length; i++){
        //     materialArray[i].side = THREE.BackSide;
        //     materialArray[i].fog = false;
        // }

        // let letSkyboxGeometry = new THREE.BoxGeometry(worldBorder, worldBorder, worldBorder);
        // this.skybox = new THREE.Mesh(letSkyboxGeometry, materialArray); 
        // scene.add(this.skybox);

        // Water plane
        let waterGeometry = new THREE.PlaneGeometry(worldBorder * 2, worldBorder * 2);

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
            this.water.material.uniforms['time'].value += 1.0 / 180.0;
            this.water.material.uniforms['sunDirection'].value.copy(this.sunLight.object3d.position).normalize();
            this.water.material.uniforms['sunColor'].value.copy(this.sunLight.object3d.color);
        }

        //https://github.com/jeromeetienne/threex.daynight
        this.sunSphere.update(this.sunAngle);
        this.sunLight.update(this.sunAngle);
        this.skydom.update(this.sunAngle);
        //console.log(this.starField);
    }
}