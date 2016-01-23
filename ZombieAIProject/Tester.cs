using UnityEngine;
using System.Collections;

public class Tester : MonoBehaviour {

    public GameObject grid;
    public int numberOfTries;
    public int succes;
    public int failure;

	// Use this for initialization
	void Start () {
        runTests(10);
	}
	
	// Update is called once per frame
	void Update () {

        //for (int i = 0; i < numberOfTries; i++)
        //{
        //    Instantiate(grid);
        //}
	}

    void runTests(int tries)
    {
        for (int i = 0; i < tries; i++)
        {
            GameObject gridObj = (GameObject)Instantiate(grid);
            gridObj.name = "GridGenerator";
            GridGenerator gridGen = gridObj.GetComponent<GridGenerator>();
            gridGen.aStar();
            numberOfTries++;
        }
    }
}
