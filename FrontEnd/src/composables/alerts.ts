import { ref, nextTick } from "vue";

let _idCounter = 1

class Notification {
    id: number;
    message: string;
    type: string;
    open: boolean;
    duration?: number;

    constructor(message: string, type: string, duration?: number) {
        this.id = Date.now() + (_idCounter++)
        this.message = message;
        this.type = type;
        this.open = false // start closed, we'll open after mount to trigger animation
        this.duration = duration
    }
}

// What types of notifications are available
const notificationTypes = Object.freeze({
    SUCCESS: 'success',
    DANGER: 'danger',
    PRIMARY: 'primary',
    WARNING: 'warning',
    NEUTRAL: 'neutral',
});

// Map notification types to titles
const typeToTitleMap: Record<string, string> = {
    [notificationTypes.SUCCESS]: 'Success!',
    [notificationTypes.DANGER]: 'An error has occured',
    [notificationTypes.PRIMARY]: 'Information',
    [notificationTypes.WARNING]: 'Warning!',
    [notificationTypes.NEUTRAL]: 'Notice',
};

const notificationList = ref<Array<Notification>>([]);

function enqueueNotification(message: string, type: string, duration?: number) {
    const notif = new Notification(message, type, duration)
    notificationList.value.push(notif)
    // open after DOM updates so Shoelace animates the entrance
    nextTick(() => { notif.open = true })
    return notif.id
}

function dequeueNotification() {
    notificationList.value.shift();
}

function removeNotificationById(id: number) {
    const idx = notificationList.value.findIndex(n => n.id === id)
    if (idx !== -1) notificationList.value.splice(idx, 1)
}

function getNotificationTitle(notif: Notification): string {
    return typeToTitleMap[notif.type] || 'Notice';
}

function clearNotifications() {
    notificationList.value = [];
}

export function useAlerts() {
    return {
        notificationTypes,
        notificationList,
        getNotificationTitle,
        enqueueNotification,
        dequeueNotification,
        removeNotificationById,
        clearNotifications,
    };
}