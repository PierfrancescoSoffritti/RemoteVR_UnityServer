using UnityEngine;

/// <summary>
/// This class represents a player. Is responsible for instantiating a prefab object and the game's IO.
/// </summary>
[RequireComponent(typeof(VRCamera))]
[RequireComponent(typeof(RemoteInputManager))]
[RequireComponent(typeof(PlayerController))]
class Player
{
    private IClient clientConnection;

    private PlayerController playerController;
    private RemoteOutputManager remoteOutputManager;

    public Player(IClient connection)
    {
        clientConnection = connection;

        // instantiate a the prefab
        GameObject playerObject = (GameObject) Object.Instantiate(Resources.Load("VRCharacter"));

        remoteOutputManager = new RemoteOutputManager(playerObject.GetComponent<VRCamera>(), clientConnection);

        // player controller
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.init(playerObject, new RemoteInputManager(clientConnection));
    }

    internal void Update()
    {
        remoteOutputManager.Update();
    }

    internal void Finish()
    {
        remoteOutputManager.finish();
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
