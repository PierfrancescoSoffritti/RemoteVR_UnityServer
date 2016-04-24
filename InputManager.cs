using System;
using System.Threading;
using UnityEngine;

class InputManager
{
    private ServerConnection serverConnection;

    // target
    private GameObject mTarget;
    private Quaternion targetInitialRotation;

    // gyro
    private float[] gyroQuaternion;
    private float[] gyroInitialRotation = null;

    public InputManager(ServerConnection socket, GameObject target)
    {
        init(socket, target);
    }

    public void init(ServerConnection socket, GameObject target)
    {
        serverConnection = socket;

        mTarget = target;
        targetInitialRotation = target.transform.rotation;

        Thread t = new Thread(new ThreadStart(readQuaternion));
        t.Start();
    }

    void readQuaternion()
    {
        while ((gyroQuaternion = serverConnection.readQuaternion()) != null)
        {
            if (gyroInitialRotation == null)
                gyroInitialRotation = gyroQuaternion;
        }
    }

    public void updateTarget()
    {
        if(gyroInitialRotation != null)
        { 
            Quaternion offsetRotation = 
                Quaternion.Inverse(new Quaternion(gyroInitialRotation[0], gyroInitialRotation[1], gyroInitialRotation[2], gyroInitialRotation[3])) 
                * new Quaternion(gyroQuaternion[0], gyroQuaternion[1], gyroQuaternion[2], gyroQuaternion[3]);

            mTarget.transform.rotation = targetInitialRotation * offsetRotation;
        }
    }
}