using System.Collections.Generic;

public class Emitter
{
    protected HashSet<IEmitterListener> listeners = new HashSet<IEmitterListener>();

    public void Connect(IEmitterListener listener)
    {
        listener.Emitter = this;
        listeners.Add(listener);
    }
    public void Disconnect(IEmitterListener listener)
    {
        listeners.Remove(listener);
    }

    protected void Emit(string signal, object[] args = null)
    {
        foreach(IEmitterListener listener in listeners)
        {
            listener.Event(signal, args);
        }
    }
}