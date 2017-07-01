using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public Canvas canvas;
    Image background;

    public WorldController worldController;

    void Start()
    {
        canvas = FindObjectOfType(typeof(Canvas)) as Canvas;

        GameObject panel = new GameObject();
        panel.transform.SetParent(canvas.transform);
        panel.AddComponent<CanvasRenderer>();
        background = panel.AddComponent<Image>();
        background.type = Image.Type.Sliced;
        background.sprite = SpriteLoader.TryLoadSprite("asd", "asd");
    }

    private void Update()
    {

        //background.sprite = SpriteLoader.TryLoadSprite("asd", "asd");
    }
}
