# RemoteVR_UnityServer
Unity server for the RemoteVR project.

This server can communicate with the RemoteVR Client. At the moment the only implementation of the client is an Android app that can be found here [RemoteVR_AndroidClient](https://github.com/PierfrancescoSoffritti/RemoteVR_AndroidClient).

<img src="https://github.com/PierfrancescoSoffritti/RemoteVR_UnityServer/blob/master/pics/gyroscope.gif" />

This project is a collection of scripts, prefabs and a 3D model. All that is contained in the provided RemoteVR.unitypackage file. Just open it and you're good to go. Also, the package contains a sample scene, open it, press play, connect the client and it's done.

## What is the RemoteVR project
The RemoteVR project is a simple attempt to platform/hardware independent multiplayer Virtual Reality.<br/>
The concept is: run the VR application in the server, use the client to connect to the server, interact with the VR application in streaming.
With this approach is easy to have more than one person in the same virtual space. Also, those persons can use different devices and OS to access the same VR experience.

## Try a demo
[Click here](notuploadedYET) to download a simple ready-to-play demo. The zip file contains both the server's .exe and client's .apk. Read the readme in the zip to know how to use the demo.

## Communication  between server and client
Server and client communicate with this simple protocol:

<img height="350" src="https://github.com/PierfrancescoSoffritti/RemoteVR_UnityServer/blob/master/pics/protocol.png" />

The second message "send screen resolution" could be extended to "send client info" and contain more info like the client's name, location ecc.<br/><br/>
Each rendered frame is sent as JPG.

## Problems
- A lot
- ...
- It's super heavy to perform high quality rendering. The resolution of the rendering is the same resolution of the client's screen. The process of converting each rendered frame to jpg and then send it to the client it's heavy (take a look at the profiler) and causes a sensible drop in the frame rate.
