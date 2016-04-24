using UnityEngine;

class Player
{
    private ServerConnection serverConnection;

    private GameObject VRCamera;
    private VRCamera VRCameraLogic;

    private InputManager inputManager;

    public Player(string serverIp, int serverPort)
    {
        // create and init server connection
        serverConnection = new ServerConnection(serverIp, serverPort);
        serverConnection.sendData(0);

        // instantiate a VRCamera from prefab
        VRCamera = (GameObject) UnityEngine.Object.Instantiate(Resources.Load("VRCamera"));
        VRCameraLogic = VRCamera.GetComponent<VRCamera>();

        // init input manager
        inputManager = new InputManager(serverConnection, VRCamera);
    }

    internal void Update()
    {
        sendFrame(VRCameraLogic.GetImage());
        inputManager.updateTarget();
    }

    private void sendFrame(byte[] bytes)
    {
        int lenghtBytes = bytes.Length;

        serverConnection.sendData(lenghtBytes);
        serverConnection.sendData(bytes);
    }

    internal void Finish()
    {
        serverConnection.close();
    }
}
