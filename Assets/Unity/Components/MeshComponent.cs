using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshComponent : MonoBehaviour, IWorldListener
{
    protected int tileSizeX = 32;
    protected int tileSizeY = 32;
    protected Texture2D paletteTexture;

    protected int sizeX;
    protected int sizeY;

    protected Color[] palette;
    protected MeshRenderer meshRenderer;
    protected Mesh mesh;
    protected World world;

    protected float zAxis = 0f;

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

    public void Awake()
    {
        gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
    }

    protected Color[] GetPalettePixels(int index)
    {
        return paletteTexture.GetPixels(0, index * tileSizeY, tileSizeX, tileSizeY);
    }
    
    public virtual void WorldCreated(World world)
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
                vertices[I++] = new Vector3(i - deltaX, j - deltaY, zAxis);
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
    }

    public virtual void Event(string signal, object[] args)
    {

    }

    public abstract void Spawn(World world, Entity entity, Vector2 position);
    public abstract void InstallAt(World world, EntityBuilding entity, World.Coord coord);
}
