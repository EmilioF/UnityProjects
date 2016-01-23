using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public float speed;
	// Use this for initialization
	void Start () {
        speed = GameObject.Find("GridGenerator").GetComponent<GridGenerator>().gridSize.x;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) *Time.deltaTime * speed, Space.World);
	}
}
