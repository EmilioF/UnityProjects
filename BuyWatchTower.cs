using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuyWatchTower : MonoBehaviour {
	
	public bool BuyingWatchTower;
	public PathFinding PathFind;
	public Tile CurrentTile;
	public Village CurrentVillage;
	
	NetworkView netView;
	
	// Use this for initialization
	void Start () {
		BuyingWatchTower = false;
		CurrentTile = transform.parent.GetComponent<Tile>(); //set the current tile that the village is currently on
		
		PathFind = transform.gameObject.GetComponentInParent<PathFinding>();
		Debug.Log (PathFind);
		//Debug.Log (tiles.Count);
		
		CurrentVillage = transform.gameObject.GetComponentInParent<Village>();
		netView = GetComponent<NetworkView>();
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (BuyingWatchTower && Physics.Raycast(ray, out hit, 1000) && Input.GetButtonDown("leftclick"))
		{
			BuyingWatchTower = false; //turn buyingwatchtower off; 
			
			Tile target = hit.transform.gameObject.GetComponent<Tile>();
			//Debug.Log ("target is: " + target);
			//LinkedList<Tile> = PathFinding.GetPathWithoutObstacles()
			
			if(target == null) //if the object that you clicked is not a tile, try to find the parent root s.t. it's a Tile.
			{
				//Debug.Log ("BITCH PLEASE ");
				target = PathFind.findParentWithTag("Tile" , hit.transform.gameObject).GetComponent<Tile>(); //try to set target to parent (wherever it is) Tile of the object clicked	
			}
			
			
			if(CurrentTile.transform.GetComponent<Renderer>().material.color == target.transform.GetComponent<Renderer>().material.color) //if color of target is the same as the current tile
			{	
				List<Tile> tiles = new List<Tile>();
				tiles = PathFind.GetTiles(CurrentTile);
				Village TestVillage = null;
				//Debug.Log (tiles.Count);
				
				//try to find the village connected with the current tile. If that village is the same as currentvillage, then you buy watchtower if there's enough money, else not.
				foreach (Tile t in tiles)
				{
					if (t.HasVillage)
					{
						TestVillage = t.Village.transform.GetComponent<Village>();
					}
				}
				
				if(TestVillage != null && TestVillage == CurrentVillage && target.IsEmpty) //if the village is the same, attempt to buy watchtower
				{
					if (CurrentVillage.Gold < 5) //not enough money, so return error msg that says "not enough Gold"
					{
						print("Not enough gold");
					}	
					else
					{
						CurrentVillage.gameObject.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -5); //now take away Gold	
						target.updateBool("HasWatchTower", true); //flip the switch to show a peasant on tile t;
						BuyingWatchTower = false; //reset....
					}
				}
			}
			
			if(Input.GetButtonDown("rightclick"))
			{
				Debug.Log ("BOOYAH");
				BuyingWatchTower = false;
			}
		}
		
	}
	
	[RPC]
	public void updateBuying(bool state)
	{
		BuyingWatchTower = state;
	}
}
