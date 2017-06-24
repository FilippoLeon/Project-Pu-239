public interface IEmitterListener
{
    void Event(string signal, object[] args);

    Emitter Emitter { set; get; }

}