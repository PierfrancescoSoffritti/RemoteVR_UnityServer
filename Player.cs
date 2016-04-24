using UnityEngine;

class Player
{
    private ClientConnection clientConnection;

    private GameObject VRCamera;
    private VRCamera VRCameraLogic;

    private InputManager inputManager;

    public Player(ClientConnection connection)
    {
        // create and init server connection
        clientConnection = connection;

        // instantiate a VRCamera from prefab
        VRCamera = (GameObject) Object.Instantiate(Resources.Load("VRCamera"));
        VRCameraLogic = VRCamera.GetComponent<VRCamera>();

        // init input manager
        inputManager = new InputManager(clientConnection, VRCamera);
    }

    internal void Update()
    {
        sendFrame(VRCameraLogic.GetImage());
        inputManager.updateTarget();
    }

    private void sendFrame(byte[] bytes)
    {
        int lenghtBytes = bytes.Length;

        clientConnection.sendData(lenghtBytes);
        clientConnection.sendData(bytes);
    }

    internal void Finish()
    {
        VRCameraLogic.Destroy();
        clientConnection.close();
    }

    public ClientConnection ClientConnection
    {
        get
        {
            return clientConnection;
        }
    }
}
