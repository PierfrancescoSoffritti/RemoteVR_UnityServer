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
        // default settings
        //ip = "localhost";
        port = 2099;
        localEndPoint = new IPEndPoint(Dns.GetHostEntry("localhost").AddressList[0], port);
        // Create a TCP/IP socket.
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        connected = false;

        // this starts the connection
        try
        {
            socket.Connect(localEndPoint);
            connected = true; // only a temporary thing, very unsafe but its just for testing
            Debug.Log("Connected to: " + socket.RemoteEndPoint.ToString() + Environment.NewLine);

        } catch (Exception exception)
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
                stream = new NetworkStream(socket);
                writer = new BinaryWriter(stream);

                writer.Write(data);

                //for (int i=0; i<data.Length; i++)
                    //writer.Write(data[i]);

                //Debug.Log("sent byte[]");

            }
            catch (Exception exception)
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
                stream = new NetworkStream(socket);
                writer = new BinaryWriter(stream);
                writer.Write(IPAddress.HostToNetworkOrder(data));

                //Debug.Log("sent int");

            } catch (Exception ex)
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
            Debug.Log("is connected and i'm reading quaternion");

            try
            {
                stream = new NetworkStream(socket);
                reader = new BinaryReader(stream);

                float x = NetworkToHostOrder(reader.ReadSingle());
                float y = NetworkToHostOrder(reader.ReadSingle());
                float z = NetworkToHostOrder(reader.ReadSingle());
                float w = NetworkToHostOrder(reader.ReadSingle());

                return new float[] { x, y, 0, w };

            }
            catch (Exception ex)
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

    private float NetworkToHostOrder(float network)
    {
        byte[] bytes = BitConverter.GetBytes(network);

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