using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Collections.Generic;

namespace UI
{
    [MoonSharpUserData]
    public class Panel : Widget, IWidgetContainer
    {
        Image background;

        private LayoutGroup layout;

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
            background = GameObject.AddComponent<Image>();
            background.type = Image.Type.Sliced;
            background.sprite = SpriteLoader.TryLoadSprite("UI", "panel_background");

            SetAnchor(new Vector2(0, 0), new Vector2(1, 0.1f));
            SetSize();
            SetMargin();
        }

        public void SetLayout(string layoutName)
        {
            switch (layoutName)
            {
                case "grid":
                    layout = GameObject.AddComponent<GridLayoutGroup>();
                    break;
                case "horizontal":
                    layout = GameObject.AddComponent<HorizontalLayoutGroup>();
                    SetChildExpand(false);
                    break;
                case "vertical":
                    layout = GameObject.AddComponent<VerticalLayoutGroup>();
                    SetChildExpand(false);
                    break;
            }
            
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
            GameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-50, -10);
        }

        public void SetMargin()
        {
            //layout.padding = new RectOffset(10, 10, 10, 10);
            //layout.
            if (layout is HorizontalOrVerticalLayoutGroup) { 
                (layout as HorizontalOrVerticalLayoutGroup).spacing = 10;
            }
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
        public void SetChildExpand(bool expand = true)
        {
            if(layout is HorizontalOrVerticalLayoutGroup)
            {
                (layout as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = expand;
            }
        }

        public void AddContentSizeFitter(ContentSizeFitter.FitMode mode)
        {
            ContentSizeFitter fitter = GameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = mode;
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
            panel.SetLayout(layout);
            switch(layout)
            {
                case "grid":
                    if(reader.GetAttribute("gridX") != null)
                    {
                        (panel.layout as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedRowCount;
                        (panel.layout as GridLayoutGroup).constraintCount = Convert.ToInt32(reader.GetAttribute("gridX"));
                    } else if(reader.GetAttribute("gridY") != null)
                    {
                        (panel.layout as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                        (panel.layout as GridLayoutGroup).constraintCount = Convert.ToInt32(reader.GetAttribute("gridY"));
                    }
                    break;
                case "horizontal":
                    break;
                case "vertical":
                    break;
            }

            switch(reader.GetAttribute("content"))
            {
                case "minFit":
                    panel.AddContentSizeFitter(ContentSizeFitter.FitMode.MinSize);
                    break;
                case "preferredFit":
                    panel.AddContentSizeFitter(ContentSizeFitter.FitMode.PreferredSize);
                    break;
                default:
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