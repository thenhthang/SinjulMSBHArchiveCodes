"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    //.withUrl("/chathub", {
    //    transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling,
    //    skipNegotiation: true,
    //    accessTokenFactory: () => {
    //        // Get and return the access token.
    //        // This function can return a JavaScript Promise if asynchronous
    //        // logic is required to retrieve the access token.
    //    }
    //})
    .configureLogging(signalR.LogLevel.Information)
    // .configureLogging("warn") // trace , debug , info or information	, warn or warning , error, criticalm none
    .withAutomaticReconnect()
    //.withAutomaticReconnect([0, 0, 10000])
    // .withAutomaticReconnect([0, 2000, 10000, 30000]) yields the default behavior
    //.withAutomaticReconnect({
    //    nextRetryDelayInMilliseconds: retryContext => {
    //        if (retryContext.elapsedMilliseconds < 60000) {
    //            // If we've been reconnecting for less than 60 seconds so far,
    //            // wait between 0 and 10 seconds before the next reconnect attempt.
    //            return Math.random() * 10000;
    //        } else {
    //            // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
    //            return null;
    //        }
    //    }
    //})
    .build()
    ;

connection.onreconnecting(error => {
    console.assert(connection.state === signalR.HubConnectionState.Reconnecting);

    document.getElementById("messageInput").disabled = true;

    const li = document.createElement("li");
    li.textContent = `Connection lost due to error "${error}". Reconnecting.`;
    document.getElementById("messagesList").appendChild(li);
});

connection.onreconnected(connectionId => {
    console.assert(connection.state === signalR.HubConnectionState.Connected);

    document.getElementById("messageInput").disabled = false;

    const li = document.createElement("li");
    li.textContent = `Connection reestablished. Connected with connectionId "${connectionId}".`;
    document.getElementById("messagesList").appendChild(li);
});

connection.onclose(async error => {
    document.getElementById("messageInput").disabled = true;
    const li = document.createElement("li");
    li.textContent = `Connection closed due to error "${error}". Try refreshing this page to restart the connection.`;
    document.getElementById("messagesList").appendChild(li);

    //console.assert(connection.state === signalR.HubConnectionState.Disconnected);
    //await start();
    //document.getElementById("messageInput").disabled = false;
});


document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", (user, message) => {
    const msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const encodedMsg = user + " says " + msg;
    const li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("ReceiveMessage2", req => {
    if (req.sender) {
        const msg = req.message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        const encodedMsg = req.sender + " says " + msg;
        const li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    }
});

connection.on("UserState", req =>
    console.log(`UserId: ${req.userId} and ConnectionId: ${req.connectionId} and Status: ${req.status}`)
);

const start = async () => {
    try {
        await connection.start();
        console.log("connected");
        console.assert(connection.state === signalR.HubConnectionState.Connected);
        document.getElementById("sendButton").disabled = false;
    } catch (err) {
        console.assert(connection.state === signalR.HubConnectionState.Disconnected);
        console.log(err);
        setTimeout(() => start(), 4000);
    }
};

const stop = async () => {
    try {
        document.getElementById("sendButton").disabled = true;
        console.assert(connection.state === signalR.HubConnectionState.Disconnected);
        console.log("stop");
        await connection.stop();
    } catch (err) {
        console.assert(connection.state === signalR.HubConnectionState.Connecting);
        console.log(err);
        setTimeout(() => start(), 4000);
    }
};

start();

document.getElementById("stopme").addEventListener("click", stop);

document.getElementById("sendButton").addEventListener("click", async event => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    try {
        //await connection.invoke("SendMessage", user, message);
        await connection.invoke("SendMessage2", message);
    } catch (err) {
        return console.error(err.toString());
    }
    event.preventDefault();
});


//const uploadFile = () => {
//    const myFile = document.getElementById("myFile");
//    let formData = new FormData();
//    if (myFile.files.length > 0) {
//        for (var i = 0; i < myFile.files.length; i++) {
//            console.log(myFile.files[i]);
//            formData.append('file-' + i, myFile.files[i]);
//        }
//    }

//    var fd = new FormData();
//    var files = myFile[0].files[0];
//    fd.append('file', files);

//    fetch('https://localhost:44387/Upload', {
//        method: 'POST',
//        //headers: { 'Content-Type': 'application/json' },
//        body: formData
//    })
//        .then((response) => response.json())
//        .then((body) => {
//            console.log('Success:', body);
//        })
//        .catch((error) => {
//            console.error('Error:', error);
//        })
//        ;
//};








//const uploadFile = async () => {
//    const myFile = document.getElementById("myFile");
//    let formData = new FormData();
//    if (myFile.files.length > 0) {
//        for (var i = 0; i < myFile.files.length; i++) {
//            formData.append('file-' + i, myFile.files[i]);
//        }
//    }
//    try {
//        const fetch = await fetch('https://localhost:44387/Upload', {
//            method: 'POST',
//            headers: { 'Content-Type': 'application/json' },
//            body: formData
//        });
//        console.info(fetch);
//        //const body = fetch.json();
//        //console.log('Success:', body);
//    } catch (err) {
//        console.error('Error:', err);
//    }
//};



document.getElementById('submit').addEventListener("click", (evt) => {
    evt.preventDefault();
    let data = new FormData(document.forms[0]);
    fetch('https://localhost:44387/', {
        method: 'post',
        body: data
    })
        .then(() => {
            alert('Posted using Fetch');
        });
});