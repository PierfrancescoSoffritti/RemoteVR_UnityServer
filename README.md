# RemoteVR project
The RemoteVR project is a simple attempt to obtain platform-independent multiplayer Virtual Reality.<br/>
The gist of the idea is to run the VR application on a server and to use many clients to interact with it, in streaming.
With this approach is easy to have more than one person in the same virtual space. Also, those people can use different devices and OS to access the same VR experience.

# Unity Server
This is a server built into Unity 3D for the RemoteVR project.

This server can communicate with the RemoteVR Client. At the moment the only implementation of the client is an Android app that can be found here [RemoteVR_AndroidClient](https://github.com/PierfrancescoSoffritti/RemoteVR_AndroidClient).

<img src="https://github.com/PierfrancescoSoffritti/RemoteVR_UnityServer/blob/master/pics/gyroscope.gif" />
<img src="https://github.com/PierfrancescoSoffritti/RemoteVR_UnityServer/blob/master/pics/museum.gif" />

This project is a collection of scripts, prefabs and a 3D model. Everything is included in the RemoteVR.unitypackage file. Just open it and you're good to go. Also, the package contains a sample scene, open it, press play, connect the client and you're done.

## Try a demo
[Click here](https://drive.google.com/open?id=0B4BhGgWS02sBVmVRX0lTYzhKczA) to download some simple, ready-to-play demos. The zip files contains both the server's .exe and client's .apk.

### How to use
- Launch the .exe file, when prompted allow network access.
- Install the .apk client on an Android device.
- Set the IP and port number of your PC in the Android client.
- Set "use UDP" in the Android client. Because the provided demos are using UDP.
- Press play and use the app.

## Communication  between server and client
Server and client communicate with this simple protocol:

<img height="350" src="https://github.com/PierfrancescoSoffritti/RemoteVR_UnityServer/blob/master/pics/protocol.png" />

The message "send screen resolution" could be extended to something like "send client info" in order to contain more information, like the client's name, location ecc.<br/><br/>
Each rendered frame is sent as JPG.

## Problems
- A lot :)
- ...
- It's super heavy to perform high quality rendering. The resolution of the rendering is the same resolution of the client's screen. The process of converting each rendered frame to jpg and then send it to the client is heavy (take a look at the profiler) and causes a sensible drop in the frame rate.
