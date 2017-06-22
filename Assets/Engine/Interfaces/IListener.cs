public interface IListener
{
    void Event(string signal, object[] args);

    Emitter Emitter { set; get; }
}