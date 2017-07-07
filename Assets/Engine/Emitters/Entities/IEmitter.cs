public interface IEmitter
{
    string Category();
    void Emit(string signal, object[] args);
}