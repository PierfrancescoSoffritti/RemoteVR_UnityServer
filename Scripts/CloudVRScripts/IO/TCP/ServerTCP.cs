using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class ServerTCP : IServer
{
    private readonly int DEFAULT_PORT = 2099;

    private Socket serverSocket;

    override
    protected void Start()
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, DEFAULT_PORT);
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            serverSocket.Bind(localEndPoint);
            serverSocket.Listen(100);

            while (listen)
            {
                block.Reset();

                // Start an asynchronous socket to listen for connections.
                foreach ( IPAddress hostName in Dns.GetHostEntry(Dns.GetHostName()).AddressList )
                {
                    if(!hostName.ToString().Contains(":"))
                        Debug.Log("Waiting for a connection on " + hostName + " : " + DEFAULT_PORT + "...");
                }

                serverSocket.BeginAccept(new AsyncCallback(OnConnect), serverSocket);

                block.WaitOne();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // callback for Async beginAccept
    private void OnConnect(IAsyncResult asyncResult)
    {
        // Get the socket that handles the client request.
        Socket serverSocket = (Socket) asyncResult.AsyncState;
        Socket clientSocket = serverSocket.EndAccept(asyncResult);
        Debug.Log("client connected");

        NotifyNewClientConnected(new ClientTCP(clientSocket));

        // Signal the thread to continue.
        block.Set();
    }

    override
    public void Disconnect()
    {
        listen = false;
        serverSocket.Close();
    }
}
