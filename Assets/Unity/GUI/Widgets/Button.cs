using System;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

namespace UI
{
    [MoonSharpUserData]
    public class Button : Widget
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
            backgroundComponent.SetNativeSize();

            buttonComponent = GameObject.AddComponent<UnityEngine.UI.Button>();

            GameObject textGameObject = new GameObject("text", new Type[] { typeof(Text) });
            textGameObject.transform.SetParent(GameObject.transform);
            textComponent = GameObject.GetComponentInChildren<Text>();
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textComponent.rectTransform.anchorMin = new Vector2(0, 0);
            textComponent.rectTransform.anchorMax = new Vector2(1, 1);
            textComponent.rectTransform.offsetMin = Vector2.zero;
            textComponent.rectTransform.offsetMax = Vector2.zero;

            SetNonExpanding();
        }

        public override void SetNonExpanding(int minWidth = -1)
        {
            base.SetNonExpanding((int) backgroundComponent.sprite.rect.width);
        }

        public override void AddAction(ActionType type, Action action)
        {
            if(type == ActionType.OnClick) {
                buttonComponent.onClick.AddListener(
                    () => action.Call( this, new object[] { WorldController.StaticWorld })
                    );
            }
            base.AddAction(type, action);
        }
        
        // MoonSharp.Interpreter.Closure
        public void OnClick(MoonSharp.Interpreter.Closure callback)
        {
            buttonComponent.onClick.AddListener(() => callback.Call());
        }

        public Button(string id) : this()
        {
            Id = id;
        }

        public static Button Create(string id)
        {
            return new Button(id);
        }

        public static Button Create(XmlReader reader, IWidget parent = null)
        {
            Button button = new Button();
            button.ReadElement(reader, parent);

            String type = reader.GetAttribute("type");
            
            button.SetParent(parent);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch(reader.Name)
                    {
                        case "Text":
                            button.Text = reader.ReadElementContentAsString();
                            break;
                        case "Sprite":
                            Sprite sprite = SpriteLoader.Load(new SpriteInfo(reader));
                            button.backgroundComponent.sprite = sprite;
                            //button.backgroundComponent.color = new Color(0, 1, 0, 0.5f);
                            //button.buttonComponent.colors. = new ColorBlock();
                            //color
                            break;
                        default:
                            XmlReader subReader = reader.ReadSubtree();
                            GUIController.ReadElement(subReader, button);
                            subReader.Close();
                            break;
                    }
                }
            }

            switch(type)
            {
                case "sliced":
                default:
                    button.backgroundComponent.type = Image.Type.Sliced;
                    break;
                case "simple":
                    button.backgroundComponent.type = Image.Type.Simple;
                    button.backgroundComponent.preserveAspect = true;
                    break;
            }

            return button;
        }

        public void SetTint(Color color)
        {
            backgroundComponent.color = color;
        }
        
        public void SetColorBlock(Color normal, Color pressed, Color disabled)
        {
            ColorBlock block = new ColorBlock();
            block.normalColor = normal;
            block.pressedColor = pressed;
            block.disabledColor = disabled;
            buttonComponent.colors = block;
        }
    }

}