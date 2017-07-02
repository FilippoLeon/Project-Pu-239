using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public class Entity : Emitter, IXmlSerializable {

    public string id;
    public World world;
    private Vector2 position;

    public struct SpriteInfo {
        public string id;
        public string type;
    }

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

    virtual public XmlSchema GetSchema()
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

    virtual public void ReadXml(XmlReader reader)
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
                switch (reader.Name)
                {
                    case "Sprite":
                        //Debug.Log("Reading sprite information.");
                        string type = reader.GetAttribute("type");
                        if (type != null)
                        {
                            //Debug.Log("Reading sprite information.");
                            spriteInfo.type = type;
                        }
                        string spriteId = reader.GetAttribute("id");
                        if(spriteId == null)
                        {
                            Debug.LogWarning("No id for Sprite!");
                        } else
                        {
                            spriteInfo.id = spriteId;
                        }
                        Debug.Log(String.Format("Sprite of {0} has id {1} and type {2}", id, spriteInfo.id, spriteInfo.type));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    virtual public void WriteXml(XmlWriter writer)
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
