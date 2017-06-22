using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class EntityBuilding : EntityInanimate, IXmlSerializable
{
    bool installed = false;
    private float walkingSpeed = 1;
    private Tile tile = null;

    public EntityBuilding(string id = null) 
        : base(id)
    {

    }

    public EntityBuilding(EntityBuilding other)
        : base(other)
    {
        this.installed = other.installed;
        this.walkingSpeed = other.walkingSpeed;
    }

    public EntityBuilding(XmlReader reader)
    {
        ReadXml(reader);
    }

    public bool Passable
    {
        get
        {
            return walkingSpeed > 0;
        }
    }

    public float WalkingSpeed
    {
        get
        {
            return walkingSpeed;
        }

        set
        {
            walkingSpeed = value;
        }
    }

    public Tile Tile
    {
        get
        {
            return tile;
        }

        set
        {
            tile = value;
            InstallAt(tile.world, tile.coord);
        }
    }

    override public XmlSchema GetSchema()
    {
        return null;
    }

    override public void ReadXml(XmlReader reader)
    {
        reader.Read();

        Debug.Assert(reader.Name == "EntityBuilding",
            String.Format("Wrong Xml Name for 'EntityBuilding', instead is '{0}'", reader.Name)
            );
        if (id == null) id = reader.GetAttribute("id");

        Debug.Log(string.Format("Reading 'EntityBuilding' from Xml, id = '{0}'", id));
        while(reader.Read())
        {
            if(reader.NodeType == XmlNodeType.Element)
            {
                switch(reader.Name)
                {
                    case "EntityInanimate":
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

    private void InstallAt(World world, World.Coord coord)
    {
        //world.InstallAt(this, coord);

        foreach(IListener listener in listeners)
        {
            if(listener is IEntityBuildingListener) ((IEntityBuildingListener) listener).InstallAt(world, coord);
        }

        foreach(Tile neighbour in Tile.GetNeighbours())
        {
            neighbour.OnNeighbourChanged(world, Tile);
        }
    }

    public void NeighbourChange(World world, Tile neighbour)
    {
        foreach (IListener listener in listeners)
        {
            if (listener is IEntityBuildingListener) ((IEntityBuildingListener)listener).NeighbourChanged(world, neighbour);
        }
    }

    override public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
    

    public BitArray GetConnectingNeighbours()
    {
        BitArray connection = new BitArray(8);
        for(int i = 0; i < 8; ++i)
        {
            Tile neighbour = Tile.GetNeighbour(i);
            if (neighbour == null) continue;
            connection[i] = neighbour.building != null;
        }

        return connection;
    }
}
