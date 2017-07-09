using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorBar : MonoBehaviour {

    public delegate float GetSize();
    public GetSize GetSizeCallback;

    MeshRenderer meshRenderer;
    Mesh mesh;
    private float zLevel = -0.01f;

    static public IndicatorBar Instantiate(GameObject parent)
    {
        GameObject go = new GameObject();
        go.transform.SetParent(parent.transform);

        go.name = "indicator_bar";

        IndicatorBar jobComponent = go.AddComponent<IndicatorBar>();

        return jobComponent;
    }

    // Use this for initialization
    void Start () {
        gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = new Vector3[] {
            new Vector3(0,0,zLevel),
            new Vector3(1,0,zLevel),
            new Vector3(0,1,zLevel),
            new Vector3(1,1,zLevel)
        };
        mesh.uv = new Vector2[] {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(1,1)
        };
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };

        gameObject.transform.localPosition += new Vector3(-0.6f, -0.5f, 0);
        gameObject.transform.localScale = new Vector3(0.1f, 1, 1);
    }
	
	// Update is called once per frame
	void Update () {
		if(GetSizeCallback != null)
        {
            gameObject.transform.localScale = new Vector3(0.1f, GetSizeCallback(), 1);
        }
	}
}
