using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections.Generic;

namespace UI
{
    [MoonSharpUserData]
    public class Panel : Widget<Panel>, IWidgetContainer
    {
        Image background;

        private LayoutGroup layout;
        public LayoutGroup Layout
        {
            get
            {
                return layout;
            }

            set
            {
                layout = value;
            }
        }
        List<IWidget> childs = new List<IWidget>();
        public IEnumerable<IWidget> Childs
        {
            get
            {
                return childs;
            }
        }

        public Panel()
        {
            GameObject.AddComponent<CanvasRenderer>();
            Layout = GameObject.AddComponent<HorizontalLayoutGroup>();
            background = GameObject.AddComponent<Image>();
            background.type = Image.Type.Sliced;
            background.sprite = SpriteLoader.TryLoadSprite("UI", "panel_background");

            SetAnchor(new Vector2(0, 0), new Vector2(1, 0.1f));
            SetSize();
            SetMargin();
        }

        public void SetAnchor(Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject.GetComponent<RectTransform>().anchorMin = anchorMin;
            GameObject.GetComponent<RectTransform>().anchorMax = anchorMax;
        }
        
        public void SetSize()
        {
            GameObject.GetComponent<RectTransform>().offsetMin = new Vector2(50, 10);
            //GameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-10,-50);
        }

        public void SetMargin()
        {
            //layout.padding = new RectOffset(10, 10, 10, 10);
            //layout.
            if (layout is HorizontalOrVerticalLayoutGroup) { 
                (layout as HorizontalOrVerticalLayoutGroup).spacing = 10;
            }
        }

        public void ReadXml(XmlReader reader)
        {

        }

        void ReadInnerElement(XmlReader reader)
        {

        }

        public enum WidgetLayout
        {
            Horizontal, Vertical, Grid,
        }

        public void SetLayout(WidgetLayout layout)
        {
            Layout.SetLayoutHorizontal();
        }

        public void Add(IWidget child)
        {
            childs.Add(child);
            child.GameObject.transform.SetParent(GameObject.transform);
        }
        public Panel(string id) : this()
        {
            this.id = id;
        }

        public new static Panel Create(string id)
        {
            return new Panel(id);
        }
        public static Panel Create(XmlReader reader, IWidget parent = null)
        {
            Panel panel = new Panel();
            if (reader.GetAttribute("id") != null)
            {
                panel.Id = reader.GetAttribute("id");
            }
            panel.SetParent(parent);
            
            if (reader.GetAttribute("anchorMin") != null || reader.GetAttribute("anchorMax") != null)
            {
                Vector2 anchorMin = XmlUtilities.ToVector2(reader.GetAttribute("anchorMin"));
                Vector2 anchorMax = XmlUtilities.ToVector2(reader.GetAttribute("anchorMax"));
                panel.SetAnchor(anchorMin, anchorMax);
            }
            string layout = reader.GetAttribute("layout");
            switch(layout)
            {
                case "grid":
                    panel.SetLayout(WidgetLayout.Grid);
                    break;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //if (reader.Name == "Text")
                    //{

                    //}
                    //else
                    //{
                        XmlReader subReader = reader.ReadSubtree();
                        GUIController.ReadElement(subReader, panel);
                        subReader.Close();
                    //}
                }
            }


            return panel;
            }

        //public override void Update(object[] args)
        //{
        //    base.Update(args);

        //    foreach(IWidget child in Childs)
        //    {
        //        child.Update(args);
        //    }
        //}
    }

}