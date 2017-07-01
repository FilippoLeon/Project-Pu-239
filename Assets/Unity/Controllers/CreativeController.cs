using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeController : MonoBehaviour {

    public WorldController worldController;

    // Use this for initialization
    void Start () {
		
	}
	
    public enum CreativeTool
    {
        PlaceWall,
        PlaceDoor,
        SpawnCharacter,
        MoveSelected,
        Despawn,
    }

    public CreativeTool creativeTool;

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Next"))
        {
            NextTool();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            World.Coord cursorCoord = WorldComponent.MouseToCoordinate(Input.mousePosition);
            Vector2 cursorPosition = WorldComponent.MouseToVector2(Input.mousePosition);
            switch(creativeTool)
            {
                case CreativeTool.PlaceWall:
                    worldController.World.InstallAt(EntityRegistry.InstantiateEntityBuilding("wall"), cursorCoord);
                    break;
                case CreativeTool.PlaceDoor:
                    worldController.World.InstallAt(EntityRegistry.InstantiateEntityBuilding("door"), cursorCoord);
                    break;
                case CreativeTool.SpawnCharacter:
                    worldController.World.Spawn(EntityRegistry.InstantiateCharacter(), cursorPosition);
                    break;
                case CreativeTool.MoveSelected:
                    EntityAnimated entityAnimated = worldController.World.GetCharacter("Jim");
                    entityAnimated.StartMovingTo(cursorPosition);

                    GameObject go = new GameObject();
                    LineRenderer lr = go.AddComponent<LineRenderer>();

                    LinkedList<Tile> path = entityAnimated.path;
                    if(path == null) {
                        break;
                    }

                    Vector3[] pos = new Vector3[path.Count];
                    int I = 0;
                    foreach(Tile tile in path)
                    {
                        //Debug.Log(tile.coord.ToVector2());
                        pos[I++] = (Vector3) tile.coord.ToVector2() - 0.02f*Vector3.forward;
                    }
                    lr.numPositions = path.Count;
                    lr.widthMultiplier = 0.05f;
                    lr.SetPositions(pos);
                    lr.material = new Material(Shader.Find("Diffuse"));


                    break;
                case CreativeTool.Despawn:
                    worldController.World.Uninstall(cursorCoord);
                    break;
                default:
                    break;
            }

        }
	}

    void NextTool()
    {
        creativeTool += 1;
        Debug.LogWarning(String.Format("Next tool '{0}'.", creativeTool));
        if ((int) creativeTool == Enum.GetNames(typeof(CreativeTool)).Length) {
            creativeTool = (CreativeTool) Enum.GetValues(typeof(CreativeTool)).GetValue(0);
        }
        ////creativeTool %= Enum.GetNames(typeof(CreativeTool)).Length;
    }
}
