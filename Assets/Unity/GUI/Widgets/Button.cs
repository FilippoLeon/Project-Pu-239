using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

namespace UI
{
    [MoonSharpUserData]
    public class Button : Widget<Button>
    {
        Image backgroundComponent;
        UnityEngine.UI.Button buttonComponent;
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



        public Button()
        {
            backgroundComponent = GameObject.AddComponent<Image>();
            backgroundComponent.type = Image.Type.Sliced;
            backgroundComponent.sprite = SpriteLoader.TryLoadSprite("UI", "button_background");
            

            buttonComponent = GameObject.AddComponent<UnityEngine.UI.Button>();

            GameObject textGameObject = new GameObject("text", new Type[] { typeof(Text) });
            textGameObject.transform.SetParent(GameObject.transform);
            textComponent = GameObject.GetComponentInChildren<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        }

        public void OnClick(MoonSharp.Interpreter.Closure callback)
        {
            buttonComponent.onClick.AddListener(() => callback.Call());
        }

        public Button(string id) : this()
        {
            Id = id;
        }

        public new static Button Create(string id)
        {
            return new Button(id);
        }

        public static Button Create(XmlReader reader, IWidget parent = null)
        {
            Button button = new Button();
            if (reader.GetAttribute("id") != null)
            {
                button.Id = reader.GetAttribute("id");
            }
            button.SetParent(parent);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Text")
                    {
                        button.Text = reader.ReadElementContentAsString();
                    }
                    else
                    {
                        XmlReader subReader = reader.ReadSubtree();
                        GUIController.ReadElement(subReader, button);
                        subReader.Close();
                    }
                }
            }


            return button;
        }
    }
}