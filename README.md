# BrendanSL
An in-browser mapping program for SCP: Secret Laboratory.

##Operation
1. SCP-SL server generates a map.
2. The EXILED plugin sends the position and rotation of each room to the python server.
3. The python server caches the data from the EXILED plugin and forwards it to each connected web browser on initial connection and whenever the data is updated.
4. The web browser draws a map using a custom SVG for each room.
