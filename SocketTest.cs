using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class SocketTest
{
    public byte[] buffer;
    public bool connected;
    public Socket socket;
    public int port;
    public string ip;
    public IPEndPoint localEndPoint;

    NetworkStream stream;
    BinaryWriter writer;
    BinaryReader reader;

    public SocketTest()
    {
        port = 2099;
        localEndPoint = new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], port);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        connected = false;

        try
        {
            socket.Connect(localEndPoint);

            stream = new NetworkStream(socket);
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);

            connected = true; // only a temporary thing, very unsafe but its just for testing
            Debug.Log("Connected to: " + socket.RemoteEndPoint.ToString() + Environment.NewLine);

        } catch (Exception)
        {

            Debug.Log("Couldn't Connect!");
            connected = false;
        }
    }

    public void sendData(byte[] data)
    { 
        if (connected)
        {
            //Debug.Log("is connected and i'm sending a byte[]");

            try
            {
                writer.Write(data);

            }
            catch (Exception)
            {
                Debug.Log("Connection Was Closed! while sending byte[]");
                connected = false;
                socket.Shutdown(SocketShutdown.Send);
                socket.Close();
            }
        }
        else
            Debug.Log("socket not connected. byte[]");
    }

    public void sendData(int data)
    {
        if (connected)
        {
            //Debug.Log("is connected and i'm sending a int");

            try
            {
                writer.Write(IPAddress.HostToNetworkOrder(data));

                //Debug.Log("sent int");

            } catch (Exception)
            {
                Debug.Log("Connection Was Closed! while sending int");
                connected = false;
                socket.Shutdown(SocketShutdown.Send);
                socket.Close();
            }
        }
        else
            Debug.Log("socket not connected. int");
    }

    public float[] readQuaternion()
    {
        if (connected)
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
                Debug.Log("Connection Was Closed! while reading quaternion");
                connected = false;
                socket.Shutdown(SocketShutdown.Receive);
                socket.Close();
                return null;
            }
        }
        else
        {
            Debug.Log("socket not connected. reading quaternion");
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
        socket.Shutdown(SocketShutdown.Receive);
        socket.Shutdown(SocketShutdown.Send);
        socket.Close();
    }
}