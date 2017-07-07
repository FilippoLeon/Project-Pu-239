using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAndKeyboardController : MonoBehaviour {

    public WorldController worldController;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        World world = worldController.World;
        if (Input.GetButtonDown("Pause"))
        {
            world.Paused = !world.Paused;
        } else if(Input.GetButtonDown("IncreaseSpeed")) {
            world.IncreaseSpeed();
        }
        else if (Input.GetButtonDown("DecreaseSpeed"))
        {
            world.DecreaseSpeed();
        }
        else if (Input.GetButtonDown("CycleSpeed"))
        {
            world.CycleSpeed();
        }
        else if (Input.GetButtonDown("Speed0"))
        {
            world.SetSpeed(0);
        }
        else if (Input.GetButtonDown("Speed1"))
        {
            world.SetSpeed(1);
        }
        else if (Input.GetButtonDown("Speed2"))
        {
            world.SetSpeed(2);
        }
    }
}
