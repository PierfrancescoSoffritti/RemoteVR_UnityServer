using System.Collections.Generic;
using UnityEngine;

public class CloudVR : MonoBehaviour
{
    public bool useTCP = true;

    private IServer server;
    private List<Player> players;

    void Start ()
    {
        var initDispatcher = UnityThreadHelper.Dispatcher;

        players = new List<Player>();

        if (useTCP)
            server = new ServerTCP();
        else
            server = new ServerUDP();

        server.ClientConnected += OnClientConnected;
    }

    void Update ()
    {
        players.ForEach(player => 
        {
            try {
                player.Update();
            } catch
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

    /// <summary>
    /// Callback called from Server when a new client is connected.
    /// </summary>
    void OnClientConnected(object sender, OnClientConnectedEventArgs args) 
    {
        players.Add(new Player(args.ClientConnection));
    }
}