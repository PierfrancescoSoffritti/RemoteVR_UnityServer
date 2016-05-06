using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class ServerUDP : IServer
{
    private readonly int DEFAULT_PORT = 2098;

    private UdpClient udpClient;

    override
    protected void Start()
    {
        udpClient = new UdpClient(DEFAULT_PORT);
        IPEndPoint IPendPoint = new IPEndPoint(IPAddress.Any, DEFAULT_PORT);

        UdpState udpState = new UdpState();
        udpState.endPoint = IPendPoint;
        udpState.udpClient = udpClient;

        while (listen)
        {
            block.Reset();

            // Start an asynchronous socket to listen for connections.
            foreach (IPAddress hostName in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                Debug.Log("Waiting for a connection on " + hostName + " : " + DEFAULT_PORT + "...");
            }

            udpClient.BeginReceive(new AsyncCallback(OnConnect), udpState);

            block.WaitOne();
        }
    }

    // callback for Async beginAccept
    private void OnConnect(IAsyncResult asyncResult)
    {
        UdpClient udpClient = (UdpClient)((UdpState)(asyncResult.AsyncState)).udpClient;
        IPEndPoint endPoint = (IPEndPoint)((UdpState)(asyncResult.AsyncState)).endPoint;

        byte[] message = udpClient.EndReceive(asyncResult, ref endPoint);
        Debug.Log("client connected");

        udpClient.Send(message, message.Length, endPoint);
        NotifyNewClientConnected(new ClientUDP(new IPEndPoint(endPoint.Address, endPoint.Port)));

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
