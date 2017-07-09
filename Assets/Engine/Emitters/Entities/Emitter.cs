using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public abstract partial class Emitter : IXmlSerializable, IEmitter, INotifyPropertyChanged
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
        OnClick, // world
        OnUpdate, // UI, world
        OnCreate, // UI, world
        OnComplete // World world, object[] parameters
    }

    public void AddAction(string name, string type, string content)
    {

        if (name == null)
        {
            Debug.LogWarning(String.Format("Try to add empty Action with no-name to list, content ={0}.", content));
            return;
        }
        Action action = new Action(this, name, type, content);
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

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    protected void SetPropertyField<T>(string propertyName, ref T field, T newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(field, newValue))
        {
            field = newValue;
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion
}