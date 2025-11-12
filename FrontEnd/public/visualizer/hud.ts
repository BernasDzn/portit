import { GUI } from 'three/examples/jsm/libs/lil-gui.module.min.js';

export default function setupGUI(port3D) {
    const gui = new GUI();

    const lightingFolder = gui.addFolder('Lighting');
    const ambientLight = port3D.environment.ambientLight;
    lightingFolder.add(ambientLight, 'intensity', 0, 2).name('Ambient Intensity');
    lightingFolder.add(port3D.environment, 'sunAngle', 0, Math.PI * 2).name('Sun Angle');
    lightingFolder.open();

    const timeFolder = gui.addFolder('Time Control');
    timeFolder.add(port3D, 'timeScale', 0.1, 5).name('Time Scale');
    timeFolder.add(port3D, 'paused').name('Paused');
    timeFolder.open();

    const ppFolder = gui.addFolder('Post-Processing');
    const bloomSettings = port3D.portsProcessing.bloom;
    ppFolder.add(bloomSettings, 'strength', 0, 3).name('Bloom Strength').onChange(() => {
        port3D.updatePostProcessing();
    });
    ppFolder.add(bloomSettings, 'radius', 0, 1).name('Bloom Radius').onChange(() => {
        port3D.updatePostProcessing();
    });
    ppFolder.add(bloomSettings, 'threshold', 0, 1).name('Bloom Threshold').onChange(() => {
        port3D.updatePostProcessing();
    });

    const cameraFolder = gui.addFolder('Camera Position');
    cameraFolder.add(port3D.camera.position, 'x', -500, 500).listen();
    cameraFolder.add(port3D.camera.position, 'y', -500, 500).listen();
    cameraFolder.add(port3D.camera.position, 'z', -500, 500).listen();
    cameraFolder.open();

    const cameraRotationFolder = gui.addFolder('Camera Rotation');
    cameraRotationFolder.add(port3D.camera.rotation, 'x', -Math.PI, Math.PI).listen();
    cameraRotationFolder.add(port3D.camera.rotation, 'y', -Math.PI, Math.PI).listen();
    cameraRotationFolder.add(port3D.camera.rotation, 'z', -Math.PI, Math.PI).listen();
    cameraRotationFolder.open();

    const splineFolder = gui.addFolder('Spline Path');
    splineFolder.add(port3D.portLayout, 'showPaths').name('Toggle Paths').onChange(() => {
        port3D.portLayout.togglePaths(!port3D.portLayout.showPaths);
    });
}