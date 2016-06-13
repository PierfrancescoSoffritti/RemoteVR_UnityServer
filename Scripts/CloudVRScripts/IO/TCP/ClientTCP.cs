using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// This class that represents a TCP connection with the client.
/// </summary>

public class ClientTCP : IClient
{
    private Socket clientSocket;

    private NetworkStream stream;
    private BinaryWriter writer;
    private BinaryReader reader;

    public ClientTCP(Socket socket)
    {
        clientSocket = socket;
        init(clientSocket);
    }

    private void init(Socket clientSocket)
    {
        try
        {
            stream = new NetworkStream(clientSocket);
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);

            UnityEngine.Debug.Log("ClientConnection to: " + clientSocket.RemoteEndPoint.ToString() + " ready ");
        }
        catch (Exception)
        {
            Debug.Log("can't create streams!");
            throw new IOException("can't create streams!");
        }
    }

    /// <summary>
    /// Read the screen resolution of the client.
    /// </summary>
    public int[] readScreenResolution()
    {
        try
        {
            int[] screenResolution = new int[2];
            screenResolution[0] = IPAddress.NetworkToHostOrder(reader.ReadInt32());
            screenResolution[1] = IPAddress.NetworkToHostOrder(reader.ReadInt32());

            return screenResolution;
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
            writer.Write(IPAddress.HostToNetworkOrder(data.Length));
            writer.Write(data);
        }
        catch (Exception)
        {
            disconnect();
            throw new IOException("Client disconnected");
        }
    }

    public Input readInput()
    {
        try
        {
            byte[] input = reader.ReadBytes(1 + (4 * 4));
            return IOUtils.handleInput(input);
        }
        catch (Exception)
        {
            disconnect();
            throw new IOException("Client disconnected");
        }
    }

    public void disconnect()
    {
        if (clientSocket.Connected)
        {
            clientSocket.Close();

            Debug.Log("Closed connection with client");
        }
    }
}