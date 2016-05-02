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

            UnityEngine.Debug.Log("ClientConnection to: " + clientSocket.RemoteEndPoint.ToString() + " ready ");
        }
        catch (Exception)
        {
            connected = false;
            UnityEngine.Debug.Log("can't create streams!");
        }
    }

    internal int[] readScreenResolution()
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
            close();
            return null;
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

    public Input readInput()
    {
        try
        {
            byte[] input = reader.ReadBytes(1+(4*4));

            switch (input[0])
            {
                case 0:
                    return readQuaternion(input);
                case 1:
                    return readTouch(input);
                default:
                    throw new ArgumentException("unknown input type");
            }
        }
        catch (Exception)
        {
            //Debug.Log("Connection Was Closed! while reading input");
            close();
            return null;
        }

    }

    private TouchInput readTouch(byte[] input)
    { 
        int type = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(input, 1));

        TouchInput touchInput = new TouchInput();
        switch (type)
        {
            case (int) TouchInput.TouchTypes.Down:
                touchInput.Data = TouchInput.TouchTypes.Down;
                break;
            case (int) TouchInput.TouchTypes.Up:
                touchInput.Data = TouchInput.TouchTypes.Up;
                break;
            default:
                throw new ArgumentException("unknown touch type");

        }
        return touchInput;
    }

    public GyroInput readQuaternion(byte[] quaternion)
    {
        byte[] temp = new byte[4];

        Array.Copy(quaternion, 1, temp, 0, 4);
        float x = NetworkToHostOrder(temp);

        Array.Copy(quaternion, 5, temp, 0, 4);
        float y = NetworkToHostOrder(temp);

        Array.Copy(quaternion, 9, temp, 0, 4);
        float z = NetworkToHostOrder(temp);

        Array.Copy(quaternion, 13, temp, 0, 4);
        float w = NetworkToHostOrder(temp);

        GyroInput input = new GyroInput();
        input.Data = new float[] { -y, x, -z, w };
        return input;
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

            UnityEngine.Debug.Log("Closed connection with client");
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