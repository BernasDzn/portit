import { ref } from "vue";

class Notification {
    message: string;
    type: string;

    constructor(message: string, type: string) {
        this.message = message;
        this.type = type;
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

function enqueueNotification(message: string, type: string) {
    const notif = new Notification(message, type);
    notificationList.value.push(notif);
}

function dequeueNotification() {
    notificationList.value.shift();
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
        clearNotifications,
    };
}