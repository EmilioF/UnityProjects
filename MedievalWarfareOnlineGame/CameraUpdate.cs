using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraUpdate : MonoBehaviour
{
	public float Speed;
	private GameObject _lighting;
	private GameObject _mainLight;
	private List<GameObject> _villages;

	void Start()
	{
		_mainLight = GameObject.Find ("Directional light");
		Speed = 100.0f;

		transform.position += new Vector3(170.0f, 0.0f, -100.0f);
		transform.Translate (new Vector3 (0, 0, 150)); 		//Zoom in for now
	}
	void Update ()
	{
		//Lights off?
		if (LightsOn())
			_mainLight.SetActive(true);
		else
			_mainLight.SetActive(false);

		transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (Input.GetKey (KeyCode.E))
		{
			transform.Translate (new Vector3 (0, 0, 1) * Time.deltaTime * Speed, Space.Self);
		}
		if (Input.GetKey (KeyCode.Q))
		{
			transform.Translate (new Vector3 (0, 0, -1) * Time.deltaTime * Speed, Space.Self);
		}
		if (Input.GetKey (KeyCode.Z))
		{
			transform.eulerAngles += new Vector3(0, -1.0f, 0);
		}
		if (Input.GetKey (KeyCode.X))
		{
			transform.eulerAngles += new Vector3(0, 1.0f, 0);
		}
		
		if (Input.GetKey (KeyCode.C) && transform.eulerAngles.x >= 15)
		{
			transform.eulerAngles += new Vector3(-1.0f, 0, 0);
		}
		if (Input.GetKey (KeyCode.V) && transform.eulerAngles.x <= 80)
		{
			transform.eulerAngles += new Vector3(1.0f, 0, 0);
		}

       
	}
	bool LightsOn()
	{
		//Lighting
//		GameObject[] villages = GameObject.FindGameObjectsWithTag("Village");
//		for (int i = 0; i < villages.Length; i++)
//		{	
//			if (villages[i].GetComponent<Village>().Chosen)
//				return false;
//		}
		return true;
	}
}