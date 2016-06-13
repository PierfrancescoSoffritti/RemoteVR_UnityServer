using UnityEngine;

/// <summary>
/// Class responsible for handling the output.
/// Sets the resolution of the <see cref="VRCamera"/> based on the client resolution. And sends the rendered image every time <see cref="Update"/> is called.
/// </summary>
class RemoteOutputManager
{
    private VRCamera vrCamera;
    private IClient client;

    public RemoteOutputManager(VRCamera vrCamera, IClient client)
    {
        this.vrCamera = vrCamera;
        this.client = client;

        // get client screen resolution
        int[] screenResolution = client.readScreenResolution();
        Debug.Log("Client screen resolution: " + screenResolution[0] + " x " + screenResolution[1]);
        // set vrCamera resolution
        vrCamera.textureWidth = screenResolution[0];
        vrCamera.textureHeight = screenResolution[1];
    }

    public void Update()
    {
        sendFrame(vrCamera.GetImage());
    }

    private void sendFrame(byte[] bytes)
    {
        client.sendImage(bytes);
    }

    internal void finish()
    {
        vrCamera.Destroy();
    }
}
