using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldComponent : MonoBehaviour, IWorldListener {

    int tileSizeX = 32;
    int tileSizeY = 32;
    Texture2D paletteTexture;

    int sizeX;
    int sizeY;

    Color[] palette;
    MeshRenderer meshRenderer;
    Mesh mesh;
    private World world;

    public Emitter Emitter
    {
        get
        {
            return world;
        }

        set
        {
            world = (World) value;
        }
    }

    void Awake () {
        gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

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

    Color[] GetPalettePixels(int region)
    {
        return paletteTexture.GetPixels(0, region * tileSizeY, tileSizeX, tileSizeY);
    }

    public void InstallAt(World world, EntityBuilding entity, World.Coord coord)
    {
        EntityComponent entityComponent = EntityComponent.SpawnEntityControllerInWorld(this, entity, coord);
        entity.Connect(entityComponent);
    }

    public static World.Coord MouseToCoordinate(Vector3 vector)
    {
        return Camera.main.ScreenToWorldPoint(vector) + new Vector3(0.5f, 0.5f, 0);
    }

    public void WorldCreated(World world)
    {

        this.world = world;
        sizeX = world.sizeX + 1;
        sizeY = world.sizeY + 1;

        float deltaX = 0.5f;
        float deltaY = 0.5f;

        Vector3[] vertices = new Vector3[sizeX * sizeY];
        int I = 0;
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                // Move mesh to have centers at integer coordinates
                vertices[I++] = new Vector3(i - deltaX, j - deltaY, 0);
            }
        }

        I = 0;
        Vector2[] uv = new Vector2[sizeX * sizeY];
        for (int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                uv[I++] = new Vector2(i / (float)(sizeX - 1), j / (float)(sizeY - 1));
            }
        }

        int[] triangles = new int[((sizeX - 1) * sizeY - 1) * 6];
        for (int J = 0; J < (sizeX - 1) * sizeY - 1; ++J)
        {
            int J6 = 6 * J;
            triangles[J6 + 0] = J;
            triangles[J6 + 1] = J + 1;
            triangles[J6 + 2] = J + sizeY + 1;
            triangles[J6 + 3] = J;
            triangles[J6 + 4] = J + sizeY + 1;
            triangles[J6 + 5] = J + sizeY;
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

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

    public void Event(string signal, object[] args)
    {

    }
}
