import { GUI } from 'three/examples/jsm/libs/lil-gui.module.min.js';

export default function setupGUI(port3D) {
    const gui = new GUI();

    const lightingFolder = gui.addFolder('Lighting');
    const ambientLight = port3D.environment.ambientLight;
    lightingFolder.add(ambientLight, 'intensity', 0, 2).name('Ambient Intensity');
    lightingFolder.add(port3D.environment, 'sunAngle', 0, Math.PI * 2).name('Sun Angle');

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
}