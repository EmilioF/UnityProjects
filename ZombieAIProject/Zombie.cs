using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour {

    public int lane;

    public GameObject classic;
    public GameObject shambler;
    public GameObject modern;
    public GameObject phone;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
  
	}

    public void drawLines()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 16, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.right * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position - transform.forward * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position - transform.right * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position + (transform.right - transform.forward) * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position + (-transform.right - transform.forward) * 2, Color.red);

        Debug.DrawLine(transform.position, transform.position - (transform.right - transform.forward) * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position - (transform.right - 2 * transform.forward) * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position - (transform.right - 4 * transform.forward) * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position - (transform.right - 6 * transform.forward) * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position - (transform.right - 8 * transform.forward) * 2, Color.blue);

        Debug.DrawLine(transform.position, transform.position - (transform.right - transform.forward) * 2, Color.red);
        Debug.DrawLine(transform.position, transform.position + (transform.right + 2 * transform.forward) * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + (transform.right + 4 * transform.forward) * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + (transform.right + 6 * transform.forward) * 2, Color.blue);
        Debug.DrawLine(transform.position, transform.position + (transform.right + 8 * transform.forward) * 2, Color.blue);

    }

}
