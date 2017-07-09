using LUA;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

namespace UI
{
    [MoonSharpUserData]
    public class Widget : Emitter, IWidget
    {
        IWidget parent = null;

        LayoutElement layoutComponent;

        public string Id {
            set {
                id = value;
                GameObject.name = id;
            } get {
                return id;
            }
        }

        static int staticId;

        public Widget() {
            GameObject = new GameObject();
            Id = "W" + Convert.ToString(staticId++);

            GameObject.transform.SetParent(GUIController.Canvas.transform);
            
            layoutComponent = GameObject.AddComponent<LayoutElement>();
            
        }

        public virtual void SetNonExpanding(int minWidth = -1)
        {
            if(minWidth >= 0) layoutComponent.minWidth = minWidth;
            layoutComponent.preferredWidth = 0;
        }

        public GameObject GameObject { set; get; }
        
        public void SetParent(IWidget parent)
        {
            this.parent = parent;
            if(parent != null)
            {
                GameObject.transform.SetParent(parent.GameObject.transform);
            }
        }

        public virtual void Update(object[] args)
        {
            Emit("OnUpdate", args);
        }
        
        public override string Category()
        {
            return "UI";
        }

        internal void ReadElement(XmlReader reader, IWidget parent)
        {
            if (reader.GetAttribute("id") != null)
            {
                Id = reader.GetAttribute("id");
            }
            string preferredSize = reader.GetAttribute("preferredSize");
            if (preferredSize != null)
            {
                Vector2 pfs = XmlUtilities.ToVector2(preferredSize);
                if(pfs.x >= 0)
                {
                    layoutComponent.preferredWidth = pfs.x;
                }
                if (pfs.y >= 0)
                {
                    layoutComponent.preferredHeight = pfs.y;
                }
            }
        }
    }
}