using UnityEngine;
using System.Collections;

public class Phone : Zombie {

	public float engineSpeed;
	public float speed;

	public bool running;

    //public int lane;
	
	public Color color;
	
	Ray rayF;
	Ray rayR;
	Ray rayL;
	Ray rayRA;
	Ray rayLA;

	public float direction;

	RaycastHit hit;
	
	float posX;
	float posZ;
	// Use this for initialization
	void Start () {
		GameObject.Find ("Engine").GetComponent<Engine> ().Zombies.Add (gameObject);
		InvokeRepeating ("changeLanes", .5f, 2.1f);
		InvokeRepeating ("checkPhoneHelp", .8f, .8f);
		InvokeRepeating ("updateDirection", .5f, .7f);
		InvokeRepeating ("updateSpeed", .5f, .3f);

		engineSpeed = GameObject.Find ("Engine").GetComponent<Engine> ().speed;

		running = false;
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

		if(!running)

			gameObject.transform.Translate (transform.forward * speed * Time.deltaTime, Space.World);
		
		if (Physics.Raycast (rayF, out hit, 5.0f)) {
			print(hit.collider.name);
			if(hit.distance < 1f){
				speed = 0;
			}
			else
				speed = Mathf.Min(hit.distance, speed);
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
	/*
	void checkRotate()
	{
		
		if (lane == 1) {
			if (30 - Mathf.Abs (posX) <= 0.01 && 18 - Mathf.Abs (posZ) <= .01) {
				GameObject newZombie = (GameObject)Instantiate(gameObject, transform.position + transform.right*direction, Quaternion.Euler(transform.localEulerAngles.x,transform.localEulerAngles.y + 90 *direction,transform.localEulerAngles.z));
				newZombie.name = gameObject.name;
				newZombie.GetComponent<Phone>().direction = direction;
				GameObject.Destroy(gameObject);
			}
		}
		if (lane == 2) {
			if (28 - Mathf.Abs (posX) <= 0.01 && 16 - Mathf.Abs (posZ) <= 0.01) {
				GameObject newZombie = (GameObject)Instantiate(gameObject, transform.position + transform.right*direction, Quaternion.Euler(transform.localEulerAngles.x,transform.localEulerAngles.y + 90 *direction,transform.localEulerAngles.z));
				newZombie.name = gameObject.name;
				newZombie.GetComponent<Phone>().direction = this.direction;
				GameObject.Destroy(gameObject);
			}
		}
		if (lane == 3) {
			if (26 - Mathf.Abs (posX) <= 0.01 && 14 - Mathf.Abs (posZ) <= 0.01) {
				GameObject newZombie = (GameObject)Instantiate(gameObject, transform.position + transform.right*direction, Quaternion.Euler(transform.localEulerAngles.x,transform.localEulerAngles.y + 90 *direction,transform.localEulerAngles.z));
				newZombie.name = gameObject.name;
				newZombie.GetComponent<Phone>().direction = this.direction;
				GameObject.Destroy(gameObject);
			}
		}
	}*/

    void checkRotate()
    {

        GameObject nextZombie;

        if (Random.value <= GameObject.Find("Engine").GetComponent<Engine>().spawnProbability)
            nextZombie = GameObject.Find("Engine").GetComponent<Engine>().returnZombie();
        else
            nextZombie = gameObject;

        float angle;
        Vector3 right;

        if (direction < 0)
        {
            angle = 180;
            right = -transform.forward;
        }
        else
        {
            angle = 90;
            right = transform.right;
        }
        if (lane == 1)
        {
            if (30 - Mathf.Abs(posX) <= 0.01 && 18 - Mathf.Abs(posZ) <= .01)
            {
                GameObject newZombie = (GameObject)Instantiate(nextZombie, transform.position + right, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + angle, transform.localEulerAngles.z));
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
                GameObject newZombie = (GameObject)Instantiate(nextZombie, transform.position + right, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + angle, transform.localEulerAngles.z));
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
                GameObject newZombie = (GameObject)Instantiate(nextZombie, transform.position + right, Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + angle, transform.localEulerAngles.z));
                newZombie.name = nextZombie.name;
                newZombie.GetComponent<Zombie>().lane = this.lane;
                GameObject.Find("Engine").GetComponent<Engine>().Zombies.Remove(gameObject);
                GameObject.Destroy(gameObject);
            }
        }
    }
	
	void changeLanes()
	{
		if (!running) {
			if (Random.value <= 0.6f) {
				if (!Physics.Raycast (rayL, out hit, 2.0f) && !Physics.Raycast (rayR, out hit, 2.0f) && !Physics.Raycast (rayRA, out hit, 2.0f) && !Physics.Raycast (rayLA, out hit, 2.0f)) {
					if (!inCorner ()) {
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
							if (rand < 0.5f) {
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
							} else {
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
		}
	}
	
	bool inCorner(){
		if (lane == 1) 
			return (30 - Mathf.Abs (posX) <= 3 && 18 - Mathf.Abs (posZ) <= 3);
		
		
		if (lane == 2) 
			return (28 - Mathf.Abs (posX) <= 3 && 16 - Mathf.Abs (posZ) <= 3);
		
		
		
		if (lane == 3) 
			return (26 - Mathf.Abs (posX) <= 3 && 14 - Mathf.Abs (posZ) <= 3);
		
		return false;
	}
	

	void updateSpeed(){
		float ran = Random.Range(engineSpeed/2,engineSpeed*2);
		if (running)
			speed = 0;
		else
			speed = ran;
		//print (speed);
	}

	void updateDirection(){
		if (!running) {
			if (Random.value < 0.2f) {
				direction = direction * -1;
				transform.Rotate (new Vector3 (0, 180, 0));
			}
		}
	}

	IEnumerator checkPhone(){
		float sec = Random.Range (1, 4);
		running = true;
		yield return new WaitForSeconds (sec);
		running = false;

	}

	void checkPhoneHelp(){
		if (Random.value < 0.4f ) {
			StartCoroutine(checkPhone());
		}
	}
}
