using System;

/// <summary>
/// Every game's input type must implement this interface.
/// </summary>
// should be done with generics and not be an empty interface .. but at the moment c# is driving me crazy :|
public interface Input
{
}

/// <summary>
/// Gyroscope input. The rotation is represented as a quaternion, stored in a float array.
/// </summary>
public class GyroInput : Input
{
    private float[] data;

    public float[] Data
    {
        get
        {
            return data;
        }

        set
        {
            if (value == null)
                throw new NullReferenceException("value == null");

            data = value;
        }
    }
}

/// <summary>
/// Touch input. Can be of two types: touch up or touch down.
/// </summary>
public class TouchInput : Input
{
    public enum TouchTypes : int { Down = 0, Up = 1};

    private TouchTypes data;

    public TouchTypes Data
    {
        get
        {
            return data;
        }

        set
        {
            data = value;
        }
    }
}