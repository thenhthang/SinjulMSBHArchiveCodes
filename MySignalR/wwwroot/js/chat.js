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

connection.on("ReceiveFileData", (path, fileName) => {
    const res = `Path: ${path} and FileName: ${fileName}`;
    console.log(res)
    let messageFileList = document.getElementById("messageFileList");
    const sp = document.createElement("div");
    sp.textContent = fileName;
    const a = document.createElement('a');
    const link = document.createTextNode("Download File");
    a.appendChild(link);
    a.download = "SinjulMSBH";
    a.title = "Download File";
    a.href = `Uploads/${fileName}`;
    a.classList.add("d-block");
    a.classList.add("mb-2");
    messageFileList.appendChild(sp);
    messageFileList.appendChild(a);
});

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
        await connection.invoke("SendMessage", user, message);
        //await connection.invoke("SendMessage2", message);
    } catch (err) {
        return console.error(err.toString());
    }
    event.preventDefault();
});

document.getElementById('submit').addEventListener("click", async event => {
    event.preventDefault();
    const loading = document.getElementById("loading");
    loading.innerText = "Please wait .. !!!!";
    let formData = new FormData(document.forms[0]);
    try {
        let response = await fetch('https://localhost:44387/', {
            method: 'POST',
            body: formData
        });
        let data = await response.json();
        console.log('Success:', data);
        loading.innerText = "";
        await connection.invoke("SendFileData", data.path, data.fileName);
    } catch (err) {
        console.error('Error:', err);
    }
});