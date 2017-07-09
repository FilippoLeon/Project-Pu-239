using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

[MoonSharpUserData]
public partial class Job : Emitter, IXmlSerializable
{
    public static string category = "jobs";
    public override string Category() { return category; }

    public string jobCategory;

    List<Entity> reservedEntites = new List<Entity>();

    public List<Parameter> parametersList = new List<Parameter>();
    public Dictionary<String, Parameter> parameters = new Dictionary<String, Parameter>();

    int currentStage = 0;
    List<Stage> stages = new List<Stage>();

    public Job(Job other) : base(other)
    {
        jobCategory = other.jobCategory;

        foreach(String key in other.parameters.Keys)
        {
            Parameter newParameter = new Parameter(other.parameters[key]);
            parametersList.Add(newParameter);
            parameters[key] = newParameter;
        }

        currentStage = other.currentStage;
        foreach (Stage otherStage in other.stages)
        {
            stages.Add(new Stage(otherStage, this));
        }
    }

    public Job(XmlReader reader)
    {
        ReadXml(reader);
    }
    
    internal bool CanProgress()
    {
        return true;
    }
    
    internal void Progress(Entity entity, World world)
    {
        CurrentStage().DoWork(entity, world);
        if(CurrentStage().Done())
        {
            NextStage();
        }
    }
    public override void ReadXml(XmlReader reader)
    {
        reader.Read();

        Debug.Assert(reader.Name == "Job",
            String.Format("Wrong Xml Name for 'Job', instead is '{0}'", reader.Name)
            );
        id = reader.GetAttribute("id");
        jobCategory = reader.GetAttribute("category");

        Debug.Log(string.Format("Reading 'Job' from Xml, id = '{0}'", id));
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "Parameters":
                        XmlReader subReader = reader.ReadSubtree();
                        ReadParameters(subReader);
                        subReader.Close();
                        break;
                    case "Stage":
                        XmlReader subReaderStage = reader.ReadSubtree();
                        Stage stage = new Stage(subReaderStage, this);
                        this.AddStage(stage);
                        subReaderStage.Close();
                        break;
                    default:
                        break;
                }
            }
        }
    }


    public void Schedule(object[] newJobParameters)
    {
        Debug.Log(String.Format("New job {0} is scheduled {1} at {2}.", 
            id, newJobParameters[0], newJobParameters[1]));
        for (int i = 0; i < newJobParameters.Length; i++)
        {
            parametersList[i].value = Convert.ChangeType(newJobParameters[i], parametersList[i].type);
        }

        foreach(IEmitterListener listener in listeners)
        {
            // FIX THIS
            ((IJobListener)listener).Schedule(null);
        }
    }

    public void Start()
    {

    }

    public Parameter GetTarget()
    {
        Stage currentStage = CurrentStage();

        switch(currentStage.targetType)
        {
            case "coordinate":
                return parameters[currentStage.target];
            default:
                break;
        }
        return null;
    }

    private void AddStage(Stage stage)
    {
        stages.Add(stage);
    }

    private void ReadParameters(XmlReader reader)
    {
        reader.Read(); 

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                String name = reader.Name;
                AddParameter(reader.ReadElementContentAsString(), 
                    new Parameter(null, ParseAsTypename(name))
                    );
            }
        }
    }

    private Type ParseAsTypename(string str)
    {
        Type type = Type.GetType(str);
        if (type == null)
        {
            switch (str)
            {
                case "String":
                    return typeof(String);
                case "World.Coord":
                    return typeof(World.Coord);
                default:
                    Debug.LogError(String.Format("Unrekognised type: {0}", str));
                    break;
            }
        }
        return type;
    }

    public override void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

    public void AddParameter(String name, Parameter parameter)
    {
        parameters.Add(name, parameter);
        parametersList.Add(parameter);
    }

    void NextStage()
    {
        currentStage += 1;
    }

    public Stage CurrentStage()
    {
        return stages[currentStage];
    }
}
