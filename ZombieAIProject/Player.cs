using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	NavMeshAgent agent;
	public List<GameObject> coinList;
	public List<GameObject> zombieList;

	public GameObject target;
	public float distance;

	RaycastHit hit;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		coinList = new List<GameObject>();
		zombieList = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		coinList = GameObject.Find ("Engine").GetComponent<Engine> ().coinArray;
		zombieList = GameObject.Find ("Engine").GetComponent<Engine> ().Zombies;

		

        if (coinList.Count > 0)
        {
            distance = Vector3.Distance(transform.position, coinList[0].gameObject.transform.position);

            foreach (GameObject coin in coinList)
            {

                if (distance >= Vector3.Distance(transform.position, coin.transform.position))
                {
                    target = coin;
                    distance = Vector3.Distance(transform.position, coin.transform.position);
                }

            }
            agent.SetDestination(target.transform.position);

        }

        else
            agent.SetDestination(new Vector3(-36, 0, 0));

		foreach (GameObject zombie in zombieList) 
		{
		    if(Physics.Linecast(transform.position,zombie.transform.position,out hit)){
			    if(hit.transform.position == zombie.transform.position)
				    Debug.DrawLine(transform.position, zombie.transform.position,Color.red);
			    }
		}

	}
    void OnBecameInvisible()
    {
        print("OMGWTFBBQ YOU LOST WTF!");
    }
}
