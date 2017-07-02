using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    private static Canvas canvas;

    public WorldController worldController;

    static public Dictionary<string, IWidget> childs = new Dictionary<string, IWidget>();

    void Start()
    {
        canvas = FindObjectOfType(typeof(Canvas)) as Canvas;

        LUA.ScriptLoader.LoadScript("UI", "UI/UI.lua");
        
        LUA.ScriptLoader.RegisterPlaceolder("UI", typeof(Panel));
        LUA.ScriptLoader.RegisterPlaceolder("UI", typeof(UI.Button));
        LUA.ScriptLoader.RegisterPlaceolder("UI", typeof(Panel.WidgetLayout));

        BuildUI();

        //LUA.ScriptLoader.Call("UI", "buildBuildingPlacementPanel", new object[] { worldController.World });
        
    }
    
    void BuildUI()
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        XmlReader reader = XmlReader.Create(PathUtilities.GetPath("Data/UI", "UI.xml"), settings);

        reader.MoveToContent();
        Debug.Assert(reader.Name == "Canvas");

        while (reader.Read())
        {
            XmlNodeType nodeType = reader.NodeType;
            switch (nodeType)
            {
                case XmlNodeType.Element:
                    XmlReader subTree = reader.ReadSubtree();
                    ReadElement(subTree);
                    subTree.Close();
                    break;
            }
        }
    }

    public static void ReadElement(XmlReader reader, IWidget parent = null)
    {
        reader.Read();
        IWidget child = null;
        switch(reader.Name)
        {
            case "Panel":
                child = Panel.Create(reader, parent);
                break;
            case "Label":
                child = Label.Create(reader, parent);
                break;
            case "Button":
                child = UI.Button.Create(reader, parent);
                break;
            case "Action":
                string actionName = reader.GetAttribute("name");
                string actionFunction = reader.ReadElementContentAsString();
                if (actionName != null)
                {
                    parent.AddAction(actionName, actionFunction);
                }
                break;
        }

        if(child != null)
        {
            childs.Add(child.Id, child);
            child.CallAction("OnCreate", new object[] { childs, WorldController.StaticWorld });
        }
      
    }

    public static Canvas Canvas
    {
        get { return canvas; }
    }

    private void Update()
    {
        foreach(IWidget child in childs.Values)
        {
            child.Update(new object[] { childs, worldController.World });
        }
        //background.sprite = SpriteLoader.TryLoadSprite("asd", "asd");
    }
}
