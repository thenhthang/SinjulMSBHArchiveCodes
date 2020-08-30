
self.addEventListener('push', event => {
    if (!(self.Notification
        && self.Notification.permission === 'granted'))
        return;

    // Local Notification
    self.registration.showNotification(
        "SinjulMSBH",
        { body: "Hello from SinjulMSBH .. !!!!" }
    );

    let data = {};
    if (event.data) data = event.data.json();

    console.log('Notification Received:');
    console.log(data);

    const title = data.title;
    const message = data.message;
    const icon = "SinjulMSBH.jpg";
    const image = "SinjulMSBH.jpg";

    console.log("waitUntil start");
    event.waitUntil(self.registration.showNotification(title, {
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
        link: "http://pushnotification.jackslater.ir",
        timestamp: Math.floor(Date.now()),
    }));
    console.log("waitUntil end");
});

self.addEventListener('notificationclick', event => event.notification.close());