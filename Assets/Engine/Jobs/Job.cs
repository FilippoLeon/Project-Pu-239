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

        foreach(String key in parameters.Keys)
        {
            Parameter newParameter = new Parameter(parameters[key]);
            parametersList.Add(newParameter);
            parameters[key] = newParameter;
        }

        currentStage = other.currentStage;
        foreach (Stage otherStage in stages)
        {
            stages.Add(new Stage(otherStage, this));
        }
    }

    public Job(XmlReader reader)
    {
        ReadXml(reader);
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
        for (int i = 0; i < newJobParameters.Length; i++)
        {
            parametersList[i].value = Convert.ChangeType(newJobParameters[i], parametersList[i].type);
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
                AddParameter(reader.ReadElementContentAsString(), new Parameter(null, Type.GetType(reader.Name)));
            }
        }
    }

    public override void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }

    public class Parameter
    {
        public object value;
        public Type type;

        public Parameter(Parameter other)
        {
            this.value = other.value;
            this.type = other.type;
        }

        public Parameter(object obj, Type type)
        {
            this.value = obj;
            this.type = type;
        }
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

    Stage CurrentStage()
    {
        return stages[currentStage];
    }
}
