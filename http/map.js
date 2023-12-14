let canvas;
let canvasContext;
let webSocket;
let roomImages = {};
const roomImageURLs = {
    Lcz012: "Lcz012.svg",
    Lcz173: "Lcz173.svg",
    Lcz914: "Lcz914.svg",
    LczGlassBox: "LczGlassBox.svg",
    LczArmory: "LczArmory.svg",
    LczCafe: "LczCafe.svg",
    LczClassDSpawn: "LczClassDSpawn.svg",
    LczChkpB: "LczChkpB.svg",
    LczChkpA: "LczChkpA.svg",
    LczStraight: "Straight.svg",
    LczToilets: "LczToilets.svg",
    LczPlants: "LczPlants.svg",
    LczAirlock: "LczAirlock.svg",
    LczCurve: "Curve.svg",
    LczTCross: "TCross.svg",
    LczCrossing: "Crossing.svg",
    Hcz049: "Hcz049.svg",
    Hcz079: "Hcz079.svg",
    Hcz096: "Hcz096.svg",
    Hcz106: "Hcz106.svg",
    Hcz939: "Hcz939.svg",
    HczChkpA: "HczChkpA.svg",
    HczChkpB: "HczChkpB.svg",
    HczEzCheckpoint: "HczEzCheckpoint.svg",
    HczHid: "HczHid.svg",
    HczNuke: "HczNuke.svg",
    HczTesla: "HczTesla.svg",
    HczServers: "HczServers.svg",
    HczTCross: "TCross.svg",
    HczArmory: "HczArmory.svg",
    HczCrossing: "Crossing.svg",
    HczCurve: "Curve.svg",
    HczStraight: "Straight.svg",
    EzGateA: "EzGateA.svg",
    EzGateB: "EzGateB.svg",
    EzCollapsedTunnel: "EzCollapsedTunnel.svg",
    EzShelter: "EzShelter.svg",
    EzVent: "EzVent.svg",
    EzStraight: "Straight.svg",
    EzIntercom: "EzIntercom.svg",
    EzDownstairsPcs: "EzDownstairsPcs.svg",
    EzUpstairsPcs: "EzUpstairsPcs.svg",
    EzPcs: "EzPcs.svg",
    EzConference: "EzConference.svg",
    EzCafeteria: "Straight.svg",
    EzCurve: "Curve.svg",
    EzCrossing: "Crossing.svg",
    Pocket: "Unknown.svg",
    Surface: "Unknown.svg",
    Unknown: "Unknown.svg"
}
const roomTranslationOverrides = {
    LczClassDSpawn: {x: 10.5, y: 0},
    Lcz012: {x: -10.5, y: 0},
    LczChkpB: {x: -10.5, y: 0},
    LczChkpA: {x: -10.5, y: 0},
    Hcz079: {x: -11.5, y: -11.5},
    Hcz106: {x: -11.5, y: -21.5},
    EzGateA: {x: 0, y: -10.5},
    EzGateB: {x: -10.5, y: 0}
};
let debugImage;
const debugImageURL = "Debug.svg";
const scale = 0.11;
const margin = 0.15;
let data = {};
let currentZones = ["LightContainment"];
let debugRoomRotation = false;

function windowResized() {
    canvas.width = document.body.clientWidth;
    canvas.height = document.body.clientHeight;
    if (Object.keys(data).length > 0) {
        drawRooms();
    }
}

function loadImage(url) {
    return new Promise((resolve, reject) => {
        let imageObj = new Image();
        imageObj.onload = () => resolve(imageObj);
        imageObj.onerror = ev => reject(ev);
        imageObj.src = url;
    });
}

async function recievedData(event) {
    // Remove trailing semicolon and parse JSON.
    console.info("Drawing rooms.")
    data = JSON.parse((await event.data.text()).slice(0, -1));
    drawRooms();
}

function drawImage(ctx, image, matrix) {
    ctx.setTransform(matrix);
    ctx.drawImage(image, -image.width / 2, -image.height / 2);
}

