using System.Collections.Generic;
using UnityEngine;

public class CloudVR : MonoBehaviour
{
    private Server server;

    private List<Player> players;

    void Start ()
    {
        var initDispatcher = UnityThreadHelper.Dispatcher;

        players = new List<Player>();

        server = new Server();
        server.ClientConnected += OnClientConnected;
    }

    void Update ()
    {
        players.ForEach(player => 
        {
            if (player.ClientConnection.Connected)
                player.Update();
            else
            {
                player.Finish();
                players.Remove(player);
            }
        });
     }

    void OnApplicationQuit()
    {
        server.Disconnect();

        players.ForEach(player => player.Finish());
    }

    void OnClientConnected(object sender, OnClientConnectedEventArgs args) 
    {
        players.Add(new Player(args.ClientConnection));
    }
}