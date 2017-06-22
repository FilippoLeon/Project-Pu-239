using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class EntityInanimate : Entity, IXmlSerializable
{

    public EntityInanimate(string id = null)
        : base(id)
    {
    }

    public EntityInanimate(EntityInanimate other)
        : base(other)
    {

    }

    override public XmlSchema GetSchema()
    {
        return null;
    }

    override public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Debug.Assert(reader.Name == "EntityInanimate", 
            String.Format("Wrong Xml Name for 'EntityInanimate', instead is '{0}'", reader.Name)
            );
        if (id == null) id = reader.GetAttribute("id");

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "Entity":
                        XmlReader subReader = reader.ReadSubtree();
                        base.ReadXml(subReader);
                        subReader.Close();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    override public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
}