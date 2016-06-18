using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class ServerUDP : IServer
{
    private static readonly int DEFAULT_PORT = 2099;

    private int clientPort = DEFAULT_PORT;

    private UdpClient udpClient;

    override
    protected void Start()
    {
        udpClient = new UdpClient(DEFAULT_PORT);
        IPEndPoint IPendPoint = new IPEndPoint(IPAddress.Any, DEFAULT_PORT);

        while (listen)
        {
            block.Reset();

            UdpState udpState = new UdpState();
            udpState.endPoint = IPendPoint;
            udpState.udpClient = udpClient;

            // Start an asynchronous socket to listen for connections.
            foreach (IPAddress hostName in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (!hostName.ToString().Contains(":"))
                    Debug.Log("Waiting for a connection on " + hostName + " : " + DEFAULT_PORT + "...");
            }

            udpClient.BeginReceive(new AsyncCallback(OnConnect), udpState);

            block.WaitOne();
        }
    }

    // callback for Async beginAccept
    private void OnConnect(IAsyncResult asyncResult)
    {
        UdpState udpState = (UdpState) asyncResult.AsyncState;

        UdpClient udpClient = udpState.udpClient;
        IPEndPoint endPoint = udpState.endPoint;

        byte[] message = udpClient.EndReceive(asyncResult, ref endPoint);
        Debug.Log("client connected");

        clientPort++;

        IClient client = new ClientUDP(endPoint, clientPort);
        
        NotifyNewClientConnected(client);

        // Signal the thread to continue.
        block.Set();
    }

    override
    public void Disconnect()
    {
        listen = false;
        udpClient.Close();
    }

    private class UdpState
    {
        internal IPEndPoint endPoint;
        internal UdpClient udpClient;
    }
}
