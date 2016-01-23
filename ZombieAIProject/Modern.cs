using UnityEngine;
using System.Collections;

public class Modern : Zombie {

	public float speed;
    //public int lane;

	public Color color;

	Ray rayF;
	Ray rayR;
	Ray rayL;
	Ray rayRA;
	Ray rayLA;

	RaycastHit hit;
	
	float posX;
	float posZ;
	// Use this for initialization
	void Start () {
		GameObject.Find ("Engine").GetComponent<Engine> ().Zombies.Add (gameObject);
		InvokeRepeating ("changeLanes", 1f, .1f);
		speed = GameObject.Find ("Engine").GetComponent<Engine> ().speed;
	}
	
	// Update is called once per frame
	void Update () {
		posX = transform.position.x;
		posZ = transform.position.z;
		
		rayF = new Ray (transform.position, transform.forward);
		rayR = new Ray (transform.position, transform.right);
		rayL = new Ray (transform.position, -transform.right);
		rayRA = new Ray (transform.position, transform.right - transform.forward);
		rayLA = new Ray (transform.position, -transform.right - transform.forward);


		checkRotate ();
		gameObject.transform.Translate (transform.forward * speed * 2 * Time.deltaTime, Space.World);

		if (Physics.Raycast (rayF, out hit, 10.0f)) {
			if(hit.distance < 1f)
				speed = 0;
			else{
				speed = Mathf.Min(hit.distance, speed);
			}
		}
		else {
			speed = GameObject.Find ("Engine").GetComponent<Engine> ().speed;
		}


        if (Physics.Linecast(transform.position, transform.position + transform.forward * 16, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + transform.right * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - transform.forward * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - transform.right * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + (transform.right - transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + (-transform.right - transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - (transform.right - transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - (transform.right - 2 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - (transform.right - 4 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - (transform.right - 6 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position - (transform.right - 8 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);

        if (Physics.Linecast(transform.position, transform.position - (transform.right - transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + (transform.right + 2 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + (transform.right + 4 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + (transform.right + 6 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if (Physics.Linecast(transform.position, transform.position + (transform.right + 8 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);


	}

    GameObject nextZombie;
    Vector3 newPosition;
    float angle;
    Vector3 right;

    void checkRotate()
    {


        if (lane == 1)
        {
            if (30 - Mathf.Abs(posX) <= 0.01 && 18 - Mathf.Abs(posZ) <= .01)
            {
                setCorner(posX, posZ);
                GameObject newZombie = (GameObject)Instantiate(nextZombie, newPosition, Quaternion.Euler(transform.localEulerAngles.x, angle, transform.localEulerAngles.z));
                newZombie.name = nextZombie.name;
                newZombie.GetComponent<Zombie>().lane = this.lane;
                GameObject.Find("Engine").GetComponent<Engine>().Zombies.Remove(gameObject);
                GameObject.Destroy(gameObject);
            }
        }
        if (lane == 2)
        {
            if (28 - Mathf.Abs(posX) <= 0.01 && 16 - Mathf.Abs(posZ) <= 0.01)
            {
                setCorner(posX, posZ);
                GameObject newZombie = (GameObject)Instantiate(nextZombie, newPosition, Quaternion.Euler(transform.localEulerAngles.x, angle, transform.localEulerAngles.z));
                newZombie.name = nextZombie.name;
                newZombie.GetComponent<Zombie>().lane = this.lane;
                GameObject.Find("Engine").GetComponent<Engine>().Zombies.Remove(gameObject);
                GameObject.Destroy(gameObject);
            }
        }
        if (lane == 3)
        {
            if (26 - Mathf.Abs(posX) <= 0.01 && 14 - Mathf.Abs(posZ) <= 0.01)
            {
                setCorner(posX, posZ);
                GameObject newZombie = (GameObject)Instantiate(nextZombie, newPosition, Quaternion.Euler(transform.localEulerAngles.x, angle, transform.localEulerAngles.z));
                newZombie.name = nextZombie.name;
                newZombie.GetComponent<Zombie>().lane = this.lane;
                GameObject.Find("Engine").GetComponent<Engine>().Zombies.Remove(gameObject);
                GameObject.Destroy(gameObject);
            }
        }
    }
	/*
	void checkRotate()
	{
		
		float rotate = transform.localEulerAngles.y + 90;
		
		if (lane == 1) {
			if (30 - Mathf.Abs (posX) <= .00001 && 18 - Mathf.Abs (posZ) <= .00001){
				transform.localEulerAngles = new Vector3 (0, rotate, 0);
				transform.position = new Vector3 (Mathf.RoundToInt (posX), transform.position.y, Mathf.RoundToInt (posZ));
			}
		}
		if (lane == 2) {
			if (28 - Mathf.Abs (posX) <= .00001 && 16 - Mathf.Abs (posZ) <= .00001){
				transform.localEulerAngles = new Vector3 (0, rotate, 0);
				transform.position = new Vector3 (Mathf.RoundToInt (posX), transform.position.y, Mathf.RoundToInt (posZ));
			}
		}
		if (lane == 3	) {
			if (26 - Mathf.Abs (posX) <= .00001 && 14 - Mathf.Abs (posZ) <= .00001){
				transform.localEulerAngles = new Vector3 (0, rotate, 0);
				transform.position = new Vector3 (Mathf.RoundToInt (posX), transform.position.y, Mathf.RoundToInt (posZ));
			}
		}
		
	}
	*/
	void changeLanes()
	{
		if (Physics.Raycast (rayF, out hit, 18.0f)) {
			if (!Physics.Raycast (rayL, out hit, 2.0f) && !Physics.Raycast (rayR, out hit, 2.0f) && !Physics.Raycast (rayRA, out hit, 2.0f) && !Physics.Raycast (rayLA, out hit, 2.0f)) {
				switch (lane) {
				case 1: 
					if (30 - Mathf.Abs (posX) <= 0.01) {
						if (posX < 0)
							transform.Translate (new Vector3 (2, 0, 0), Space.World);
						else
							transform.Translate (new Vector3 (-2, 0, 0), Space.World);
						lane = 2;
					} else {
						if (posZ < 0)
							transform.Translate (new Vector3 (0, 0, 2), Space.World);
						else
							transform.Translate (new Vector3 (0, 0, -2), Space.World);
						lane = 2;
					}
					break;
				case 2: 
					float rand = Random.value;
					if(rand < 0.5f){
						if (28 - Mathf.Abs (posX) <= 0.01) {
							if (posX < 0)
								transform.Translate (new Vector3 (2, 0, 0), Space.World);
							else
								transform.Translate (new Vector3 (-2, 0, 0), Space.World);
							lane = 3;
						} else {
							if (posZ < 0)
								transform.Translate (new Vector3 (0, 0, 2), Space.World);
							else
								transform.Translate (new Vector3 (0, 0, -2), Space.World);
							lane = 3;
						}
					}
					else{
						if (28 - Mathf.Abs (posX) <= 0.01) {
							if (posX < 0)
								transform.Translate (new Vector3 (-2, 0, 0), Space.World);
							else
								transform.Translate (new Vector3 (2, 0, 0), Space.World);
							lane = 1;
						} else {
							if (posZ < 0)
								transform.Translate (new Vector3 (0, 0, -2), Space.World);
							else
								transform.Translate (new Vector3 (0, 0, 2), Space.World);
							lane = 1;
						}
					}
					break;
			
				case 3: 
					if (26 - Mathf.Abs (posX) <= 0.01) {
						if (posX < 0)
							transform.Translate (new Vector3 (-2, 0, 0), Space.World);
						else
							transform.Translate (new Vector3 (2, 0, 0), Space.World);
						lane = 2;
					} else {
						if (posZ < 0)
							transform.Translate (new Vector3 (0, 0, -2), Space.World);
						else
							transform.Translate (new Vector3 (0, 0, 2), Space.World);
						lane = 2;
					}
					break;
			
				}
			}
		}
	}

	void drawLines(){
		Debug.DrawLine (transform.position, transform.position + transform.forward * 18, color);
		Debug.DrawLine (transform.position, transform.position + transform.right * 2, color);
		Debug.DrawLine (transform.position, transform.position - transform.forward * 2, color);
		Debug.DrawLine (transform.position, transform.position - transform.right * 2, color);
		Debug.DrawLine (transform.position, transform.position + (transform.right - transform.forward) * 2, color);
		Debug.DrawLine (transform.position, transform.position + (-transform.right - transform.forward) * 2, color);
	}

    void setCorner(float X, float Z)
    {
        if (Random.value <= GameObject.Find("Engine").GetComponent<Engine>().spawnProbability)
        {
            int rand = Random.Range(1, 5);
            nextZombie = GameObject.Find("Engine").GetComponent<Engine>().returnZombie();

            X = Mathf.Abs(X);
            Z = Mathf.Abs(Z);

            if (rand == 1)
            {
                newPosition = new Vector3(Mathf.RoundToInt(X), transform.position.y, Mathf.RoundToInt(Z) - 0.2f);
                angle = 180;
            }

            else if (rand == 2)
            {
                newPosition = new Vector3(-Mathf.RoundToInt(X) + 0.2f, transform.position.y, Mathf.RoundToInt(Z));
                angle = 90;
            }

            else if (rand == 3)
            {
                newPosition = new Vector3(-Mathf.RoundToInt(X), transform.position.y, -Mathf.RoundToInt(Z) + 0.2f);
                angle = 0;
            }

            else
            {
                newPosition = new Vector3(Mathf.RoundToInt(X) - 0.2f, transform.position.y, -Mathf.RoundToInt(Z));
                angle = 270;
            }

        }

        else
        {
            nextZombie = gameObject;
            newPosition = transform.position + transform.right;
            angle = transform.localEulerAngles.y + 90;
        }
    }
}
