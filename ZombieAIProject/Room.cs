using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour {

    public Vector2 Size;
    public List<Transform> cellList;
    public int size;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        roomSize();
	}

    public void roomSize()
    {
        size = 0;

        foreach (Transform cell in cellList)
        {
            size++;
        }
    }

}
