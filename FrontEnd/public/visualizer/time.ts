let currentDate;
const offsetDegrees = -90;

const timeIncrementSeconds = 1; // 30 seconds for smooth sun movement
const updateInterval = 16; // 1 second

var timedEvents = [];

function incrementTimeByMinutes(timeScale) {
    // Add seconds scaled by timeScale for smooth sun movement
    currentDate.setSeconds(currentDate.getSeconds() + (timeIncrementSeconds * timeScale));
}

function setSunAngle(app) {

    // calculate current sun angle with full precision (hours, minutes, and seconds)
    const totalSeconds = currentDate.getHours() * 3600 + currentDate.getMinutes() * 60 + currentDate.getSeconds();
    const percentageOfTheDay = totalSeconds / (24 * 3600);
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
    
    incrementTimeByMinutes(app.timeScale);
    setSunAngle(app);
    updateDateTable();

    // Check timed events
    timedEvents.forEach(event => {
        event.checkAndTrigger(currentDate);
    });
}

export class TimedEvent {
    targetHour;
    callback;
    triggeredToday = false;

    constructor(targetHour, callback) {
        this.targetHour = targetHour;
        this.callback = callback;

        timedEvents.push(this);
    }

    checkAndTrigger(currentDate) {
        if (currentDate.getHours() === this.targetHour) {
            if (!this.triggeredToday) {
                this.callback();
                this.triggeredToday = true;
            }
        } else {
            this.triggeredToday = false;
        }
    }
}

export default function initTime(app) {
    
    getCurrentTime();
    updateDateTable();

    setTimeout(function tick() {

        if (app.paused) {
            setTimeout(tick, updateInterval);
            return;
        }

        updateTime(app);
        setTimeout(tick, updateInterval);
    }, updateInterval);
}