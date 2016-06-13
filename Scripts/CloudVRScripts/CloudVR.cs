using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main class of the project, attach this to any game object in your scene and you're ready to go.
/// </summary>

public class CloudVR : MonoBehaviour
{
    public bool useTCP = true;

    private IServer server;
    private List<Player> players = new List<Player>();

    void Awake ()
    {
        var initDispatcher = UnityThreadHelper.Dispatcher;

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
    /// Callback called from the Server when a new client is connected.
    /// Creates a new Player.
    /// </summary>
    void OnClientConnected(object sender, OnClientConnectedEventArgs args) 
    {
        players.Add(new Player(args.ClientConnection));
    }
}