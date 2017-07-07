using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    private static World world;

    public static World StaticWorld;
    public World World
    {
        get
        {
            return world;
        }

        set
        {
            world = value;
            StaticWorld = world;
        }
    }

    public MouseAndKeyboardController inputController;
    public CameraController cameraController;
    public CreativeController creativeController;
    public GUIController guiController;
    
    void PreInit()
    {
        inputController = gameObject.AddComponent<MouseAndKeyboardController>();
        inputController.worldController = this;
        creativeController = gameObject.AddComponent<CreativeController>();
        creativeController.worldController = this;
        cameraController = gameObject.GetComponent<CameraController>();
        cameraController.worldController = this;

        SpriteLoader.Load();
        new LUA.ScriptLoader();

        guiController = gameObject.AddComponent<GUIController>();
        guiController.worldController = this;
    }

    // Use this for initialization
    void Start() {
        PreInit();

        World = new World(200, 200);
        world.SetRegistry();

        GameObject worldObject = new GameObject();
        WorldComponent worldComponent = worldObject.AddComponent<WorldComponent>();
        GameObject overlayObject = new GameObject();
        OverlayComponent overlayComponent = overlayObject.AddComponent<OverlayComponent>();

        world.Connect(worldComponent);
        world.Connect(overlayComponent);

        world.Generate();

        cameraController.Center();
	}
	
	// Update is called once per frame
	void Update () {
        world.Update();
	}
}
