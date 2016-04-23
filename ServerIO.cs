using UnityEngine;

public class ServerIO : MonoBehaviour
{
    private SocketTest socket;

    private InputManager inputManager;

    public VRCamera myCamera;
    public GameObject target;

    void Start ()
    {
        socket = new SocketTest();
        socket.sendData(0);

        inputManager = new InputManager(socket, target);
        myCamera.socket = socket;
    }

    void Update ()
    {
        inputManager.updateTarget();
     }

    void OnApplicationQuit()
    {
        socket.close();
    }
}