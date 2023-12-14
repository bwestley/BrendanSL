#!/usr/bin/python3
from time import sleep
import aiohttp
import asyncio
import json
import logging
import os
import re
import ssl
import subprocess
import sys
import websockets

WEBSOCKET_ADDRESS = "westley.digital"
WEBSOCKET_PORT = 8767
SOCKET_ADDRESS = "127.0.0.1"
SOCKET_PORT = 8777

logging.basicConfig(format="%(levelname)s:%(processName)s:%(threadName)s:%(name)s:%(filename)s:%(lineno)d:%(message)s", level=logging.DEBUG)

CLIENTS: set = set()

data: bytes = bytes()
mapDataRecieved = asyncio.Event()

async def socketServer(reader: asyncio.StreamReader, writer: asyncio.StreamWriter):
    global data
    address = writer.get_extra_info("peername")
    address = f"{address[0]}:{address[1]}"
    logging.info(f"{address} opened connection.")
    data = await reader.readuntil(b";")
    mapDataRecieved.set()
    mapDataRecieved.clear()
    logging.info(f"{address} data recieved.")
    logging.info(data.decode("ASCII"))
    logging.info(f"{address} closed connection.")

async def websocketServer(websocket: websockets.WebSocketServerProtocol, path: str):
    address = websocket.remote_address[0] + ":" + str(websocket.remote_address[1])
    logging.info(f"{address} opened connection.")

    CLIENTS.add(websocket)

    try:
        while websocket.open:
            if len(data) > 0:
                await websocket.send(data)
            await mapDataRecieved.wait()
    except:
        logging.exception(f"{address} exception occured.")
    CLIENTS.remove(websocket)

    logging.info(f"{address} closed connection.")

sslContext = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
sslContext.load_cert_chain(certfile="/home/scpserver/map/fullchain.pem", keyfile="/home/scpserver/map/privkey.pem")

logging.info(f"Starting websocket server on wss://{WEBSOCKET_ADDRESS}:{WEBSOCKET_PORT}.")
logging.info(f"Strting socket server on {SOCKET_ADDRESS}:{SOCKET_PORT}.")
asyncio.get_event_loop().run_until_complete(asyncio.gather(
    websockets.serve(websocketServer, WEBSOCKET_ADDRESS, WEBSOCKET_PORT, ssl=sslContext, ping_interval=None, ping_timeout=None),
    asyncio.start_server(socketServer, SOCKET_ADDRESS, SOCKET_PORT)
))
logging.info(f"Servers started.")
asyncio.get_event_loop().run_forever()