using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

class OnClientConnectedEventArgs : EventArgs
{
    public ClientConnection ClientConnection { get; set; }
}

class Server
{
    private readonly int DEFAULT_PORT = 2099;

    public event EventHandler<OnClientConnectedEventArgs> ClientConnected;

    private Socket serverSocket;
    private bool listen;

    // Thread signal.
    public static ManualResetEvent block = new ManualResetEvent(false);

    public Server()
    {
        listen = true;
        new Thread(new ThreadStart(Start)).Start();
    }

    private void Start()
    {
        int port = DEFAULT_PORT;

        IPEndPoint localEndPoint = new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], port);
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
                Debug.Log("Waiting for a connection...");
                serverSocket.BeginAccept(new AsyncCallback(OnConnect), serverSocket);

                block.WaitOne();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void OnConnect(IAsyncResult ar)
    {
        // Signal the thread to continue.
        block.Set();

        // Get the socket that handles the client request.
        Socket serverSocket = (Socket)ar.AsyncState;
        Socket clientSocket = serverSocket.EndAccept(ar);
        Debug.Log("client connected");

        NotifyClientConnected(clientSocket);
    }

    protected void NotifyClientConnected(Socket socket)
    {
        // make a copy to be more thread-safe
        EventHandler<OnClientConnectedEventArgs> handler = ClientConnected;

        if (handler != null)
        {
            UnityThreadHelper.Dispatcher.Dispatch(() =>
            {
                // invoke the subscribed event-handler(s)
                handler(this, new OnClientConnectedEventArgs() { ClientConnection = new ClientConnection(socket) });
            });
        }
    }

    public void Disconnect()
    {
        listen = false;
        serverSocket.Close();
    }
}
