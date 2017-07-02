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

    public EntityRegistry()
    {
        string buldingsPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, path), "buildings.xml");

        if (File.Exists(buldingsPath))
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(buldingsPath, settings);
            
            ReadPrototypes(reader);
        } else
        {
            Debug.LogError("Error while loading prototypes.");
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
                switch(xmlReader.Name)
                {
                    case "EntityBuilding":
                        XmlReader subReader = xmlReader.ReadSubtree();
                        EntityBuilding entityBuilding = new EntityBuilding(subReader);
                        buildingsRegistry.Add(entityBuilding.id, entityBuilding);
                        subReader.Close();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    internal static Entity InstantiateCharacter()
    {
        EntityHuman entityHuman = new EntityHuman();
        entityHuman.name = "Jim";
        return entityHuman;
    }

    internal static EntityBuilding InstantiateEntityBuilding(string id)
    {
        return new EntityBuilding(buildingsRegistry[id]);
    }
}
