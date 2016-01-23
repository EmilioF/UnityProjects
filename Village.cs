using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Village : MonoBehaviour
{	
	
	public GUISkin GameGuiSkin;
	
	public int Gold;
	public int Wood;
	public int CannonballShotsTaken;
	public GameObject Glow;
	public Color turnColor;
	public Color currentColor;
	public bool Chosen = false;
	public bool BuyingWatchTower = false;	
	
	public Tile CurrentTile;
	
	public BuyWatchTower buyTower; 
	public Peasant PeasantPrefab;
	public PathFinding PathFind;
	
	
	NetworkView netView;
	
	public Rank ThisRank;
	
	public List<Tile> VillageTerritory;
	
	public enum Rank { Hovel = 1, Town = 2, Fort = 3, Castle = 4 };
	
	void Start()
	{
		CurrentTile = transform.parent.GetComponent<Tile>(); //set the current tile that the village is currently on
		currentColor = CurrentTile.GetComponent<Renderer>().material.color;
		Gold = 500;
		Wood = 200;
		ThisRank = Rank.Hovel; //set beginning village to hovel 
		
		Glow = transform.FindChild("Glow").gameObject;
		
		MergeVillages m = gameObject.GetComponent<MergeVillages>();
		
		VillageTerritory = m.GetTiles(CurrentTile);
		netView = GetComponent<NetworkView>();
		
		PathFind = transform.gameObject.GetComponentInParent<PathFinding>();
		
		CannonballShotsTaken = 0; //start with no shots on village        
	}
	
	void Update()
	{
		turnColor = GameObject.Find("Engine").GetComponent<Engine>().currentPlayer;
		
		if (Chosen)
		{
			Glow.SetActive(true);
		}
		else
		{
			Glow.SetActive(false);
		}
		
		if(Input.GetButtonDown("rightclick"))
		{
			Chosen = false;
			buyTower.updateBuying(false);
		}
	}
	
	//This method takes in a Tile t and sees whether the village bundle associated with it has an empty tile or not.
	Tile findRandomEmptyTile(Tile t)
	{
		HexGenerator hexgen = GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>(); //this code will reset ALL TILES to unvisited
		t.Visited = true;
		if (t.IsEmpty) return t; //base case...
		foreach (Transform tile in t.Neighbours)
		{ //iterate through each of tile t's neighbors
			if (t.GetComponent<Renderer>().material.color == tile.transform.GetComponent<Renderer>().material.color && tile.GetComponent<Tile>().Visited != true)
				//if the color of the neighbor matches the color of tile t AND has not been visited
			{
				tile.GetComponent<Tile>().Visited = true; //mark the neighbor tile of t as traversed
				Tile test = findRandomEmptyTile(tile.GetComponent<Tile>()); //test to see what it returns
				
				if (test != null) //if test returns some non-null value, return that tile test
				{
					hexgen.resetVisitedAll(); //reset visited of all tiles before returning
					return test;
				}
				else continue; //if test is null, then look at the next neighbor to see whether there exists an empty tile in that search
			}
		}
		return null; //search has been completed without finding any empty tiles, so return null.
	}
	
	/*
    3 things that can happen:
    1. No empty tile exists...: "noTile"
    2. Not enough moneys: "noGold"
    3. Villager created: "villagerCreated"
    */
	
	[RPC]
	public void Hire(string villagerType, NetworkViewID village)
	{
		if (Network.isServer)
		{
			//Debug.Log (CurrentTile);
			Village v = NetworkView.Find(village).transform.GetComponent<Village>();
			Tile t = findRandomEmptyTile(CurrentTile);
			HexGenerator hexgen = GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>(); //this code will reset ALL TILES to unvisited
			hexgen.resetVisitedAll();
			if (t == null) //no such empty tile exists
			{
				print("noTile"); //this will return an error string when tile is not empty
			}
			
			//if there is an empty tile then proceed as follows:
			if (villagerType == "peasant") // is 10 g
			{
				if (v.Gold < 10) //not enough money, so return error msg that says "not enough Gold"
				{
					print("noGold");
				}
				else //create villager on tile t 
				{
					netView.RPC("updateGold", RPCMode.AllBuffered, -10); //now take away Gold	
					t.updateBool("HasPeasant", true); //flip the switch to show a peasant on tile t;
					t.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", true);
				}
				
			}
			else if (villagerType == "infantry") //is 20 g
			{
				
				if (v.Gold < 20) //not enough money, so return error msg that says "not enough Gold"
				{
					print("noGold");
				}
				else //create villager on tile t
				{
					netView.RPC("updateGold", RPCMode.AllBuffered, -20); //now take away Gold	
					t.updateBool("HasInfantry", true); //flip the switch to show a peasant on tile t;
					t.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", true);
					Vector2 coor = t.Position; //this is just used to print...
					print("Infantry created at tile:  " + coor);
				}
			}
			else if (villagerType == "soldier")	//is 30g
			{
				if (v.Gold < 30) //not enough money, so return error msg that says "not enough Gold"
				{
					print("noGold");
				}
				else //create villager on tile t
				{
					netView.RPC("updateGold", RPCMode.AllBuffered, -30); //now take away Gold	
					t.updateBool("HasSoldier", true); //flip the switch to show a peasant on tile t;
					t.Soldier.transform.GetComponent<Soldier>().updateBool("canMove", true);
				}
			}
			else if (villagerType == "knight") //is 40g
			{
				if (v.Gold < 40) //not enough money, so return error msg that says "not enough Gold"
				{
					print("noGold");
				}
				else //create villager on tile t
				{
					netView.RPC("updateGold", RPCMode.AllBuffered, -40); //now take away Gold	
					t.updateBool("HasKnight", true); //flip the switch to show a peasant on tile t;
					t.Knight.transform.GetComponent<Knight>().updateBool("canMove", true);
				}
			}
			
			else if (villagerType == "cannon") //is 40g
			{
				if (v.Gold < 35) //not enough money, so return error msg that says "not enough Gold"
				{
					print("Not enough gold");
				}
				else if (v.Wood < 12)
				{
					print("Not enough wood");
				}
				
				else
				{
					netView.RPC("updateGold", RPCMode.AllBuffered, -35); //now take away Gold	
					netView.RPC("updateWood", RPCMode.AllBuffered, -12);
					t.updateBool("HasCannon", true); //flip the switch to show a peasant on tile t;
				}
			}
			else if (villagerType == "watchtower")
			{
				if (v.Gold < 5) //not enough money, so return error msg that says "not enough Gold"
				{
					print("Not enough gold");
				}	
				else
				{
					netView.RPC("updateGold", RPCMode.AllBuffered, -5); //now take away Gold	
					t.updateBool("HasWatchTower", true); //flip the switch to show a peasant on tile t;
				}
			}
			else //error!	
			{
				Debug.LogError("Can't create this thing bro"); //send error msg to console
				print("notA");
			}
		}
		
	}
	
	void OnGUI() //sets up the gui menu for the village: buying villagers and upgrading villages
	{
		GUI.skin = GameGuiSkin;
		
		if (Chosen)
		{
			if (CurrentTile.HasCastle)
			{
				GUI.Label(new Rect(10, 0, 350, 25), "Gold: " + Gold + ", Wood: " + Wood + ", Cannonball Shots taken: " + CannonballShotsTaken + "/10");
				
				
				if (GUI.Button(new Rect(135, 85, 120, 50), "Buy Knight: \n 40 gold")) //activates buy knight button
				{
					if (turnColor == currentColor)
						netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "knight", this.GetComponent<NetworkView>().viewID });
					else
						print("NOT YOUR TURN");
				}
				
				if (GUI.Button(new Rect(10, 85, 120, 50), "Buy Soldier: \n 30 gold")) //activates buy soldier button
				{
					if (turnColor == currentColor)
						netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "soldier", this.GetComponent<NetworkView>().viewID });
					else
						print("NOT YOUR TURN");
				}
				
				if (GUI.Button(new Rect(135, 140, 120, 50), "Buy Cannon: \n 30 gold")) //activates buy soldier button
				{
					if (turnColor == currentColor)
						netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "cannon", this.GetComponent<NetworkView>().viewID });
					else
						print("NOT YOUR TURN");
				}
				if (GUI.Button(new Rect(10, 140, 120, 50), "Buy WatchTower: \n 5 gold"))
				{
					if (turnColor == currentColor)
						buyTower.updateBuying(true);
					else
						print("NOT YOUR TURN");
				}
			}
			else if (CurrentTile.HasFort) //fort can buy ALL villagers
			{
				GUI.Label(new Rect(10, 0, 350, 25), "Gold: " + Gold + ", Wood: " + Wood + ", Cannonball Shots taken: " + CannonballShotsTaken + "/5");
				
				if (GUI.Button(new Rect(135, 85, 120, 50), "Buy Knight: \n 40 gold")) //activates buy knight button
				{
					if (turnColor == currentColor)
						netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "knight", this.GetComponent<NetworkView>().viewID });
					else
						print("NOT YOUR TURN");
					
				}
				if (GUI.Button(new Rect(10, 85, 120, 50), "Buy Soldier: \n 30 gold")) //activates buy soldier button
				{
					if (turnColor == currentColor)
						netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "soldier", this.GetComponent<NetworkView>().viewID });
					else
						print("NOT YOUR TURN");
				}
				if (GUI.Button(new Rect(10, 140, 120, 50), "Buy WatchTower: \n 5 gold"))
				{
					if (turnColor == currentColor)
						buyTower.updateBuying(true);
					else
						print("NOT YOUR TURN");
				}
				
				if (GUI.Button(new Rect(135, 140, 120, 50), "Buy Cannon: \n 30 gold")) //activates buy soldier button
				{
					if (turnColor == currentColor)
						netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "cannon", this.GetComponent<NetworkView>().viewID });
					else
						print("NOT YOUR TURN");
				}
				
			}
			else if (CurrentTile.HasTown) //Town can buy soldier, peasant, or infantry
			{
				GUI.Label(new Rect(10, 0, 350, 25), "Gold: " + Gold + ", Wood: " + Wood + ", Cannonball Shots taken: " + CannonballShotsTaken + "/2");
				
				if (GUI.Button(new Rect(10, 85, 120, 50), "Buy Soldier: \n 30 gold")) //activates buy soldier button
				{
					netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "soldier", this.GetComponent<NetworkView>().viewID });
				}
				
				if (GUI.Button(new Rect(10, 140, 120, 50), "Buy WatchTower: \n 5 gold"))
				{
					if (turnColor == currentColor)
						buyTower.updateBuying(true);
					else
						print("NOT YOUR TURN");
				}
			}
			else // base case if (CurrentTile.HasHovel)
			{
				GUI.Label(new Rect(10, 0, 200, 25), "Gold: " + Gold + ", Wood: " + Wood);
				
			}
			
			//if the rank is a Hovel, then can only buy peasant or infantry
			if (GUI.Button(new Rect(10, 30, 120, 50), "Buy Peasant: \n 10 gold")) //activates buy peasant button
			{
				if (turnColor == currentColor)
					netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "peasant", this.GetComponent<NetworkView>().viewID });
				else
					print("NOT YOUR TURN");
				
			}
			if (GUI.Button(new Rect(135, 30, 120, 50), "Buy Infantry: \n 20 gold")) //activates buy infantry button
			{
				if (turnColor == currentColor)
					netView.RPC("Hire", RPCMode.AllBuffered, new object[] { "infantry", this.GetComponent<NetworkView>().viewID });
				else
					print("NOT YOUR TURN");
			}
			
			
			if (CurrentTile.HasHovel || CurrentTile.HasTown || CurrentTile.HasFort)
			{
				if (GUI.Button(new Rect(Screen.width*0.5f-150f, 0f, 200f, 30f), "Upgrade Village"))
				{
					if (turnColor == currentColor)
					{
						if (Wood >= 8) //if there's enough wood, upgrade.
						{
							if (CurrentTile.HasHovel) //upgrade from hovel to town
							{
								netView.RPC("upgradeVillage", RPCMode.AllBuffered, 8);
								transform.parent.GetComponent<Tile>().Hovel.SetActive(false); //make the hovel not show on gamescreen
								CurrentTile.updateBool("HasHovel", false);
								CurrentTile.updateBool("HasTown", true);
								
								netView.RPC("updateRank", RPCMode.AllBuffered, 2);
							}
							else if (CurrentTile.HasTown)
							{
								netView.RPC("upgradeVillage", RPCMode.AllBuffered, 8);
								transform.parent.GetComponent<Tile>().Hovel.SetActive(false); //make the hovel not show on gamescreen
								CurrentTile.updateBool("HasTown", false);
								CurrentTile.updateBool("HasFort", true);
								
								netView.RPC("updateRank", RPCMode.AllBuffered, 3);
								
							}
							
							else if (CurrentTile.HasFort)
							{
								if (Wood >= 12)
								{
									netView.RPC("upgradeVillage", RPCMode.AllBuffered, 12);
									transform.parent.GetComponent<Tile>().Hovel.SetActive(false); //make the hovel not show on gamescreen
									CurrentTile.updateBool("HasFort", false);
									CurrentTile.updateBool("HasCastle", true);
									
									netView.RPC("updateRank", RPCMode.AllBuffered, 4);
								}
							}
							else //not enough wood duh
							{
								print("Not Enough Wood");
							}
						}
					}
				}
			}
		}
	}
	
	void OnMouseDown()
	{
		if (currentColor == GameObject.Find("Player(Clone)").GetComponent<Player>().yourColor)
		{
			if (!Chosen)
			{
				//Lighting
				GameObject[] villages = GameObject.FindGameObjectsWithTag("Village");
				for (int i = 0; i < villages.Length; i++)
				{
					if (villages[i].GetComponent<Village>().Chosen)
						villages[i].GetComponent<Village>().Chosen = false;
				}
			}
			
			Chosen = !Chosen;
		}
	}
	
	public void AddTile(Tile target)
	{
		Color color = CurrentTile.GetComponent<Renderer>().material.color;
		target.updateColor(color);
		
		transform.gameObject.GetComponent<MergeVillages>().CheckForMerge();
	}
	
	[RPC]
	public void upgradeVillage(int wood)
	{
		this.Wood -= wood;
		ThisRank = Rank.Town;
	}
	
	[RPC]
	public void updateGold(int gold)
	{
		this.Gold += gold;
	}
	
	[RPC]
	public void updateWood(int wood)
	{
		this.Wood += wood;
	}
	
	[RPC]
	public void updateCannonShots(int shots)
	{
		this.CannonballShotsTaken += shots;
	}
	
	[RPC]
	public void updateRank(int i)
	{
		if (i == 1)
			ThisRank = Rank.Hovel;
		else if (i == 2)
			ThisRank = Rank.Town;
		else if (i == 3)
			ThisRank = Rank.Fort;
		else if (i == 4)
			ThisRank = Rank.Castle;
	}
	
	public void KillVillagers ()
	{
		Tile tile = this.GetComponentInParent<Tile>();
		
		List<Tile> tiles = GetComponent<MergeVillages>().GetTiles(tile);
		
		foreach (Tile t in tiles)
		{
			if (t.HasUnit)
			{
				t.updateBool("HasUnit", false);
				t.updateBool("HasTomb", true);
				t.updateBool("HasPeasant", false);
				t.updateBool("HasSoldier", false);
				t.updateBool("HasInfantry", false);
				t.updateBool("HasKnight", false);
				t.updateBool("HasCannon", false);
			}
		}
		
		GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -Gold);
		
		CurrentTile.updateBool("HasHovel", true);
		CurrentTile.updateBool("HasFort", false);
		CurrentTile.updateBool("HasTown", false);
		CurrentTile.updateBool("HasCastle", false);
	}
}