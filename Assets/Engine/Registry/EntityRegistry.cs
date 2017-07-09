using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using System;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class EntityRegistry {

    static string path = "Data";

    static public Dictionary<string, EntityBuilding> buildingsRegistry = new Dictionary<string, EntityBuilding>();
    static public Dictionary<string, Job> jobRegistry = new Dictionary<string, Job>();

    public EntityRegistry()
    {
        ReadScripts();

        ReadData();
    }


    private void ReadScripts()
    {
        LUA.ScriptLoader.RegisterPlaceolder(null, typeof(EntityRegistry));

        LUA.ScriptLoader.LoadScript(Job.category, "Entity/jobs.lua");
        LUA.ScriptLoader.LoadScript(EntityBuilding.category, "Entity/buildings.lua");
    }

    private void ReadData()
    {
        ReadXml("buildings.xml");
        ReadXml("Jobs.xml");
    }

    public void ReadXml(string fileName) {
        string fullPath = PathUtilities.GetPath(path, fileName);

        if (File.Exists(fullPath))
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(fullPath, settings);

            ReadPrototypes(reader);
        } else
        {
            Debug.LogError(String.Format("Error while loading prototypes from {0}.", fullPath));
        }
    }

    public void ReadPrototypes(XmlReader xmlReader)
    {
        xmlReader.MoveToContent();

        Debug.Assert(xmlReader.Name == "Prototypes", 
            String.Format("Wrong Xml Name for 'Prototypes', instead is '{0}'", xmlReader.Name)
            );
        while(xmlReader.Read())
        {
            if(xmlReader.NodeType == XmlNodeType.Element)
            {
                ReadElement(xmlReader);
            }
        }
    }

    void ReadElement(XmlReader reader)
    {
        XmlReader subReader = reader.ReadSubtree();
        switch (reader.Name)
        {
            case "EntityBuilding":
                EntityBuilding entityBuilding = new EntityBuilding(subReader);
                buildingsRegistry.Add(entityBuilding.id, entityBuilding);
                break;
            case "Job":
                Job newJob = new Job(subReader);
                jobRegistry.Add(newJob.id, newJob);
                break;
            default:
                break;
        }
        subReader.Close();
    }

    internal static Entity InstantiateCharacter()
    {
        EntityHuman entityHuman = new EntityHuman();
        entityHuman.name = "Jim";
        return entityHuman;
    }

    public static EntityBuilding InstantiateEntityBuilding(string id)
    {
        return new EntityBuilding(buildingsRegistry[id]);
    }
}
