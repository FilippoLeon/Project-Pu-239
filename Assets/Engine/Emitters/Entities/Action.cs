using MoonSharp.Interpreter;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using UnityEngine;

public abstract partial class Emitter : IXmlSerializable, IEmitter, INotifyPropertyChanged
{
    public class Action
    {
        ActionType type;
        string name;
        string content;
        Closure closure;

        public enum ActionType { FunctionName, Inline, None, Closure };
       

        public Action(Emitter emitter, string eventName, string type, string content)
        {
            this.content = content;
            if(content == "")
            {
                Debug.LogWarning("Empty or Invalid Action field.");
                this.type = ActionType.None;
                return;
            }
            switch(type)
            {
                case "script":
                    this.type = ActionType.FunctionName;
                    break;
                default:
                    this.type = ActionType.Inline;
                    name = emitter.id + "_" + eventName;
                    //Debug.Log(String.Format("New inline action {0}", name));
                    LUA.ScriptLoader.DoString(emitter.Category(), name + " = " + content);
                    return;
            }
        }

        public Action(Closure closure)
        {
            this.closure = closure;
            this.type = ActionType.Closure;
        }

        public DynValue Call(Emitter emitter, object[] args)
        {
            switch (type)
            {
                case ActionType.FunctionName:
                    return LUA.ScriptLoader.Call(emitter.Category(), content, args);
                case ActionType.Inline:
                    return LUA.ScriptLoader.Call(emitter.Category(), name, args);
                case ActionType.Closure:
                    try
                    {
                        return closure.Call(args);
                    }
                    catch (ScriptRuntimeException e)
                    {
                        Debug.LogError("Script exception: " + e.DecoratedMessage);
                        return null;
                    }
                    catch (ArgumentException e)
                    {
                        Debug.LogError("Script exception while running call to action: " + e.Message);
                        return null;
                    }
                default:
                    return null;
            }
        }
    }
    
}