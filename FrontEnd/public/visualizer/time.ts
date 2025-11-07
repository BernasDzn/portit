let currentDate;
const offsetDegrees = -90;

const timeIncrement = 10; // 10 minutes
const updateInterval = 1000; // 1 second

function incrementTimeByMinutes() {
    currentDate.setMinutes(currentDate.getMinutes() + timeIncrement);
}

function setSunAngle(app) {

    // calculate current sun angle
    const percentageOfTheDay = (currentDate.getHours() * 60 + currentDate.getMinutes()) / (24 * 60);
    const currentSunAngle = percentageOfTheDay * 2 * Math.PI;

    // offset to make it look better
    const offsetRadians = (offsetDegrees * Math.PI) / 180;
    app.environment.sunAngle = currentSunAngle + offsetRadians;
}

function getCurrentTime() {
    currentDate = new Date();
}

function updateDateTable() {

    document.getElementById('date').innerText = currentDate.toDateString();
    document.getElementById('time').innerText = currentDate.toTimeString().split(' ')[0];
}

function updateTime(app) {
    
    incrementTimeByMinutes();
    setSunAngle(app);
    updateDateTable();
}

export default function initTime(app) {
    
    getCurrentTime();
    updateDateTable();

    setTimeout(function tick() {

        if (app.paused) {
            setTimeout(tick, updateInterval * app.timeScale);
            return;
        }

        updateTime(app);
        setTimeout(tick, updateInterval * app.timeScale);
    }, updateInterval * app.timeScale);
}