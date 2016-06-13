using System.Threading;
using UnityEngine;

/// <summary>
/// This class reads the input from the <see cref="IClient"/>
/// Is considered as the only access point to this data.
/// </summary>
public class RemoteInputManager
{
    // the source of remote input data
    private IClient clientConnection;

    // gyro
    private float[] gyroQuaternion;
    private float[] gyroInitialRotation = null;

    // touch
    private bool touchDown = false;

    public RemoteInputManager(IClient socket)
    {
        init(socket);
    }

    public void init(IClient socket)
    {
        clientConnection = socket;

        Thread t = new Thread(new ThreadStart(read));
        t.Start();
    }

    /// <summary>
    /// Reads the last remote input received from the client.
    /// This method is called periodically by the worker thread and runs until the client is disconnected.
    /// </summary>
    void read()
    {
        Input input;

        while((input = clientConnection.readInput()) != null)
        {
            if (input is GyroInput)
                handleQuaternion((GyroInput) input);
            else if (input is TouchInput)
                handleTouchInput((TouchInput) input);
        }
    }

    /// <summary>
    /// Handles the remote touch input.
    /// </summary>
    private void handleTouchInput(TouchInput input)
    {
        switch (input.Data)
        {
            case TouchInput.TouchTypes.Down:
                touchDown = true;
                break;
            case TouchInput.TouchTypes.Up:
                touchDown = false;
                break;
        }
    }

    /// <summary>
    /// Handles the remote rotation input.
    /// </summary>
    private void handleQuaternion(GyroInput input)
    {
        gyroQuaternion = input.Data;

        // save the initial rotation
        if (gyroInitialRotation == null)
            gyroInitialRotation = gyroQuaternion;
    }

    /// <summary>
    /// Returns the last rotation from the client. This value should be multiplied by the initial rotation of the target and the result used as the new rotation of the target.
    /// </summary>
    public Quaternion Rotation
    {
        get
        {
            Quaternion offsetRotation =
                Quaternion.Inverse(new Quaternion(gyroInitialRotation[0], gyroInitialRotation[1], gyroInitialRotation[2], gyroInitialRotation[3]))
                * new Quaternion(gyroQuaternion[0], gyroQuaternion[1], gyroQuaternion[2], gyroQuaternion[3]);

            return offsetRotation;
        }
    }

    /// <summary>
    /// Return true if the display of the client is being touched (touch down == touch and hold).
    /// </summary>
    public bool TouchDown
    {
        get
        {
            return touchDown;
        }
    }
}