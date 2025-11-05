import * as THREE from 'three';

function makeBillboard(text, size, colour) {
    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');
    context.font = `${size}px Arial`;
    context.fillStyle = `#${colour.toString(16).padStart(6, '0')}`;
    context.fillText(text, 0, 50);

    const texture = new THREE.CanvasTexture(canvas);
    texture.minFilter = THREE.LinearFilter;
    texture.wrapS = THREE.ClampToEdgeWrapping;
    texture.wrapT = THREE.ClampToEdgeWrapping;

    const labelMaterial = new THREE.SpriteMaterial({ 
        map: texture,
        side: THREE.DoubleSide,
        transparent: true
    });

    const label = new THREE.Sprite(labelMaterial);
    label.scale.set(100, 50, 1);
    return label;
}

export { makeBillboard };