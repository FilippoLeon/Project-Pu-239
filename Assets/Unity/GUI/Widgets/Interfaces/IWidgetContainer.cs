using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
    public interface IWidgetContainer : IWidget
    {
        LayoutGroup Layout { get; set; }

        IEnumerable<IWidget> Childs { get; }
    }
}