using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

abstract public class Emitter : IXmlSerializable, IEmitter
{
    public string id;

    protected HashSet<IEmitterListener> listeners = new HashSet<IEmitterListener>();

    protected Dictionary<string, Emitter.Action> actions = new Dictionary<string, Emitter.Action>();

    public abstract string Category();

    public Emitter() { }

    public Emitter(Emitter other)
    {
        id = other.id;
        actions = other.actions;
        if(listeners.Count >= 1)
        {
            Debug.LogWarning("Copying Emitter with non-empty listeners, listeners will not be copied.");
        }
    }

    public class Action
    {
        ActionType type;
        string content;
        Script script;
        Closure closure;

        public enum ActionType { FunctionName, Inline, None, Closure };
       

        public Action(string type, string content)
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
                    script = new Script();
                    script.Options.DebugPrint += (string str) =>
                    {
                        Debug.Log("LUA inline: " + str);
                    };
                    script.DoString("action =" + content);
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
                    try
                    {
                        return script.Call(script.Globals["action"], args);
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

    public void Connect(IEmitterListener listener)
    {
        listener.Emitter = this;
        listeners.Add(listener);
    }
    public void Disconnect(IEmitterListener listener)
    {
        listeners.Remove(listener);
    }

    virtual public XmlSchema GetSchema()
    {
        return null;
    }

    public virtual void ReadElement(XmlReader reader)
    {
        switch (reader.Name)
        {
            case "Action":
                string actionName = reader.GetAttribute("event");
                string type = reader.GetAttribute("type");
                string actionFunction = reader.ReadElementContentAsString();
                AddAction(actionName, type, actionFunction);
                break;
        }
    }

    public enum ActionType
    {
        OnClick, OnUpdate, OnCreate
    }

    public void AddAction(string name, string type, string content)
    {

        if (name == null)
        {
            Debug.LogWarning(String.Format("Try to add empty Action with no-name to list, content ={0}.", content));
            return;
        }
        Action action = new Action(type, content);
        actions.Add(name, action);
        AddAction((ActionType) Enum.Parse(typeof(ActionType), name), action);
    }

    public virtual void AddAction(ActionType actionType, Action action) { }

    public virtual void AddAction(string eventName, MoonSharp.Interpreter.Closure closure)
    {
        Action action = new Action(closure);
        actions.Add(eventName, action);
    }

    virtual public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    virtual public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

    public void Emit(string signal, object[] args = null)
    {
        if(actions.ContainsKey(signal))
        {
            actions[signal].Call(this, args);
        }
        
        foreach(IEmitterListener listener in listeners)
        {
            listener.Event(signal, args);
        }
    }
    
}