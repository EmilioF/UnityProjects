using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engine : MonoBehaviour {

	public float speed;
	public float spawnProbability;
	public int numberOfZombies;
	public float ratioEasyToHard;
	public int[] zombieArray;
	public Vector4[] positionArray;

	public GameObject classic;
	public GameObject shambler;
	public GameObject modern;
	public GameObject phone;

	public List<GameObject> coinArray;
	public List<GameObject> Zombies;

	// Use this for initialization
	void Start () {
		zombieArray = new int[numberOfZombies];
		positionArray = new Vector4[numberOfZombies];

		shufflePositions ();

		for (int i = 0; i < zombieArray.Length; i++) 
		{
				zombieArray[i] = Random.Range(1,3);
		}

		for (int i = 0; i < zombieArray.Length; i++) 
		{
			Vector4 position = positionArray[i];

			GameObject newZombie;

			switch(zombieArray[i]){
			case 1: newZombie = (GameObject)Instantiate(classic,new Vector3(position.x,1,position.y),Quaternion.Euler(0,position.w,0));
					newZombie.GetComponent<Zombie>().lane = (int)position.z;
					break;
			case 2: newZombie = (GameObject)Instantiate(shambler,new Vector3(position.x,1,position.y),Quaternion.Euler(0,position.w,0));
					newZombie.GetComponent<Zombie>().lane = (int)position.z;
					break;
			}
		}


	}
	
	// Update is called once per frame
	void Update () {

        //respawn();

	}

	void shufflePositions(){
		for (int i = 0; i < positionArray.Length; i++) {

			int lane = Random.Range (1, 4);
			int side = Random.Range (1, 4);

			switch (lane) {
			case 1:
				switch (side) {
				case 1:
					positionArray [i] = new Vector4 ((int)Random.Range (-14, 14) * 2, -18, lane, 270);
					break;
				case 2:
					positionArray [i] = new Vector4 (-30, (int)Random.Range (-8, 8) * 2, lane, 0);
					break;
				case 3:
					positionArray [i] = new Vector4 ((int)Random.Range (-14, 14) * 2, 18, lane, 90);
					break;
				}
				break;
			case 2:
				switch (side) {
				case 1:
					positionArray [i] = new Vector4 ((int)Random.Range (-13, 13) * 2, -16, lane, 270);
					break;
				case 2:
					positionArray [i] = new Vector4 (-28, (int)Random.Range (-7, 7) * 2, lane, 0);
					break;
				case 3:
					positionArray [i] = new Vector4 ((int)Random.Range (-13, 13) * 2, 16, lane, 90);
					break;
				}
				break;
			case 3:
				switch (side) {
				case 1:
					positionArray [i] = new Vector4 ((int)Random.Range (-12, 12) * 2, -14, lane, 270);
					break;
				case 2:
					positionArray [i] = new Vector4 (-26, (int)Random.Range (-6, 6) * 2, lane, 0);
					break;
				case 3:
					positionArray [i] = new Vector4 ((int)Random.Range (-12, 12) * 2, 14, lane, 90);
					break;
				}
				break;
			}
		}
	}

	void respawn(){
        int lane;

        foreach (GameObject zombie in Zombies)
        {
            lane = zombie.GetComponent<Zombie>().lane;

            if (inCorner(lane, zombie))
            {
                
                if(Random.value <= spawnProbability)
                {
                    print("DESTROYED");
                    zombie.transform.localEulerAngles = new Vector3(0, zombie.transform.localEulerAngles.y + 90, 0);
                    GameObject newZombie = (GameObject)Instantiate(zombie, zombie.transform.position,
                        Quaternion.Euler(zombie.transform.localEulerAngles.x, zombie.transform.localEulerAngles.y, zombie.transform.localEulerAngles.z));
                    
                    //GameObject newZombie = (GameObject)Instantiate(zombie, zombie.transform.position + transform.right *2, Quaternion.identity);
                    GameObject.Destroy(zombie);
                    Zombies.Remove(zombie);
                    break;
                }
            }
        }

	}

    bool inCorner(int lane, GameObject zombie)
    {
        float posX = zombie.transform.position.x;
        float posZ = zombie.transform.position.z;

        if (lane == 1)
            return (30 - Mathf.Abs(posX) <= 0.00001 && 18 - Mathf.Abs(posZ) <= 0.00001);
        
        if (lane == 2)
            return (28 - Mathf.Abs(posX) <= 0.00001 && 16 - Mathf.Abs(posZ) <= 0.00001);

        if (lane == 3)
            return (26 - Mathf.Abs(posX) <= 0.00001 && 14 - Mathf.Abs(posZ) <= 0.00001);

        return false;
    }

    public GameObject returnZombie()
    {
        if (Random.value >= ratioEasyToHard)
        {
            if (Random.value >= 0.5)
                return classic;
            else
                return shambler;
        }

        else
        {
            if (Random.value >= 0.5)
                return modern;
            else
                return phone;
        }
    }
}
