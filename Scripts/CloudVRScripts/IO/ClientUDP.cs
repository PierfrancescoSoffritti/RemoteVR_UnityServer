using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientUDP : IClient
{
    private readonly int DEFAULT_PORT = 2099;

    private IPEndPoint clientIPEndPoint;
    private UdpClient udpClient;

    public ClientUDP(IPEndPoint clientIPEndPoint)
    {
        this.clientIPEndPoint = clientIPEndPoint;

        udpClient = new UdpClient(DEFAULT_PORT);
        udpClient.Connect(clientIPEndPoint);
        udpClient.Client.ReceiveTimeout = 5000;
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
