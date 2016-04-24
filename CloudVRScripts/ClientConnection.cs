using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Helper class that represents a connection to the client.
/// </summary>

public class ClientConnection
{
    private Socket clientSocket;

    private bool connected;

    private NetworkStream stream;
    private BinaryWriter writer;
    private BinaryReader reader;

    public ClientConnection(Socket socket)
    {
        clientSocket = socket;
        init();
    }

    private void init()
    {
        try
        {
            stream = new NetworkStream(clientSocket);
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);

            connected = true;

            Debug.Log("ClientConnection to: " + clientSocket.RemoteEndPoint.ToString() + " ready ");
        }
        catch (Exception)
        {
            connected = false;
            Debug.Log("can't create streams!");
        }
    }

    public void sendData(byte[] data)
    {
        //Debug.Log("is connected and i'm sending a byte[]");

        try
        {
            writer.Write(data);
        }
        catch (Exception)
        {
            //Debug.Log("Connection Was Closed! while sending byte[]");
            close();
        }
    }

    public void sendData(int data)
    {
       //Debug.Log("is connected and i'm sending a int");

        try
        {
            writer.Write(IPAddress.HostToNetworkOrder(data));

            //Debug.Log("sent int");

        } catch (Exception)
        {
            //Debug.Log("Connection Was Closed! while sending int");
            close();
        }
    }

    public float[] readQuaternion()
    {
        //Debug.Log("is connected and i'm reading quaternion");

        try
        {
            byte[] quaternion = reader.ReadBytes(4*4);

            byte[] temp = new byte[4];

            Array.Copy(quaternion, 0, temp, 0, 4);
            float x = NetworkToHostOrder(temp);

            Array.Copy(quaternion, 4, temp, 0, 4);
            float y = NetworkToHostOrder(temp);

            Array.Copy(quaternion, 8, temp, 0, 4);
            float z = NetworkToHostOrder(temp);

            Array.Copy(quaternion, 12, temp, 0, 4);
            float w = NetworkToHostOrder(temp);

            return new float[] { -y, x, -z, w };

        }
        catch (Exception)
        {
            //Debug.Log("Connection Was Closed! while reading quaternion");
            close();
            return null;
        }
    }

    private float NetworkToHostOrder(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToSingle(bytes, 0);
    }

    public void close()
    {
        if (connected)
        {
            connected = false;

            stream.Close();
            clientSocket.Close();

            Debug.Log("Closed connection with client");
        }
    }

    public bool Connected
    {
        get
        {
            return connected;
        }
    }
}