function drawRooms() {
    // Reset canvas
    canvasContext.clearRect(0, 0, canvas.width, canvas.height);
    canvasContext.save();

    // Calculate scaling

    // Invert Z coordinate for Y coordinate (correction A) and rotate object 180 degrees (correction B) when drawing because Unity has
    // a right-handed coordinate system and the canvas has a left-handed system.
    // Unity HTML5 Canvas
    // ===== ============
    // Z     +--X
    // |     |
    // Y--X  Y

    let rooms = data.rooms.filter(room => currentZones.includes(room.zone));

    // Calculate minimum and maxamum room positions
    let minX = rooms.reduce((accumulator, room) => Math.min(accumulator, room.posx), Infinity);
    let maxX = rooms.reduce((accumulator, room) => Math.max(accumulator, room.posx), -Infinity);
    let minY = rooms.reduce((accumulator, room) => Math.min(accumulator, room.posz), Infinity);
    let maxY = rooms.reduce((accumulator, room) => Math.max(accumulator, room.posz), -Infinity);

    // Calculate scaling and offset
    let scaleX = (canvas.width * (1 - margin * 2)) / (maxX - minX);
    let scaleY = (canvas.height * (1 - margin * 2)) / (maxY - minY);
    let deltaX = minX;
    let deltaY = -maxY; // correction A
    
    // Square scaling
    scaleX = Math.min(scaleX, scaleY);
    scaleY = scaleX;

    for (room of rooms) {
        let x = room.posx;
        let y = -room.posz; // correction A
        //let angle = (Math.PI / 180) * (room.roty + 180); // apply correction B and convert degrees to radians
        let angle = room.roty + 180;

        /*/ Apply room translation overrides (for oversized rooms)
        if (roomTranslationOverrides[room.type]) {
            x += Math.sin(angle) * roomTranslationOverrides[room.type].x;
            y += Math.cos(angle) * roomTranslationOverrides[room.type].y;
        }*/

        // Calculate transformation matrix
        /*let matrix = new DOMMatrix([
            Math.cos(angle) * scaleX * scale,
            Math.sin(angle) * scaleY * scale,
            -Math.sin(angle) * scaleX * scale,
            Math.cos(angle) * scaleY * scale,
            (x - deltaX) * scaleX + canvas.width * margin,
            (y - deltaY) * scaleY + canvas.height * margin
        ]);*/
        let matrix = new DOMMatrix();
        matrix.translateSelf(
            (x - deltaX) * scaleX + canvas.width * margin,
            (y - deltaY) * scaleY + canvas.height * margin
        );
        matrix.rotateSelf(angle);
        if (roomTranslationOverrides[room.type]) {
            matrix.translateSelf(
                roomTranslationOverrides[room.type].x * scaleX,
                roomTranslationOverrides[room.type].y * scaleX
            );
        }
        matrix.scaleSelf(scaleX * scale, scaleY * scale);
        
        if (roomImages.hasOwnProperty(room.type)) {
            // Render room type image
            console.debug(`Drawing ${debugRoomRotation ? debugImageURL : roomImageURLs[room.type]} at (${(deltaX + x) * scaleX}, ${(deltaY + y) * scaleY}) for ${room.type}.`)
            drawImage(canvasContext, debugRoomRotation ? debugImage : roomImages[room.type], matrix);
        } else {
            // Render missing texture
            console.warn(`Drawing ${debugRoomRotation ? debugImageURL : roomImageURLs["Unknown"]} at (${(deltaX + x) * scaleX}, ${(deltaY + y) * scaleY}) for ${room.type}.`)
            drawImage(canvasContext, debugRoomRotation ? debugImage : roomImages["Unknown"], matrix);
        }
    }
    canvasContext.restore();
}

function takeAction(action) {
    let update = true;
    switch (action) {
        case "1":
            currentZones = ["LightContainment"];
            break;
        case "2":
            currentZones = ["HeavyContainment", "Entrance"];
            break;
        case "3":
            currentZones = ["Surface"];
            break;
        case "d":
            debugRoomRotation = !debugRoomRotation;
            break;
        default:
            update = false;
    }
    if (update) {
        drawRooms();
    }
}

async function setup() {
    // Setup canvas
    canvas = document.getElementById("canvas");
    canvasContext = canvas.getContext('2d');
    windowResized();

    // Load images
    console.log("Loading images.");

    let imageLoaders = [loadImage(debugImageURL)];
    let roomNames = ["DEBUG"];
    for (roomName in roomImageURLs) {
        imageLoaders.push(loadImage(roomImageURLs[roomName]));
        roomNames.push(roomName);
    }
    let images = await Promise.all(imageLoaders);
    debugImage = images[0];
    for (let i = 1; i < images.length; i++) {
        roomImages[roomNames[i]] = images[i];
    }

    console.log("Images loaded.");

    // Connect to mapping server
    console.log("Connecting to wss://westley.digital:8767.");
    webSocket = new WebSocket("wss://westley.digital:8767");
    webSocket.onopen = () => {console.log("Connected to wss://westley.digital:8767.");};
    webSocket.onmessage = recievedData;

    // Connect events
    document.body.addEventListener("keydown", ev => takeAction(ev.key));
    addEventListener("resize", windowResized);
}

addEventListener("load", setup);