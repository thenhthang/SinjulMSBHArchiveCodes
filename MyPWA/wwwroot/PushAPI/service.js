console.log('Hello from service worker')

const applicationServerPublicKey =
    'BB7hoisYPa1KdE-wVnAjt361r4U4fKpk1sRFaJUjdqAN1hZUHa0lwJlyBQGQxSGcRBn6xaqBx_F5G7JIXca2HLc';

// Application Server Public Key defined in Views/Device/Create.cshtml
if (typeof applicationServerPublicKey === 'undefined') {
    errorHandler('Vapid public key is undefined .. !!!!');    
}

function errorHandler(message, e) {
    if (typeof e === 'undefined') {
        e = null;
    }
    console.error(message, e);
}

// urlB64ToUint8Array is a magic function that will encode the base64 public key
// to Array buffer which is needed by the subscription option
const urlB64ToUint8Array = base64String => {
    const padding = '='.repeat((4 - (base64String.length % 4)) % 4)
    const base64 = (base64String + padding).replace(/\-/g, '+').replace(/_/g, '/')
    const rawData = atob(base64)
    const outputArray = new Uint8Array(rawData.length)
    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i)
    }
    return outputArray
}

function base64Encode(arrayBuffer) {
    return btoa(String.fromCharCode.apply(null, new Uint8Array(arrayBuffer)));
}

// saveSubscription saves the subscription to the backend
const saveSubscription = async subscription => {
    const SERVER_URL = 'http://localhost:5001/SaveSubscription'
    const response = await fetch(SERVER_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(subscription),
    })
    return response.json()
}

self.addEventListener('activate', async () => {
    // This will be called only once when the service worker is activated.
    console.log('service worker activate')
    try {
        //navigator.serviceWorker.ready.then(async function (reg) {

            const options = {
                applicationServerKey: urlB64ToUint8Array(applicationServerPublicKey),
                userVisibleOnly: true
            }

            // const subscription = await reg.pushManager.subscribe(options)
            const subscription = await self.registration.pushManager.subscribe(options)

            const p256dh = base64Encode(subscription.getKey('p256dh'));
            const auth = base64Encode(subscription.getKey('auth'));
            const endpoint = subscription.endpoint;

            console.log(p256dh)
            console.log(auth)
            console.log(endpoint)

            //const response = await saveSubscription(subscription)
            //console.log(response)

        //})
    } catch (err) {
        errorHandler('[subscribe] Unable to subscribe to push', err)
    }
})

self.addEventListener("push", function (event) {
    if (!(self.Notification && self.Notification.permission === 'granted')) {
        return;
    }
    if (event.data) {
        console.log(event.data);
        console.log("Push event!! ", event.data.text());
        showLocalNotification("SinjulMSBH", event.data.text(), self.registration);
    } else {
        console.log("Push event but no data");
    }
});

const showLocalNotification = (title, body, swRegistration) => {
    const options = {
        body
        // here you can add more properties like icon, image, vibrate, etc.
    };
    swRegistration.showNotification(title, options);
};