using UnityEngine;
using System.Collections;

public class Classic : Zombie {

	public float speed;
    //public int lane;

	public Color color;

	Ray ray;
	RaycastHit hit;

	float posX;
	float posZ;
	// Use this for initialization
	void Start () {
		GameObject.Find ("Engine").GetComponent<Engine> ().Zombies.Add (gameObject);
		speed = GameObject.Find ("Engine").GetComponent<Engine> ().speed;
	}
	
	// Update is called once per frame
	void Update () {
		posX = transform.position.x;
		posZ = transform.position.z;

		ray = new Ray (transform.position, transform.forward);

		//checkCorner ();
		checkRotate ();
		gameObject.transform.Translate (transform.forward * speed * Time.deltaTime, Space.World);

		if (Physics.Raycast (ray, out hit, 5.0f)) {
			if(hit.distance < 1f)
				speed = 0;
			else{
				speed = Mathf.Min(hit.distance, speed);
			}

		}
		else {
			speed = GameObject.Find ("Engine").GetComponent<Engine> ().speed;
		}

        if( Physics.Linecast(transform.position, transform.position + transform.forward * 16, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + transform.right * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - transform.forward * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - transform.right * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + (transform.right - transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + (-transform.right - transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - (transform.right - transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - (transform.right - 2 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - (transform.right - 4 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - (transform.right - 6 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position - (transform.right - 8 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);

        if( Physics.Linecast(transform.position, transform.position - (transform.right - transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + (transform.right + 2 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + (transform.right + 4 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + (transform.right + 6 * transform.forward) * 2, out hit))
            if(hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
        if( Physics.Linecast(transform.position, transform.position + (transform.right + 8 * transform.forward) * 2, out hit))
            if (hit.collider.name == "Player")
                DestroyObject(hit.collider.gameObject);
  

	}

    //void checkRotate()
    //{

    //    float rotate = transform.localEulerAngles.y + 90;

    //    if (lane == 1) {
    //        if (30 - Mathf.Abs (posX) <= 0.01 && 18 - Mathf.Abs (posZ) <= .01){
    //            transform.localEulerAngles = new Vector3 (0, rotate, 0);
    //            transform.position = new Vector3 (Mathf.RoundToInt (posX), transform.position.y, Mathf.RoundToInt (posZ));
    //        }
    //    }
    //    if (lane == 2) {
    //        if (28 - Mathf.Abs (posX) <= 0.01 && 16 - Mathf.Abs (posZ) <= 0.01){
    //            transform.localEulerAngles = new Vector3 (0, rotate, 0);
    //            transform.position = new Vector3 (Mathf.RoundToInt (posX), transform.position.y, Mathf.RoundToInt (posZ));
    //        }
    //    }
    //    if (lane == 3	) {
    //        if (26 - Mathf.Abs (posX) <= 0.01 && 14 - Mathf.Abs (posZ) <= 0.01){
    //            transform.localEulerAngles = new Vector3 (0, rotate, 0);
    //            transform.position = new Vector3 (Mathf.RoundToInt (posX), transform.position.y, Mathf.RoundToInt (posZ));
    //        }
    //    }

    //}
    GameObject nextZombie;
    Vector3 newPosition;
    float angle;
    Vector3 right;

	void checkRotate (){


        if (lane == 1)
        {
            if (30 - Mathf.Abs(posX) <= 0.01 && 18 - Mathf.Abs(posZ) <= .01)
            {
                setCorner(posX,posZ);
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

	void changeLanes()
	{
		switch (lane) {
		case 1: 
			if( 30 - Mathf.Abs (posX) <= 0.01){
					if(posX < 0)
						transform.Translate(new Vector3(2,0,0),Space.World);
					else
						transform.Translate(new Vector3(-2,0,0),Space.World);
				lane = 2;
				}
			else{
				if(posZ < 0)
					transform.Translate(new Vector3(0,0,2),Space.World);
				else
					transform.Translate(new Vector3(0,0,-2),Space.World);
				lane = 2;
			}
				break;
		case 2: 
			if( 28 - Mathf.Abs (posX) <= 0.01){
				if(posX < 0)
					transform.Translate(new Vector3(2,0,0),Space.World);
				else
					transform.Translate(new Vector3(-2,0,0),Space.World);
				lane = 3;
			}
			else{
				if(posZ < 0)
					transform.Translate(new Vector3(0,0,2),Space.World);
				else
					transform.Translate(new Vector3(0,0,-2),Space.World);
				lane = 3;
			}
			break;

		case 3: 
			if( 26 - Mathf.Abs (posX) <= 0.01){
				if(posX < 0)
					transform.Translate(new Vector3(-2,0,0),Space.World);
				else
					transform.Translate(new Vector3(2,0,0),Space.World);
				lane = 2;
			}
			else{
				if(posZ < 0)
					transform.Translate(new Vector3(0,0,-2),Space.World);
				else
					transform.Translate(new Vector3(0,0,2),Space.World);
				lane = 2;
			}
			break;

		}
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

            else if(rand == 3)
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
