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

        if (Input.GetButtonDown("Pause"))
        {
            worldController.Paused = !worldController.Paused;
        } else if(Input.GetButtonDown("IncreaseSpeed")) {
            worldController.IncreaseSpeed();
        }
        else if (Input.GetButtonDown("DecreaseSpeed"))
        {
            worldController.DecreaseSpeed();
        }
        else if (Input.GetButtonDown("CycleSpeed"))
        {
            worldController.CycleSpeed();
        }
        else if (Input.GetButtonDown("Speed0"))
        {
            worldController.SetSpeed(0);
        }
        else if (Input.GetButtonDown("Speed1"))
        {
            worldController.SetSpeed(1);
        }
        else if (Input.GetButtonDown("Speed2"))
        {
            worldController.SetSpeed(2);
        }
    }
}
