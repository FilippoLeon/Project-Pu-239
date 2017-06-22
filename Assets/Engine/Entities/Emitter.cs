using System.Collections.Generic;

public class Emitter
{
    protected HashSet<IListener> listeners = new HashSet<IListener>();

    public void Connect(IListener listener)
    {
        listener.Emitter = this;
        listeners.Add(listener);
    }
    public void Disconnect(IListener listener)
    {
        listeners.Remove(listener);
    }

    protected void Emit(string signal, object[] args = null)
    {
        foreach(IListener listener in listeners)
        {
            listener.Event(signal, args);
        }
    }
}