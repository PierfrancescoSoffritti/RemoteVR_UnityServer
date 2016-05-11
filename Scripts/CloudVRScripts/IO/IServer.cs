using System;
using System.Threading;
using UnityEngine;

class OnClientConnectedEventArgs : EventArgs
{
    public IClient ClientConnection { get; set; }
}

abstract class IServer
{
    public event EventHandler<OnClientConnectedEventArgs> ClientConnected;

    protected bool listen;

    // Thread signal.
    public static ManualResetEvent block = new ManualResetEvent(false);

    public IServer()
    {
        listen = true;
        new Thread(new ThreadStart(Start)).Start();
    }

    protected void NotifyNewClientConnected(IClient client)
    {
        // make a copy to be more thread-safe
        EventHandler<OnClientConnectedEventArgs> handler = ClientConnected;

        if (handler != null)
        {
            // dispatch event on the main thread
            UnityThreadHelper.Dispatcher.Dispatch(() =>
            {
                // invoke the subscribed event-handler(s)
                handler(this, new OnClientConnectedEventArgs() { ClientConnection = client });
            });
        }
    }

    /// <summary>
    /// Init socket and start listening. Is called in a separate thread
    /// </summary>
    protected abstract void Start();
    public abstract void Disconnect();
}
