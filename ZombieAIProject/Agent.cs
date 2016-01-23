using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

    public Light flashlight;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
            flashlight.enabled = !flashlight.enabled;
	}
}
