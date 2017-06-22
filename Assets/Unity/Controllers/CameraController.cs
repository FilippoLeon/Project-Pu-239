using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float maxZoom = 0.1f;
    public float minZoom = 30f;
    public float cameraZ = -0.5f;
    public float zoomSpeed = 0.1f;
    public float panSpeed = 5f;
    public float slideSpeed = 0.5f;

    public WorldController worldController;

    Vector3 grabPosition;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float speedX = Input.GetAxis("Horizontal");
        float speedY = Input.GetAxis("Vertical");
        float zoom = Input.GetAxis("Zoom");
        zoom *= 0.01f * zoom * zoom;

        Camera.main.transform.position += new Vector3(speedX, speedY, 0) * Camera.main.orthographicSize * slideSpeed;
        Camera.main.orthographicSize += zoom * zoomSpeed;

        //// DRAG
        //if(Input.GetButtonDown("Pan"))
        //{
        //    grabPosition = Input.mousePosition;
        //} else if (Input.GetButton("Pan"))
        //{
        //    Camera.main.transform.position += Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        //}

        //// GRAB
        if (Input.GetButton("Pan"))
        {
            Camera.main.transform.position += panSpeed * Camera.main.orthographicSize * Camera.main.ScreenToViewportPoint(grabPosition - Input.mousePosition);
        }
        grabPosition = Input.mousePosition;

        if (worldController == null) return;
        RestoreCameraWithinBounds();
    }

    void RestoreCameraWithinBounds()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = Mathf.Clamp(cameraPos.x, 0, worldController.World.sizeX);
        cameraPos.y = Mathf.Clamp(cameraPos.y, 0, worldController.World.sizeY);
        Camera.main.transform.position = cameraPos;

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, maxZoom, minZoom);
    }

    public void Center()
    {
        if (worldController == null) return;
        World world = worldController.World;
        Camera.main.transform.position = new Vector3(world.sizeX / 2f, world.sizeY / 2f, cameraZ);
    }
}
