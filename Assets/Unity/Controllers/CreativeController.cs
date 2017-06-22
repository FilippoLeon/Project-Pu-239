using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeController : MonoBehaviour {

    public WorldController worldController;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire2"))
        {
            World.Coord coord = WorldComponent.MouseToCoordinate(Input.mousePosition);
            worldController.World.InstallAt(EntityRegistry.InstantiateEntityBuilding("wall"), coord);
        }
	}
}
