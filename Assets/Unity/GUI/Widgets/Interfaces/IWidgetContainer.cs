using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
    public interface IWidgetContainer : IWidget
    {

        IEnumerable<IWidget> Childs { get; }
    }
}