using System.Threading;
using UnityEngine;

class InputManager
{
    private ClientConnection clientConnection;

    // target
    private GameObject mTarget;
    private Quaternion targetInitialRotation;

    // gyro
    private float[] gyroQuaternion;
    private float[] gyroInitialRotation = null;

    public InputManager(ClientConnection socket, GameObject target)
    {
        init(socket, target);
    }

    public void init(ClientConnection socket, GameObject target)
    {
        clientConnection = socket;

        mTarget = target;
        targetInitialRotation = target.transform.rotation;

        Thread t = new Thread(new ThreadStart(readQuaternion));
        t.Start();
    }

    void readQuaternion()
    {
        while ((gyroQuaternion = clientConnection.readQuaternion()) != null)
        {
            if (gyroInitialRotation == null)
                gyroInitialRotation = gyroQuaternion;
        }
    }

    public void updateTarget()
    {
        if(gyroInitialRotation != null && clientConnection.Connected)
        { 
            Quaternion offsetRotation = 
                Quaternion.Inverse(new Quaternion(gyroInitialRotation[0], gyroInitialRotation[1], gyroInitialRotation[2], gyroInitialRotation[3])) 
                * new Quaternion(gyroQuaternion[0], gyroQuaternion[1], gyroQuaternion[2], gyroQuaternion[3]);

            mTarget.transform.rotation = targetInitialRotation * offsetRotation;
        }
    }
}