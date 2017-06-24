using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class OverlayComponent : MeshComponent
{

    Func<int, int, int> Heatmap;
    Texture2D overlayTexture;

    new void Awake()
    {
        //Heatmap = (i, j) => i + j;

        Heatmap = (i, j) =>
        {
            int id = (Emitter as World).GetTileAt(new World.Coord(i, j)).Room.Id;
            return id == -1 ? 244 : 200 * id;
        };

        palette = File.ReadAllLines(StreamingAssets.ReadFile("Colormaps", "rainbow_bgyrm_35-85_c71_n256.csv")).Select(
            line => {
                string[] lineS = line.Split(',');
                return new Color(float.Parse(lineS[0]), float.Parse(lineS[1]), float.Parse(lineS[2]));
            }
        ).ToArray();

        base.Awake();

        name = "Overlay";
        
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
                    colors[(s * tileSizeX + i) * tileSizeY + j] = baseColor;
                        //+ 3 *
                        //new Color(UnityEngine.Random.Range(0.0f, .3f),
                        //          UnityEngine.Random.Range(0.0f, .3f),
                        //          UnityEngine.Random.Range(0.0f, .3f)
                        //);
                }
            }
        }
        paletteTexture.SetPixels(colors);
        paletteTexture.Apply(false);


    }
    
    public override void Spawn(World world, Entity entity, Vector2 position)
    {

    }

    public override void InstallAt(World world, EntityBuilding entity, World.Coord coord)
    {

    }

    public override void WorldCreated(World world)
    {
        zAxis = -0.01f;

        base.WorldCreated(world);
        
        meshRenderer.material = new Material(Shader.Find("Sprites/Diffuse"));
        Color col = meshRenderer.material.color;
        col.a = 0.2f;
        meshRenderer.material.color = col;

        // Set main texture (world)
        overlayTexture = new Texture2D(tileSizeX * (sizeX - 1), tileSizeY * (sizeY - 1), TextureFormat.ARGB32, false);
        meshRenderer.material.mainTexture = overlayTexture;
        meshRenderer.material.mainTexture.filterMode = FilterMode.Point;


        int paletteSize = palette.Length;
        for (int i = 0; i < sizeX - 1; ++i)
        {
            for (int j = 0; j < sizeY - 1; ++j)
            {
                overlayTexture.SetPixels(i * tileSizeX, j * tileSizeY, tileSizeX, tileSizeY, 
                    GetPalettePixels(Heatmap(i,j))
                    );
            }
        }
        overlayTexture.Apply(false);
        //overlayTexture.set
    }

    /// TODO: probablyx remove this guy
    private void Update()
    {

        int paletteSize = palette.Length;
        for (int i = 0; i < sizeX - 1; ++i)
        {
            for (int j = 0; j < sizeY - 1; ++j)
            {
                overlayTexture.SetPixels(i * tileSizeX, j * tileSizeY, tileSizeX, tileSizeY,
                    GetPalettePixels(Heatmap(i, j) % 255)
                    );
            }
        }
        overlayTexture.Apply(false);
    }
}

