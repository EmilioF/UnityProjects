using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour {

    public Vector2 Position;
    public bool visited;
    public List<Transform> Neighbours;
    public int gCost;
    public int hCost;
    public bool hasCamera;
    public Vector3 cameraDir;
    public GameObject securityCamera;
    public bool isVisible;
    public Room parentRoom;
    public enum Kind {Room, Hall};
    public Kind kind;
    public Cell parent;
    public int camerasWatching;
	// Use this for initialization
	void Start () {
        visited = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void updateKind(Kind pKind)
    {
        kind = pKind;
    }

    public void paintFloor(Color color)
    {
        foreach (Transform child in this.transform)
        {
            if (child.name == "Cube")
                child.GetComponent<Renderer>().material.color = color;
        }
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
