using System;
using System.Net;

class IOUtils
{
    internal static Input handleInput(byte[] input)
    {
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

    private static TouchInput readTouch(byte[] input)
    {
        int type = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(input, 1));

        TouchInput touchInput = new TouchInput();
        switch (type)
        {
            case (int)TouchInput.TouchTypes.Down:
                touchInput.Data = TouchInput.TouchTypes.Down;
                break;
            case (int)TouchInput.TouchTypes.Up:
                touchInput.Data = TouchInput.TouchTypes.Up;
                break;
            default:
                throw new ArgumentException("unknown touch type");

        }
        return touchInput;
    }

    /// <summary>
    /// Convert a byte array representing a quaternion into a <see cref="GyroInput"/>.
    /// The order of the quaternion numbers is changed, because the Android's orientation is not compatible with Unity.
    /// </summary>
    private static GyroInput readQuaternion(byte[] quaternion)
    {
        byte[] temp = new byte[4];

        Array.Copy(quaternion, 1, temp, 0, 4);
        float x = NetworkToHostOrderFloat(temp);

        Array.Copy(quaternion, 5, temp, 0, 4);
        float y = NetworkToHostOrderFloat(temp);

        Array.Copy(quaternion, 9, temp, 0, 4);
        float z = NetworkToHostOrderFloat(temp);

        Array.Copy(quaternion, 13, temp, 0, 4);
        float w = NetworkToHostOrderFloat(temp);

        GyroInput input = new GyroInput();
        input.Data = new float[] { -y, x, -z, w };
        return input;
    }

    private static float NetworkToHostOrderFloat(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToSingle(bytes, 0);
    }

    public static int NetworkToHostOrderInt(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToInt32(bytes, 0);
    }
}
