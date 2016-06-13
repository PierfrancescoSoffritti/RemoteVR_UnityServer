using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// This class is a UDP endpoint for s with the client.
/// </summary>
public class ClientUDP : IClient
{
    private IPEndPoint clientIPEndPoint;
    private UdpClient udpClient;

    public ClientUDP(IPEndPoint clientIPEndPoint, int serverPort)
    {
        this.clientIPEndPoint = clientIPEndPoint;

        udpClient = new UdpClient(serverPort);
        udpClient.Connect(clientIPEndPoint);
        udpClient.Client.ReceiveTimeout = 5000;

        // tell the client that the session has started.
        // this is also usefull in order to tell the client on which port it has to communicate
        udpClient.Send(new byte[1], 1);
    }

    public Input readInput()
    {
        try {
            byte[] receiveBytes = udpClient.Receive(ref clientIPEndPoint);
            return IOUtils.handleInput(receiveBytes);
        } catch (Exception)
        {
            disconnect();
            throw new IOException("Client disconnected");
        }
    }

    public int[] readScreenResolution()
    {
        try
        {
            byte[] receiveBytes = udpClient.Receive(ref clientIPEndPoint);

            byte[] temp = new byte[4];
            int[] size = new int[2];

            Array.Copy(receiveBytes, 0, temp, 0, 4);
            size[0] = IOUtils.NetworkToHostOrderInt(temp);

            Array.Copy(receiveBytes, 4, temp, 0, 4);
            size[1] = IOUtils.NetworkToHostOrderInt(temp);

            udpClient.Send(receiveBytes, receiveBytes.Length);

            return size;
        }
        catch (Exception)
        {
            disconnect();
            throw new IOException("Client disconnected");
        }
    }

    public void sendImage(byte[] data)
    {
        try
        {
            byte[] packet = new byte[data.Length + 4];
            byte[] imgLength = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(data.Length));

            imgLength.CopyTo(packet, 0);
            data.CopyTo(packet, imgLength.Length);

            udpClient.Send(packet, packet.Length);
        }
        catch (Exception)
        {
            disconnect();
            throw new IOException("Client disconnected");
        }
    }

    public void disconnect()
    {
        udpClient.Close();
        Debug.Log("Closed connection with client");
    }
}
