using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : MonoBehaviour, IEntityBuildingListener {

    SpriteRenderer spriteRenderer;

    EntityBuilding entityBuilding;
    public Emitter Emitter
    {
        get
        {
            return entityBuilding;
        }

        set
        {
            entityBuilding = (EntityBuilding) value;
        }
    }

    public static EntityComponent SpawnEntityControllerInWorld(
        WorldComponent worldComponent, Entity entity, 
        World.Coord coord)
    {
        GameObject entityObject = new GameObject();
        EntityComponent entityComponent = entityObject.AddComponent<EntityComponent>();
        entityObject.transform.SetParent(worldComponent.transform);
        entityComponent.SetPosition(coord);

        entityComponent.spriteRenderer = entityObject.AddComponent<SpriteRenderer>();
        entityComponent.spriteRenderer.sprite = SpriteLoader.GetSprite("test");

        return entityComponent;
    }

    public void Event(string signal, object[] args)
    {
        throw new NotImplementedException();
    }

    public void InstallAt(World world, World.Coord coord)
    {
        spriteRenderer.sprite = SpriteLoader.GetSprite("wall", ((EntityBuilding)Emitter).GetConnectingNeighbours());
        Debug.Log("-------->" + 
            EnumerableUtilities.PrintValues( ((EntityBuilding)Emitter).GetConnectingNeighbours(), 8)
            );
    }

    public void NeighbourChanged(World world, Tile neighbour)
    {
        spriteRenderer.sprite = SpriteLoader.GetSprite("wall", ((EntityBuilding)Emitter).GetConnectingNeighbours());
        Debug.Log(
            EnumerableUtilities.PrintValues( ((EntityBuilding)Emitter).GetConnectingNeighbours(), 8)
            );
    }

    private void SetPosition(World.Coord coord)
    {
        this.transform.position = new Vector3(coord.x, coord.y, 0);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
