using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public partial class Entity : Emitter, IXmlSerializable {
    
    public World world;
    private Vector2 position;

    public static string category = "entitties";
    public override string Category() { return category; }

    public SpriteInfo spriteInfo = new SpriteInfo();

    public Vector2 Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
            foreach (IEmitterListener listener in listeners)
            {
                if (listener is IEntityListener) ((IEntityListener)listener).PositionChanged(world, position);
            }
        }
    }

    public Entity(string id = null)
    {
    }

    public Entity(Entity other)
    {
        this.id = other.id;
        this.spriteInfo = other.spriteInfo;
    }

    override public XmlSchema GetSchema()
    {
        return null;
    }

    public void Spawn(World world, Vector2 position)
    {
        this.world = world;
        this.Position = position;

        foreach (IEmitterListener listener in listeners)
        {
            if(listener is IEntityListener) ((IEntityListener) listener).Spawn(world, position);
        }
    }

    override public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Debug.Assert(reader.Name == "Entity", 
            String.Format("Wrong Xml Name for 'Entity', instead is '{0}'", reader.Name)
            );
        if (id == null) id = reader.GetAttribute("id");

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                ReadElement(reader);
            }
        }
    }

    public override void ReadElement(XmlReader reader)
    {
        switch (reader.Name)
        {
            case "Sprite":
                spriteInfo = new SpriteInfo(reader);
                break;
            default:
                break;
        }
    }

    override public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

    internal void Despawn(World world)
    {
        foreach (IEmitterListener listener in listeners)
        {
            if (listener is IEntityListener) ((IEntityListener) listener).Despawn(world);
        }
    }
}
