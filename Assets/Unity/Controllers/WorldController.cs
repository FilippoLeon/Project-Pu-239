﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    private static World world;

    public bool Paused
    {
        get
        {
            return paused;
        }
        set
        {
            paused = value;
        }
    }

    public World World
    {
        get
        {
            return world;
        }

        set
        {
            world = value;
        }
    }

    private bool paused = true;
    private int speed = 1;
    private int elapsed = 0;

    internal object GetWorld()
    {
        throw new NotImplementedException();
    }

    private int elapsedLong = 0;
    private int elapsedLongLong = 0;
    static private int ticLength = 4;
    static private int longTicLength = ticLength * 60;
    static private int longLongTicLength = longLongTicLength * 5;

    public MouseAndKeyboardController inputController;
    public CameraController cameraController;
    public CreativeController creativeController;

    public static int[] speedRatio = new int[] { 1, 2, 4 };
    private static int maxSpeed = speedRatio.Length - 1;
    private static int minSpeed = 0;

    internal void IncreaseSpeed()
    {
        speed = Math.Min(speed + 1, maxSpeed);
    }

    internal void DecreaseSpeed()
    {
        speed = Math.Max(speed - 1, minSpeed);
    }
    internal void CycleSpeed()
    {
        if (speed == maxSpeed) speed = minSpeed;
        else speed = Math.Min(speed + 1, maxSpeed);
    }

    internal void SetSpeed(int newSpeed)
    {
        speed = Math.Min(Math.Max(newSpeed, minSpeed), maxSpeed);
    }
    
    void PreInit()
    {
        new EntityRegistry();

        inputController = gameObject.AddComponent<MouseAndKeyboardController>();
        inputController.worldController = this;
        creativeController = gameObject.AddComponent<CreativeController>();
        creativeController.worldController = this;

        cameraController = gameObject.GetComponent<CameraController>();
        cameraController.worldController = this;

        SpriteLoader.Load();
    }

	// Use this for initialization
	void Start () {
        PreInit();

        world = new World(100, 100);

        GameObject worldObject = new GameObject();
        WorldComponent worldComponent = worldObject.AddComponent<WorldComponent>();

        world.Connect(worldComponent);

        world.Generate();

        cameraController.Center();
	}
	
	// Update is called once per frame
	void Update () {
        if (paused) return;
        elapsed += 1;
        elapsedLong += 1;
        elapsedLongLong += 1;
        if (elapsed >= ticLength / speedRatio[speed])
        {
            world.Tic();
            if(elapsedLong >= longTicLength / speedRatio[speed])
            {
                world.TicLong();
                if (elapsedLongLong >= longLongTicLength / speedRatio[speed])
                {
                    world.TicLongLong();
                    elapsedLongLong = 0;
                }
                elapsedLong = 0;
            }
            elapsed = 0;
        }
	}
}
