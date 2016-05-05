using System.Threading;
using UnityEngine;

public class InputManager
{
    private ClientConnection clientConnection;

    // target
    private GameObject mTarget;
    private Quaternion targetInitialRotation;

    // gyro
    private float[] gyroQuaternion;
    private float[] gyroInitialRotation = null;

    // touch
    private bool move = false;

    public InputManager(ClientConnection socket, GameObject target)
    {
        init(socket, target);
    }

    public void init(ClientConnection socket, GameObject target)
    {
        clientConnection = socket;

        mTarget = target;
        targetInitialRotation = target.transform.rotation;

        Thread t = new Thread(new ThreadStart(read));
        t.Start();
    }

    // used by worker thread
    void read()
    {
        Input input;

        while(clientConnection.Connected && (input = clientConnection.readInput()) != null)
        {
            if (input is GyroInput)
                readQuaternion((GyroInput) input);
            else if (input is TouchInput)
                readTouchInput((TouchInput) input);
        }
    }

    private void readTouchInput(TouchInput input)
    {
        switch (input.Data)
        {
            case TouchInput.TouchTypes.Down:
                move = true;
                break;
            case TouchInput.TouchTypes.Up:
                move = false;
                break;
        }
    }

    private void readQuaternion(GyroInput input)
    {
        gyroQuaternion = input.Data;

        if (gyroInitialRotation == null)
            gyroInitialRotation = gyroQuaternion;
    }

    public void updateTarget()
    {
        if (gyroInitialRotation == null || !clientConnection.Connected)
            return;

        updateRotation();
    }

    private void updateRotation()
    {
        Quaternion offsetRotation =
                Quaternion.Inverse(new Quaternion(gyroInitialRotation[0], gyroInitialRotation[1], gyroInitialRotation[2], gyroInitialRotation[3]))
                * new Quaternion(gyroQuaternion[0], gyroQuaternion[1], gyroQuaternion[2], gyroQuaternion[3]);

        mTarget.transform.rotation = targetInitialRotation * offsetRotation;
    }

    public bool Move
    {
        get
        {
            return move;
        }
    }
}