using UnityEngine;

public class ServerIO : MonoBehaviour
{
    private Player player;

    void Start ()
    {
        player = new Player("localhost", 2099);
    }

    void Update ()
    {
        player.Update();
     }

    void OnApplicationQuit()
    {
        player.Finish();
    }
}