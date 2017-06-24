using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldComponent : MeshComponent {
    
    new void Awake() {
        base.Awake();

        name = "World";
        
        // Generate palette
        palette = new Color[]
        {
            new Color(0, 0, 1),
            new Color(0, 1, 0)
        };
        int paletteSize = palette.Length;
       
        // Palette texture generation
        paletteTexture = new Texture2D(tileSizeX, tileSizeY * paletteSize);

        Color[] colors = paletteTexture.GetPixels();
        for (int s = 0; s < paletteSize; ++s)
        {
            Color baseColor = palette[s];
            for (int i = 0; i < tileSizeX; ++i)
            {
                for (int j = 0; j < tileSizeY; ++j)
                {
                    colors[(s * tileSizeX + i) * tileSizeY + j] = baseColor + 3 * 
                        new Color(UnityEngine.Random.Range(0.0f, .3f), 
                                  UnityEngine.Random.Range(0.0f, .3f), 
                                  UnityEngine.Random.Range(0.0f, .3f)
                        );
                }
            }
        }
        paletteTexture.SetPixels(colors);
        paletteTexture.Apply(false);
    }
    
    public override void Spawn(World world, Entity entity, Vector2 position)
    {
        EntityComponent entityComponent = EntityComponent.SpawnEntityControllerInWorld(this, entity, position);
        entity.Connect(entityComponent);
    }

    public override void InstallAt(World world, EntityBuilding entity, World.Coord coord)
    {
        //EntityComponent entityComponent = EntityComponent.SpawnEntityControllerInWorld(this, entity, coord);
        //entity.Connect(entityComponent);
    }

    public static World.Coord MouseToCoordinate(Vector3 vector)
    {
        return Camera.main.ScreenToWorldPoint(vector) + new Vector3(0.5f, 0.5f, 0);
    }
    public static Vector2 MouseToVector2(Vector3 vector)
    {
        return Camera.main.ScreenToWorldPoint(vector);
    }


    public override void WorldCreated(World world)
    {
        base.WorldCreated(world);

        // Set main texture (world)
        Texture2D worldTexture = new Texture2D(tileSizeX * (sizeX - 1), tileSizeY * (sizeY - 1));
        meshRenderer.material.mainTexture = worldTexture;
        meshRenderer.material.mainTexture.filterMode = FilterMode.Point;

        int paletteSize = palette.Length;
        for (int i = 0; i < sizeX - 1; ++i)
        {
            for (int j = 0; j < sizeY - 1; ++j)
            {
                worldTexture.SetPixels(i * tileSizeX, j * tileSizeY, tileSizeX, tileSizeY, GetPalettePixels(UnityEngine.Random.Range(0, paletteSize)));
            }
        }
        worldTexture.Apply(false);
    }
    
    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 pos = MouseToVector2(Input.mousePosition);

            RaycastHit2D ray = Physics2D.Raycast(pos, Vector3.forward);
            Debug.DrawRay(pos, Vector3.forward, Color.red, 10.0f);


            if(ray.collider != null && ray.collider.transform.GetComponent<EntityComponent>() != null)
            {
                Debug.Log(ray.collider.transform.gameObject.name);
                EntityComponent selectedComponent = ray.collider.transform.GetComponent<EntityComponent>();

                world.Selected = selectedComponent.Emitter;
            }
        }
    }
}
