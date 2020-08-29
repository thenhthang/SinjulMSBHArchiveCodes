console.log('js is working')

const serviceWorker = 'service.js';
let isSubscribed = false;

function errorHandler(message, e) {
    if (typeof e === 'undefined') {
        e = null;
    }
    console.error(message,e);
}

const check = () => {
    if (!('serviceWorker' in navigator)) {
        throw new Error('No Service Worker support!')
    }
    if (!('PushManager' in window)) {
        throw new Error('No Push API Support!')
    }
}

// I added a function that can be used to register a service worker.
const registerServiceWorker = async () => {
    //notice the file name
    const swRegistration = await navigator.serviceWorker.register(serviceWorker);
    if (swRegistration.installing) {
        console.log('Service worker installing');
    } else if (swRegistration.waiting) {
        console.log('Service worker installed');
    } else if (swRegistration.active) {
        console.log('Service worker active');
    }
    // Are Notifications supported in the service worker?
    if (!(swRegistration.showNotification)) {
        errorHandler('[initialiseState] Notifications aren\'t supported on service workers.');
        return;
    }
    // We need the service worker registration to check for a subscription
    navigator.serviceWorker.ready.then(function (reg) {
        // Do we already have a push message subscription?
        reg.pushManager.getSubscription()
            .then(function (subscription) {
                isSubscribed = subscription;
                if (isSubscribed) {
                    console.log('User is already subscribed to push notifications');
                } else {
                    console.log('User is not yet subscribed to push notifications');
                }
            })
            .catch(function (err) {
                console.log('[req.pushManager.getSubscription] Unable to get subscription details.', err);
            });
    });
    return swRegistration;
}

// request permission on page load
document.addEventListener('DOMContentLoaded', async function () {
    // Permission for Notification
    const requestNotificationPermission = async () => {
        const permission = await window.Notification.requestPermission();

        // This will output: granted, default or denied
        console.log(Notification.permission)

        // value of permission can be 'granted', 'default', 'denied'
        // granted: user has accepted the request
        // default: user has dismissed the notification permission popup by clicking on x
        // denied: user has denied the request.
        if (permission === 'denied') {
            errorHandler('[Notification.requestPermission] Browser denied permissions to notification api .. !!!!');
            throw new Error('Permission not granted for Notification');
        } else if (permission === 'granted') {
            console.log('[Notification.requestPermission] Initializing service worker.');
        }
    }
})


// Local Notification
const showLocalNotification = (title, body, swRegistration) => {
    const options = {
        body,
        // here you can add more properties like icon, image, vibrate, etc.
    };
    const options2 = {
        // here you can add more properties like icon, image, vibrate, etc.
        // "//": "Visual Options",
        "body": "<String>",
        "icon": "<URL String>",
        "image": "<URL String>",
        "badge": "<URL String>",
        "vibrate": "<Array of Integers>",
        "sound": "<URL String>",
        "dir": "<String of 'auto' | 'ltr' | 'rtl'>",

        // "//": "Behavioural Options",
        "tag": "<String>",
        "data": "<Anything>",
        "requireInteraction": "<boolean>",
        "renotify": "<Boolean>",
        "silent": "<Boolean>",

        // "//": "Both Visual & Behavioural Options",
        "actions": "<Array of Strings>",

        // "//": "Information Option. No visual affect.",
        "timestamp": "<Long>"
    }
    swRegistration.showNotification(title, options);
}

//notice I changed main to async function so that I can use await for registerServiceWorker
const main = async () => {
    check();
    const swRegistration = await registerServiceWorker();
    // const permission = await requestNotificationPermission();
    showLocalNotification('This is title', 'this is the message', swRegistration);
}

// we will not call main in the beginning.
// main(); 
