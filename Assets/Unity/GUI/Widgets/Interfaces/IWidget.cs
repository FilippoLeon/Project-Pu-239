using UnityEngine;

namespace UI
{
    public interface IWidget
    {
        void SetParent(IWidget parent);
        string Id { set; get; }
        GameObject GameObject { set; get; }
        
        void Update(object[] args);
        void CallAction(string actionName, object[] args);
        void AddAction(string actionName, string actionFunction);
    }
}