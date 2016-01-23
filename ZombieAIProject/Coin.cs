using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//GameObject.Find ("Engine").GetComponent<Engine> ().coinArray.Add (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnTriggerEnter(){
		GameObject.Find ("Engine").GetComponent<Engine> ().coinArray.Remove (gameObject);
		Destroy (gameObject);
	}
}
