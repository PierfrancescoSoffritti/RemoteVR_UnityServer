using UnityEngine;

[RequireComponent(typeof(VRCamera))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerController))]
class Player
{
    private IClient clientConnection;

    private GameObject playerObject;
    private VRCamera VRCamera;

    private InputManager inputManager;
    private PlayerController playerController;

    public Player(IClient connection)
    {
        // create and init server connection
        clientConnection = connection;

        // instantiate a VRCamera from prefab
        playerObject = (GameObject) Object.Instantiate(Resources.Load("VRCharacter"));
        VRCamera = playerObject.GetComponent<VRCamera>();

        int[] screenResolution = connection.readScreenResolution();
        Debug.Log("Client screen resolution: " +screenResolution[0] +" x " + screenResolution[1]);
        VRCamera.textureWidth = screenResolution[0];
        VRCamera.textureHeight = screenResolution[1];

        // init input manager
        inputManager = new InputManager(clientConnection, playerObject);

        // player controller
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.inputManager = inputManager;
    }

    internal void Update()
    {
        sendFrame(VRCamera.GetImage());
        inputManager.updateTarget();
    }

    private void sendFrame(byte[] bytes)
    {
        clientConnection.sendImage(bytes);
    }

    internal void Finish()
    {
        VRCamera.Destroy();
        clientConnection.disconnect();
    }

    public IClient ClientConnection
    {
        get
        {
            return clientConnection;
        }
    }
}
