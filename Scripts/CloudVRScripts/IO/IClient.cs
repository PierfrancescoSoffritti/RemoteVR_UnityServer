/// <summary>
/// This interface represents the contract with the client.
/// Should be implemented by each class that wants to communicate with the client.
/// </summary>

public interface IClient
{
    int[] readScreenResolution();
    void sendImage(byte[] data);
    Input readInput();
    void disconnect();
}
