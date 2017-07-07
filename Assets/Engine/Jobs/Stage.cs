using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public class Stage : Emitter, IXmlSerializable
{
    public static string category = "jobs";
    public override string  Category() { return category; }
    
    int completion;
    int cost;
    bool completed;

    public string targetType;
    public string targetPosition;
    public string target;

    List<Entity> resources = new List<Entity>();
    Job job;

    public Stage(Stage other, Job job) : base(other)
    {
        completion = other.completion;
        cost = other.cost;
        completed = other.completed;

        targetType = other.targetType;
        targetPosition = other.targetPosition;
        target = other.target;

        foreach(Entity resource in other.resources)
        {
            resources.Add(new Entity(resource));
        }
        this.job = job;
    }

    public Stage(XmlReader reader, Job job)
    {
        this.job = job;
        ReadXml(reader);
    }
    
    public override void ReadXml(XmlReader reader)
    {
        reader.Read();

        Debug.Assert(reader.Name == "Stage",
            String.Format("Wrong Xml Name for 'Stage', instead is '{0}'", reader.Name)
            );
        id = reader.GetAttribute("id");
        cost = Convert.ToInt32(reader.GetAttribute("cost"));

        Debug.Log(string.Format("Reading 'Stage' from Xml, id = '{0}'", id));
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                XmlReader subReader = reader.ReadSubtree();
                switch (reader.Name)
                {
                    case "Skills":
                        ReadSkill(subReader);
                        break;
                    case "Resources":
                        ReadResource(subReader);
                        break;
                    case "Target":
                        ReadTarget(subReader);
                        break;
                    default:
                        base.ReadElement(subReader);
                        break;
                }
                subReader.Close();
            }
        }
    }

    void ReadTarget(XmlReader reader)
    {
        reader.Read();
        Debug.Assert(reader.Name == "Target",
            String.Format("Wrong Xml Name for 'Target', instead is '{0}'", reader.Name)
            );

        targetType = reader.GetAttribute("type");
        targetPosition = reader.GetAttribute("position");
        target = reader.ReadElementContentAsString();
    }
        

    void ReadResource(XmlReader reader)
    {

    }

    void ReadSkill(XmlReader reader)
    {

    }

    public override void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

    void Start()
    {

    }

    void DoWork(World world)
    {

        completion += 1;
        if(completion >= cost)
        {
            Complete(world);
        }
    }

    void Complete(World world)
    {
        Emit("OnComplete", new object[] { world, job.parameters });
    }
       
}