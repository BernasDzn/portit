import * as THREE from 'three';
import {OBJLoader} from 'three/examples/jsm/loaders/OBJLoader.js';
import {MTLLoader} from 'three/examples/jsm/loaders/MTLLoader.js';

function loadModel(path) {

    return new Promise((resolve, reject) => {
        const loader = new OBJLoader();
        const mtlLoader = new MTLLoader();
        loader.load(
            path,
            (object) => {
                
                mtlLoader.load(
                    path.replace('.obj', '.mtl'),
                    (materials) => {
                        materials.preload();
                        object.traverse((child) => {
                            if (child.isMesh) {
                                child.material = materials.materials[child.name] || child.material;
                            }
                        });
                        resolve(object);
                    },
                    undefined,
                    (error) => {
                        console.warn('MTL file not found or failed to load, proceeding without materials.', error);
                        resolve(object); // Resolve with the object even if MTL fails
                    }
                );

            },
            undefined,
            (error) => {
                reject(error);
            }
        );
    });
}

export { loadModel };