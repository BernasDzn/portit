import * as THREE from "three";
import { Water } from 'three/examples/jsm/objects/Water.js'; // Water tutorial
import { THREEx } from './helpers/threex.daynight.ts';

const waterLevel = -5;
const waterColor = 0x00008B;

const fogColor = 0xcccccc;

const waterNormalsPath = '/visualizer/textures/maps/waternormals.jpg';
const worldBorder = 1000;
const waterExtent = 4000; // Extend water to reach the skybox (skybox radius is 2000)

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
    hemisphereLight;
    directionalLight; pointLight;
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
    scene;

    async init(scene, camera) {

        this.camera = camera;
        this.scene = scene;

        // Ambient light
        this.ambientLight = new THREE.AmbientLight(0xffffff, 0.3);
        scene.add(this.ambientLight);

        this.hemisphereLight = new THREE.HemisphereLight(
            0x87ceeb,
            0x2c3e50,
            0.0
        );
        scene.add(this.hemisphereLight);

        // const testCube = new THREE.BoxGeometry(5, 5, 5);
        // const testCubeMesh = new THREE.Mesh(testCube);
        // testCubeMesh.position.set(-100, 30, 100);
        // scene.add(testCubeMesh);

        // Day night cycle 
        this.sunAngle = 1.9;
        this.sunSphere = new THREEx.DayNight.SunSphere();
        scene.add(this.sunSphere.object3d);
        this.sunLight = new THREEx.DayNight.SunLight();
        
        this.sunLight.object3d.castShadow = true;
        this.sunLight.object3d.shadow.mapSize.width = 4096;
        this.sunLight.object3d.shadow.mapSize.height = 4096;
        this.sunLight.object3d.shadow.camera.near = 1;
        this.sunLight.object3d.shadow.camera.far = 100000; // sun distance is 90000
        this.sunLight.object3d.shadow.camera.left = -600;
        this.sunLight.object3d.shadow.camera.right = 600;
        this.sunLight.object3d.shadow.camera.top = 600;
        this.sunLight.object3d.shadow.camera.bottom = -600;
        
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

        // Water plane - extends to skybox
        let waterGeometry = new THREE.PlaneGeometry(waterExtent, waterExtent);

        let waterPlane = new THREE.Mesh(waterGeometry);
        waterPlane.rotation.x = -Math.PI / 2;
        waterPlane.position.y = waterLevel;
        waterPlane.name = "WaterPlane";
        waterPlane.castShadow = true;
        waterPlane.receiveShadow = true;
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
        
        if (this.sunLight && this.sunLight.object3d) {
            var phase = Math.sin(this.sunAngle) > Math.sin(0) ? 'day' : 
                       Math.sin(this.sunAngle) > Math.sin(-Math.PI/6) ? 'twilight' : 'night';
            
            if (phase === 'night') {
                this.hemisphereLight.intensity = 0.5;
            } else if (phase === 'twilight') {
                this.hemisphereLight.intensity = 0.15;
            } else {
                this.hemisphereLight.intensity = 0.0;
            }
            
            if (!this.sunLight.object3d.target.parent) {
                this.scene.add(this.sunLight.object3d.target);
            }
            this.sunLight.object3d.target.position.set(0, 0, 0);
            this.sunLight.object3d.target.updateMatrixWorld();
            
            this.sunLight.object3d.updateMatrixWorld(true);
            
            if (this.sunLight.object3d.shadow && this.sunLight.object3d.shadow.camera) {
                this.sunLight.object3d.shadow.camera.position.copy(this.sunLight.object3d.position);
                this.sunLight.object3d.shadow.camera.lookAt(0, 0, 0);
                this.sunLight.object3d.shadow.camera.updateProjectionMatrix();
                this.sunLight.object3d.shadow.camera.updateMatrixWorld(true);
            }
        }
    }
}