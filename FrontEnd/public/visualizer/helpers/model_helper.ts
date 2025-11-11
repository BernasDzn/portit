import * as THREE from 'three';
import {OBJLoader} from 'three/examples/jsm/loaders/OBJLoader.js';
import {MTLLoader} from 'three/examples/jsm/loaders/MTLLoader.js';
import {GLTFLoader} from 'three/examples/jsm/loaders/GLTFLoader.js';

function loadModel(path) {
    // Detect file type and use appropriate loader
    if (path.endsWith('.gltf') || path.endsWith('.glb')) {
        return loadGLTF(path);
    } else if (path.endsWith('.obj')) {
        return loadOBJ(path);
    } else {
        return Promise.reject(new Error(`Unsupported file format: ${path}`));
    }
}

function loadOBJ(path) {

    return new Promise((resolve, reject) => {
        const objLoader = new OBJLoader();
        const mtlLoader = new MTLLoader();

        const mtlPath = path.replace('.obj', '.mtl');

        mtlLoader.load(
            mtlPath,
            (materials) => {
                materials.preload();
                // Apply materials to OBJLoader
                objLoader.setMaterials(materials);

                objLoader.load(
                    path,
                    (object) => {
                        resolve(object);
                    },
                    undefined,
                    (error) => reject(error)
                );
            },
            undefined,
            (error) => {
                console.warn('MTL not found, loading OBJ without materials:', error);

                // fallback, if not found load OBJ without materials
                objLoader.load(
                    path,
                    (object) => resolve(object),
                    undefined,
                    (error) => reject(error)
                );
            }
        );
    });
}

function loadGLTF(path) {
    return new Promise((resolve, reject) => {
        const loader = new GLTFLoader();
        loader.load(
            path,
            (gltf) => {
                // GLTF files return a scene, so we return the scene
                resolve(gltf.scene);
            },
            (progress) => {
                // Optional: log loading progress
                // console.log((progress.loaded / progress.total * 100) + '% loaded');
            },
            (error) => {
                console.error('Error loading GLTF:', error);
                reject(error);
            }
        );
    });
}

function loadModelRaw(path) {

    return new Promise((resolve, reject) => {
        const loader = new OBJLoader();
        loader.load(
            path,
            (object) => {
                resolve(object);
            },
            undefined,
            (error) => {
                reject(error);
            }
        );
    });
}

export { loadModel, loadModelRaw, loadGLTF };