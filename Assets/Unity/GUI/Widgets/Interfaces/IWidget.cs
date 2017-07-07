using UnityEngine;

namespace UI
{
    public interface IWidget : IEmitter
    {
        void SetParent(IWidget parent);
        string Id { set; get; }
        GameObject GameObject { set; get; }
        
        void Update(object[] args);
    }
}