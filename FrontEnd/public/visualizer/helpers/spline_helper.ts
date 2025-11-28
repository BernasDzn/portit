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
    speed;
    clock;
    layout;
    callback;

    stopAfterLap = true;
    loop;
    
    constructor(pathPoints, facePoint, scene, model, layout, offsetHeight = 0, speed = 4, callback = null, loop = false) {
        this.facePoint = facePoint;
        this.model = model;
        this.speed = speed;
        this.layout = layout;
        this.loop = loop;

        this.curve = new THREE.CatmullRomCurve3(pathPoints);
        this.curve.closed = loop;

        this.clock = new THREE.Clock();

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

    setPath(points) {
        if (!this.curve) return;
        this.curve = new THREE.CatmullRomCurve3(points);
        this.curve.closed = false;
        // Rebuild visible line geometry
        const linePoints = this.curve.getPoints(50);
        const geometry = new THREE.BufferGeometry().setFromPoints(linePoints);
        this.path.geometry = geometry;
        this.currentPointIndex = 0;
        this.clock = new THREE.Clock(); // Reset clock for new path
        if (points.length > 1) {
            this.facePoint(points[1], this.model);
        }
    }

    goOnAnAdventure() {
        if (!this.curve) return;

        if (this.currentPointIndex >= 100) {

            if (this.stopAfterLap && !this.loop) {
                if (this.callback) {
                    this.callback();
                }
                return;
            }
            this.currentPointIndex = 0;
        }

        const point = this.curve.getPoint(this.currentPointIndex / 100);
        this.model.position.copy(point);

        const nextPoint = this.curve.getPoint((this.currentPointIndex + 5) / 100);
        this.facePoint(nextPoint, this.model);

        // this.currentPointIndex += 0.1;
        this.currentPointIndex += this.clock.getDelta() * this.speed;
    }
}
