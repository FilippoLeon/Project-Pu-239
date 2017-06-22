using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class SpriteLoader
{
    static string folder = "Sprites";

    static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

    static public Vector2 defaultSize = new Vector2(32, 32);
    static public Vector2 defaultPivot = new Vector2(0.5f, 0.5f);

    static public Texture2D placeholderTexture;
    static public Sprite placeHolder;

    static public int pixelPerUnit = 32;

    static public int multiplierX, multiplierY;

    static public Dictionary<string, Dictionary<string, Sprite>> categories =
        new Dictionary<string, Dictionary<string, Sprite>>();

    public class SpriteData
    {
        public string name;
        public Rect rect;
        public Vector2 pivot;

        public SpriteData() { }
        public SpriteData(Rect rect, Vector2 pivot)
        {
            this.rect = rect;
            this.pivot = pivot;
        }

    }

    static public void Load()
    {
        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, folder));
        FileInfo[] fileInfo = dir.GetFiles("*.png");
        foreach (FileInfo file in fileInfo)
        {
            LoadSpriteSheet(file);
        }

        if (placeHolder != null) return;
        int sizeX = 32, sizeY = 32;
        placeholderTexture = new Texture2D(sizeX, sizeY, TextureFormat.RGBA32, false);
        placeholderTexture.filterMode = FilterMode.Point;

        Color[] colors = new Color[] {
            new Color(247f / 255f, 152f / 255f, 19f/255f),
            new Color(247f/255f, 19f/255f, 98f/255f)
        };
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                int color = 0;
                if ((i / 16 + j / 16) % 2 == 0) color = 1;
                placeholderTexture.SetPixel(i, j, colors[color]);
            }

        }
        placeholderTexture.Apply();

        placeHolder = Sprite.Create(placeholderTexture,
            new Rect(0f, 0f, sizeX, sizeY),
            new Vector2(0.5f, 0.5f), pixelPerUnit);
    }

    /// <summary>
    /// Look up for sprite categories, return wanted sprite if available.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    static public Sprite TryLoadSprite(string category, string id)
    {
        if (categories.ContainsKey(category) && categories[category].ContainsKey(id))
            return categories[category][id];
        return null;
    }

    static private void LoadSpriteSheet(FileInfo path)
    {
        string[] split = path.FullName.Split('.');
        string pathXml = string.Join(".", split, 0, split.Length - 1) + ".xml";
        string fileName = path.Name.Split('.')[0];
        //File.Name

        Debug.Log(string.Format("Loading \"{0}\" ({1})...", path, fileName));

        byte[] imageData = File.ReadAllBytes(path.FullName);

        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);

        if (tex.LoadImage(imageData))
        {
            tex.filterMode = FilterMode.Point;
        }
        else
        {
            Debug.LogError("Cannot load texture!");
        }
        Debug.Assert(tex != null, "Texture can't be null, invalid sheet map?");
        
        if (File.Exists(pathXml))
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(pathXml, settings);

            LoadSprites(reader, tex);
        }
        else
        {
            SpriteData data = new SpriteData(new Rect(Vector2.zero, defaultSize), defaultPivot);
            //data.name = name;
            sprites[data.name] = Sprite.Create(tex, data.rect, data.pivot, pixelPerUnit);
            return;
        }
    }

    static public void LoadSprites(XmlReader reader, Texture2D tex)
    {
        reader.MoveToContent();

        Vector2 dP = defaultPivot;
        Vector2 dS = defaultSize;
        if (reader.GetAttribute("defaultSize") != null)
        {
            string[] size = reader.GetAttribute("defaultSize").Split(',');
            dS = new Vector2(Convert.ToSingle(size[0]), Convert.ToSingle(size[1]));
        }
        multiplierX = (int) dS.x;
        multiplierY = (int) dS.y;

        if (reader.GetAttribute("multiplier") != null)
        {
            string[] size = reader.GetAttribute("multiplier").Split(',');
            multiplierX = (int) Convert.ToSingle(size[0]);
            multiplierY = (int) Convert.ToSingle(size[1]);
        }
        if (reader.GetAttribute("defaultPivot") != null)
        {
            string[] pivot = reader.GetAttribute("defaultPivot").Split(',');
            dP = new Vector2(Convert.ToSingle(pivot[0]), Convert.ToSingle(pivot[1]));
        }

        while (reader.Read())
        {
            XmlNodeType nodeType = reader.NodeType;
            switch (nodeType)
            {
                case XmlNodeType.Element:
                    Debug.Assert(reader.Name == "Sprite");
                    Vector2 pos;
                    if (reader.GetAttribute("start") != null)
                    {
                        string[] start = reader.GetAttribute("start").Split(',');
                        pos = new Vector2(Convert.ToSingle(start[0]) * multiplierX, Convert.ToSingle(start[1]) * multiplierY);
                    }
                    else
                    {
                        pos = new Vector2(0, 0);
                    }
                    Vector2 dS1 = dS;
                    if (reader.GetAttribute("size") != null)
                    {
                        string[] size = reader.GetAttribute("size").Split(',');
                        dS1 = new Vector2(Convert.ToSingle(size[0]), Convert.ToSingle(size[1]));
                    }
                    Vector2 dP1 = dP;
                    if (reader.GetAttribute("pivot") != null)
                    {
                        string[] pivot = reader.GetAttribute("pivot").Split(',');
                        dP1 = new Vector2(Convert.ToSingle(pivot[0]), Convert.ToSingle(pivot[1]));
                    }
                    string name = reader.ReadInnerXml();

                    Debug.Assert(tex != null);
                    sprites[name] = Sprite.Create(tex, new Rect(pos, dS1), dP1, pixelPerUnit);
                    if (reader.GetAttribute("category") != null)
                    {
                        string cat = reader.GetAttribute("category");
                        if (!categories.ContainsKey(cat)) categories[cat] = new Dictionary<string, Sprite>();
                        categories[cat][name] = sprites[name];
                    }
                    Debug.Assert(sprites[name] != null);
                    break;
                default:
                    break;
            }
        }

    }

    public static Sprite GetSprite(string name)
    {
        if (sprites.ContainsKey(name) && sprites[name] != null) return sprites[name];
        else return placeHolder;
    }

    private static string BitArrayToString(BitArray neighbours)
    {
        Func<bool, int> ToInt = (bool b) => { return b ? 1 : 0; };

        string ret = "_" + ToInt(neighbours[0]) + ToInt(neighbours[1]) + ToInt(neighbours[2]) + ToInt(neighbours[3]);
        ret += "_";
        if (neighbours[0] && neighbours[1]) ret += ToInt(neighbours[4]);
        else ret += 0;
        if (neighbours[1] && neighbours[2]) ret += ToInt(neighbours[5]);
        else ret += 0;
        if (neighbours[2] && neighbours[3]) ret += ToInt(neighbours[6]);
        else ret += 0;
        if (neighbours[3] && neighbours[0]) ret += ToInt(neighbours[7]);
        else ret += 0;

        return ret;
    }

    public static Sprite GetSprite(string name, BitArray neighbours)
    {
        name += BitArrayToString(neighbours);
        Debug.Log(String.Format("Loading '{0}'", name));
        if (sprites.ContainsKey(name) && sprites[name] != null) return sprites[name];
        else return placeHolder;
    }
}

/// CODE FOR PLACEHOLDER TEXTURE
//// Placeholder texture
//Texture2D placeHolderTexture = new Texture2D(tileSizeX, tileSizeY);
//renderer.material.mainTexture = placeHolderTexture;
//        renderer.material.mainTexture.filterMode = FilterMode.Point;
        
//        Color[] colors = placeHolderTexture.GetPixels();
//Color color1 = new Color(0.2f, 0.3f, 0.9f);
//Color color2 = new Color(0.4f, 0.8f, 0.4f);
//        for (int i = 0; i<tileSizeX; ++i)
//        {
//            for (int j = 0; j<tileSizeY; ++j)
//            {
//                bool flag = 2 * i < tileSizeX && 2 * j < tileSizeY || 2 * i >= tileSizeX && 2 * j >= tileSizeY;
//colors[tileSizeY * i + j] = flag? color1 : color2;
//            }
//        }
//        placeHolderTexture.SetPixels(colors);
//        placeHolderTexture.Apply(false);