import * as THREE from "three";

const pathHeightOffset = 50;

function randomColour() {
    return Math.floor(Math.random() * 16777215);
}

export default class PathFollower {
    path;
    curve; // the Catmull-Rom curve
    currentPointIndex = 0;
    facePoint;
    model;

    constructor(pathPoints, facePoint, scene, model, layout, offsetHeight = 0) {
        this.facePoint = facePoint;
        this.model = model;

        this.curve = new THREE.CatmullRomCurve3(pathPoints);
        this.curve.closed = true;

        // Build visible line geometry
        const points = this.curve.getPoints(50);
        points.forEach(p => p.y += offsetHeight);
        const geometry = new THREE.BufferGeometry().setFromPoints(points);
        const material = new THREE.LineBasicMaterial({ color: randomColour() });

        this.path = new THREE.Line(geometry, material);
        this.path.visible = layout.showPaths;

        scene.add(this.path);
        this.facePoint(pathPoints[1], model); // face initial direction
    }

    setVisible(visible) {
        if (this.path) {
            this.path.visible = visible;
        }
    }

    goOnAnAdventure() {
        if (!this.curve) return;

        if (this.currentPointIndex >= 100) {
            this.currentPointIndex = 0;
        }

        const point = this.curve.getPoint(this.currentPointIndex / 100);
        this.model.position.copy(point);

        const nextPoint = this.curve.getPoint((this.currentPointIndex + 5) / 100);
        this.facePoint(nextPoint, this.model);

        this.currentPointIndex += 0.1;
    }
}
