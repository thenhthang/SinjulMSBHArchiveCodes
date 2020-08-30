
const SOME_URL = 'http://PushNotifications.JackSlater.Ir';

self.addEventListener('push', async event => {
    if (!(self.Notification
        && self.Notification.permission === 'granted'))
        return;

    // Local Notification
    await self.registration.showNotification(
        "SinjulMSBH",
        { body: "Hello from SinjulMSBH .. !!!!" }
    );

    // When to Show Notifications
    const clientList = await clients.matchAll();
    if (clientList.length === 0) {

        let body = {};
        if (event.data) {
            body = event.data.json();
        } else {
            body = 'Push message no payload';
        }

        console.log('Notification Received:');
        console.log(body);

        const title = body.title;
        const message = body.message;
        const icon = "SinjulMSBH.jpg";
        const image = "SinjulMSBH.jpg";

        console.log("waitUntil start");
        await event.waitUntil(self.registration.showNotification(title, {
            data: {
                dateOfArrival: Date.now(),
                primaryKey: 13
            },
            actions: [
                {
                    action: 'explore', title: 'Explore this new world',
                    icon: 'checkmark.png'
                },
                {
                    action: 'close', title: 'Close notification',
                    icon: 'xmark.png'
                },
            ],
            body: message,
            icon: icon,
            badge: icon,
            image: image,
            requireInteraction: true,
            //timeout: 8000,
            tag: 'SinjulMSBH',
            dir: 'ltr',
            lang: 'en-US',
            closeOnClick: true,
            renotify: true,
            maxActions: 2,
            vibrate: [100, 50, 100],
            silent: false,
            link: SOME_URL,
            timestamp: Math.floor(Date.now()),
        }));
        console.log("waitUntil end");
    } else {
        // Send a message to the page to update the UI
        console.log('Application is already open .. !!!!');
    }
});


self.addEventListener('notificationclick', async event => {

    // Notifications and Tabs
    const clientList = await clients.matchAll();
    const client = clis.find(client => c.visibilityState === 'visible');
    if (client !== undefined) {
        client.navigate(SOME_URL);
        client.focus();
    } else {
        // there are no visible windows. Open one.
        clients.openWindow(SOME_URL);
        notification.close();
    }

    const notification = event.notification;
    const primaryKey = notification.data.primaryKey;
    const action = event.action;
    if (action === 'close') {
        notification.close();
    } else {
        clients.openWindow(SOME_URL);
        notification.close();
    }

    // Hiding Notifications on Page Focus
    // do your notification magic
    // close all notifications
    const notifications = await self.registration.getNotifications();
    notifications.forEach(notification => notification.close());

    // close all notifications with tag of 'SinjulMSBH'
    const options = { tag: 'SinjulMSBH' };
    const notificationsTag = await self.registration.getNotifications(options);
    notificationsTag.forEach(notification => notification.close());
});

self.addEventListener('notificationclose', event => {
    const notification = event.notification;
    const primaryKey = notification.data.primaryKey;
    console.log('Closed notification: ' + primaryKey);
});