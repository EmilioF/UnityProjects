using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate (new Vector3 (Input.GetAxis ("Horizontal"), 0, 0) * Time.deltaTime * speed , Space.World);

		transform.Translate (new Vector3 (0, 0, Input.GetAxis ("Vertical")) * Time.deltaTime * speed , Space.World);
		if (Input.GetKey(KeyCode.Q))
			transform.Rotate(Vector3.up * Time.deltaTime * speed *2,Space.World);

		if (Input.GetKey(KeyCode.E))
			transform.Rotate(Vector3.down * Time.deltaTime * speed *2, Space.World);

	}
}
