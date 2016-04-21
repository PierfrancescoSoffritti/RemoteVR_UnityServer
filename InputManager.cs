using System.Threading;
using UnityEngine;


class InputManager
{

    private static InputManager instance = new InputManager();

    SocketTest mSocket;
    float[] quaternion;
    MutablePose3D headPose;

    public static InputManager getInstance()
    {
        return instance;
    }

    public InputManager init(SocketTest socket)
    {
        this.mSocket = socket;

        headPose = new MutablePose3D();

        Thread t = new Thread(new ThreadStart(readQuaternion));
        t.Start();

        return instance;
    }

    void readQuaternion()
    {
        while (true)
        {
            quaternion = mSocket.readQuaternion();
            //headPose.Set(Vector3.zero, new Quaternion(quaternion[0], quaternion[1], quaternion[2], quaternion[3]));
        }
    }

    public Pose3D getHeadPose()
    {
        return headPose;
    }

    public Quaternion getQuaternion()
    {
        return new Quaternion(quaternion[0], quaternion[1], quaternion[2], quaternion[3]);
    }

}