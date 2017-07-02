using LUA;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [MoonSharpUserData]
    public class Widget<T> : IWidget where T : IWidget, new()
    {
        IWidget parent = null;

        public string id;
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
            Id = Convert.ToString(staticId++);

            GameObject.transform.SetParent(GUIController.Canvas.transform);
        }

        public GameObject GameObject { set; get; }

        public static T Create(string id)
        {
            T t = new T();
            t.Id = id;
            return t;
        }
        
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
            CallAction("OnUpdate", args);
        }

        Dictionary<string, string> actions = new Dictionary<string, string>();

        public void CallAction(string actionName, object[] args)
        {
            if (actions.ContainsKey(actionName)) {
                ScriptLoader.Call("UI", actions[actionName], args);
            }
        }

        public void AddAction(string actionName, string actionFunction)
        {
            actions[actionName] = actionFunction;
        }
    }
}