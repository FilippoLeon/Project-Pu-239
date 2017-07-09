using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

namespace UI
{
    [MoonSharpUserData]
    public class Label : Widget
    {
        Text textComponent;
        
        public string Text
        {
            get
            {
                return textComponent.text;
            }
            set
            {
                textComponent.text = value;
            }
        }
        
        public Label()
        {
            textComponent = GameObject.AddComponent<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }
        
        public Label(string id) : this()
        {
            Id = id;
        }

        public static Label Create(string id)
        {
            return new Label(id);
        }

        public static Label Create(XmlReader reader, IWidget parent = null)
        {
            Label label = new Label();
            label.ReadElement(reader, parent);
            label.SetParent(parent);

            while(reader.Read())
            {
                if(reader.NodeType == XmlNodeType.Element)
                {
                    if(reader.Name == "Text")
                    {
                        label.Text = reader.ReadElementContentAsString();
                    } else
                    {
                        XmlReader subReader = reader.ReadSubtree();
                        GUIController.ReadElement(subReader, label);
                        subReader.Close();
                    }
                }
            }

            
            return label;
        }
    }
}