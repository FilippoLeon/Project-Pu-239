﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityComponent : MonoBehaviour, IEntityBuildingListener {

    SpriteRenderer spriteRenderer;

    Entity entity;
    public Emitter Emitter
    {
        get
        {
            return entity;
        }

        set
        {
            entity = (Entity) value;
        }
    }

    public static EntityComponent SpawnEntityControllerInWorld(
        WorldComponent worldComponent, Entity entity, 
        Vector2 coord)
    {
        GameObject entityObject = new GameObject();
        EntityComponent entityComponent = entityObject.AddComponent<EntityComponent>();
        entityObject.transform.SetParent(worldComponent.transform);
        //entityComponent.SetPosition(coord);

        PolygonCollider2D collider = entityObject.AddComponent<PolygonCollider2D>();
        collider.points = new Vector2[] {
            new Vector2(-0.5f, -0.5f),
            new Vector2(-0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, -0.5f)
        };

        entityComponent.spriteRenderer = entityObject.AddComponent<SpriteRenderer>();
        entityComponent.spriteRenderer.sprite = SpriteLoader.GetSprite("test");

        return entityComponent;
    }

    public void Event(string signal, object[] args)
    {
        throw new NotImplementedException();
    }

    public void Spawn(World world, Vector2 position)
    {
        name = entity.id;

        if (entity is EntityAnimated) name = ((EntityAnimated) entity).name;
        spriteRenderer.sprite = SpriteLoader.GetSprite("character", "one");
    }

    public void InstallAt(World world, World.Coord coord)
    {
        SetEntitySprite(world);
    }

    public void NeighbourChanged(World world, Tile neighbour)
    {
        SetEntitySprite(world);
    }

    public void SetEntitySprite(World world)
    {
        if (Emitter is Entity)
        {
            Entity.SpriteInfo spriteInfo = (Emitter as Entity).spriteInfo;
            if (spriteInfo.type == "connecting")
            {
                spriteRenderer.sprite = SpriteLoader.GetSprite(spriteInfo.id, ((EntityBuilding)Emitter).GetConnectingNeighbours());
            } else
            {
                spriteRenderer.sprite = SpriteLoader.GetSprite(spriteInfo.id);
            }
        }
    }

    //private void SetPosition(Vector2 position)
    //{
    //    this.transform.position = position;
    //}

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PositionChanged(World world, Vector2 position)
    {
        this.transform.position = position;
    }

    public void Despawn(World world)
    {
        Destroy(gameObject);
    }
}
