using System;

// should be done with generics .. but c# is driving me crazy :|
public interface Input
{
}

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