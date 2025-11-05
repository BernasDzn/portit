import * as THREE from 'three';
import {OBJLoader} from 'three/examples/jsm/loaders/OBJLoader.js';
import {MTLLoader} from 'three/examples/jsm/loaders/MTLLoader.js';

function loadModel(path) {

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

export { loadModel, loadModelRaw };