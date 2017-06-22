using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class Entity : Emitter, IXmlSerializable {

    public string id;

    public Entity(string id = null)
    {
    }


    public Entity(Entity other)
    {
        this.id = other.id;
    }

    virtual public XmlSchema GetSchema()
    {
        return null;
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
}
