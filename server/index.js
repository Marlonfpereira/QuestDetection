const https = require("https");
const http = require("http");
const express = require("express");
const WebSocket = require("ws");
const fs = require("fs");

const app = express();

const options = {
    key: fs.readFileSync("sec/server.key"),
    cert: fs.readFileSync("sec/server.crt"),
};

app.use(express.static("public"));
app.use(express.json());

http.createServer(app).listen(80, () => {
    console.log("http on 80");
});
const server = https.createServer(options, app).listen(443, () => {
    console.log("https on 443");
});

var wsServer = new WebSocket.Server({ server });

function broadcast(data, ws) {
    wsServer.clients.forEach((client) => {
        if (client !== ws && client.readyState === WebSocket.OPEN) {
            client.send(data);
        }
    });
}

let predictions = [];
wsServer.on("connection", (ws) => {
    console.log("new socket connected");
    ws.on("message", (data) => {

        const objects = JSON.parse(data.toString());
        predictions = objects[0]

        broadcast(data, ws);
    });
    ws.on("close", () => {
        console.log("socket disconnected");
    });
});

app.get("/predictions", (req, res) => {
    console.log(predictions);
    res.send({ predictions });
});

