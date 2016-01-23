using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Infantry : Unit
{
	public Village Home;
	public Tile CurrentTile;
	public Tile Destination;
	public PathFinding PathFind;
	public GameObject Glow;
	
	private bool _neutral;
	private bool _home;
	private bool _enemy;
	private bool _water;
	private bool moving;
	
	private bool canMove;
	private int woodCounter;
	
	// TODO: Delete after testing
	public bool Chosen = false;
	private Vector3 position;
	private Ray ray;
	private RaycastHit hit;
	public Color turnColor;
	public Color currentColor;
	
	void Start()
	{
		rank = UnitRank.Infantry;
		//Debug.Log("Infantry Start has been called");
		// Get current tile position
		CurrentTile = transform.GetComponentInParent<Tile>();
		//Debug.Log ("CurrentTile : "+CurrentTile);
		
		
		//this will find the village associated with this Infantry, which will be the variable home.
		PathFind = transform.gameObject.GetComponentInParent<PathFinding>();
		List<Tile> tiles = PathFind.GetTiles(CurrentTile);
		//Debug.Log (tiles.Count);
		
		foreach (Tile t in tiles)
		{
			if (t.HasVillage)
			{
				Home = t.Village.transform.GetComponent<Village>();
			}
		}
		
		_neutral = false;
		_home = false;
		_enemy = false;
		_water = false;
		
		// TODO: Test purposes only
		CurrentTile.HasInfantry = true;
		//Debug.Log(CurrentTile);
		
		Glow = transform.FindChild("Glow").gameObject;
	}
	
	void Update()
	{
		turnColor = GameObject.Find("Engine").GetComponent<Engine>().currentPlayer;
		currentColor = CurrentTile.GetComponent<Renderer>().material.color;
		if (Chosen)
		{
			Glow.SetActive(true);
		}
		else
		{
			Glow.SetActive(false);
		}
		
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(ray, out hit, 1000))
			{
				if (hit.collider.transform == this.transform)
				{
					if (currentColor == turnColor)
						this.Chosen = true;
				}
				else
					this.Chosen = false;
			}
		}
		
		if (!canMove)
			Chosen = false;
		//		ResetAllHighlight();
		////		GetComponent<NetworkView>().RPC("ResetAllHighlight", RPCMode.AllBuffered);
		//
		//		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		//
		//		if (Chosen && Physics.Raycast(ray, out hit, 1000))
		//		{
		//			Tile target = hit.transform.gameObject.GetComponent<Tile>();
		//			//Debug.Log ("target is: " + target);
		//			//LinkedList<Tile> = PathFinding.GetPathWithoutObstacles()
		//
		//			if(target == null) //if the object that you clicked is not a tile, try to find the parent root s.t. it's a Tile.
		//			{
		//				//Debug.Log ("BITCH PLEASE ");
		//				target = PathFind.findParentWithTag("Tile" , hit.transform.gameObject).GetComponent<Tile>(); //try to set target to parent (wherever it is) Tile of the object clicked
		//			}
		//
		//			if(target != null)
		//			{
		//				GetComponent<NetworkView>().RPC("HighlightPath", RPCMode.AllBuffered, CurrentTile.GetComponent<NetworkView>().viewID, target.GetComponent<NetworkView>().viewID);
		//			}
		//		}
		//Here is how the mechanism will work: Chosen when Infantry created is false. When you click on Infantry, chosen is always true.
		//If you right click, chosen = false. If you click on a tile when chosen is true, Infantry will attempt to move there.
		
		if (Chosen && Physics.Raycast(ray, out hit, 1000) && Input.GetMouseButtonDown(1) && hit.transform != transform && canMove) //attempt to move Infantry
		{
			
			Tile target = hit.transform.gameObject.GetComponent<Tile>();
			
			if (target == null) //if the object that you clicked is not a tile, try to find the parent root s.t. it's a Tile.
			{
				//Debug.Log("BITCH PLEASE ");
				target = PathFind.findParentWithTag("Tile", hit.transform.gameObject).GetComponent<Tile>(); //try to set target to parent (wherever it is) Tile of the object clicked
			}
			
			
			
			
			NetworkViewID targetID = target.transform.GetComponent<NetworkView>().viewID;
			
			if (target.transform.GetComponent<Renderer>().material.color != CurrentTile.transform.GetComponent<Renderer>().material.color) //try to invade the destination tile...
			{
				
				GetComponent<NetworkView>().RPC("Invade", RPCMode.AllBuffered, targetID);
			}
			else //just perform move
			{
				GetComponent<NetworkView>().RPC("Move", RPCMode.AllBuffered, CurrentTile.GetComponent<NetworkView>().viewID, target.GetComponent<NetworkView>().viewID);
			}
			
			CurrentTile = transform.GetComponentInParent<Tile>(); //reset the currentTile to the tile this script is on. Do this even if the Infantry OBJECT (in game) is not on this tile.
		}
		
	}
	
	
	public void TileType(Tile destination)
	{
		//		Debug.Log("Current tile " + CurrentTile.transform.GetComponent<Renderer>().material.color);
		//		Debug.Log("Destination tile " + destination.transform.GetComponent<Renderer>().material.color);
		
		// if tile belongs to Home
		if (CurrentTile.transform.GetComponent<Renderer>().material.color == destination.transform.GetComponent<Renderer>().material.color)
			_home = true;
		// if tile is neutral
		else if (destination.transform.GetComponent<Renderer>().material.color == Color.gray)
			_neutral = true;
		else if (destination.transform.GetComponent<Renderer>().material.color == Color.blue)
			_water = true;
		else
			_enemy = true;
	}
	
	//this can only take over tiles that are neighboring current tile and are neutral.
	[RPC]
	public void Invade(NetworkViewID destinationID)
	{
		
		if (Network.isServer)
		{
			Tile destination = NetworkView.Find(destinationID).transform.GetComponent<Tile>();
			
			TileType(destination);
			if (!IsNeighbour(destination)) //checks to see if the destination tile is adjacent to current tile. If not, cannot invade and return false...
			{
				Debug.LogError("Sorry, can only invade tile that is a adjacent to current tile.");
			}
			
			else if (_neutral)
			{
				if (destination.IsEmpty)
				{
					CurrentTile.updateBool("HasInfantry", false);
					CurrentTile.updateBool("HasUnit", false);
					//Debug.Log ("After tile is: "+CurrentTile);
					destination.updateBool("HasInfantry", true);
					CurrentTile = destination; //now the little fucker's destination tile is on the empty square!
					destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
					Add(CurrentTile);
					//return true; //filler code
				}
				else if (destination.HasMeadow)
				{
					CurrentTile.updateBool("HasInfantry", false);
					CurrentTile.updateBool("HasUnit", false);
					//Debug.Log ("After tile is: "+CurrentTile);
					destination.updateBool("HasInfantry", true);
					destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the meadow square!
					//Debug.Log ("Home: "+ Home + ", "+ Home.Wood);
					Add(CurrentTile);
					
					//return false;
				}
				
				// Test case 4: tile has a tree
				else if (destination.HasTree)
				{
					
					//Debug.Log ("Before Move Wood = " + Home.Wood);
					CurrentTile.updateBool("HasInfantry", false);
					CurrentTile.updateBool("HasUnit", false);
					
					destination.updateBool("HasInfantry", true);
					destination.updateBool("HasTree", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
					//Debug.Log ("Tree hit. Past tile: " + past + " Current tile: "+ CurrentTile );
					Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
					destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
					Add(CurrentTile);
					
					//return false;
					//Debug.Log ("After Wood = " + Home.Wood);
					
				}
				
				// Test case 5: tile has a tombstone, move Infantry there, but can't move again
				else if (destination.HasTomb)
				{
					CurrentTile.updateBool("HasInfantry", false);
					CurrentTile.updateBool("HasUnit", false);
					
					destination.updateBool("HasInfantry", true);
					destination.updateBool("HasTomb", false);
					destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the tomb! YAY DEAD PEOPLE
					Add(CurrentTile);
					
					//return false;
				}
			}
			else if (_enemy) //can invade enemy tiles that have a peasant or non-villager tiles
			{
				bool isGuarded = false;
				
				foreach (Transform neighborTransform in destination.Neighbours)
				{
				    isGuarded = false;
				    Tile neighborTile = neighborTransform.GetComponent<Tile>();
				
					if ((neighborTransform.GetComponent<Renderer>().material.color == destination.transform.GetComponent<Renderer>().material.color
					     && neighborTile.HasUnit == true && neighborTile.HasPeasant == false) || neighborTile.HasFort || neighborTile.HasCastle)
						//check to see whether the destination tile is guarded by infantry or higher villager (not peasant). If so, return false (can't move there) otherwise true.
				    {
				        Debug.Log("Sorry, this tile is guarded");
				        print(neighborTile.transform.name);
				        isGuarded = true;
				        break;
				    }
				}
				
				if (!destination.isGuarded("Infantry"))
				{
					if (destination.IsEmpty || destination.HasMeadow)
					{
						CurrentTile.updateBool("HasInfantry", false);
						CurrentTile.updateBool("HasUnit", false);
						//Debug.Log ("After tile is: "+CurrentTile);
						destination.updateBool("HasInfantry", true);
						destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
						CurrentTile = destination; //now the little fucker's destination tile is on the empty square!
						Add(CurrentTile);
						
					}
					
					else if (destination.HasUnit)
					{
						if (destination.HasPeasant)
						{
							CurrentTile.updateBool("HasInfantry", false);
							CurrentTile.updateBool("HasUnit", false);
							//Debug.Log ("After tile is: "+CurrentTile);
							destination.updateBool("HasPeasant", false);
							destination.updateBool("HasInfantry", true);
							destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
							CurrentTile = destination; //now the little fucker's destination tile is on the conquered square!
							Add(CurrentTile);
						}
						
						else
						{
							print("Not a peasant");
						}
					}
					// Test case 4: tile has a tree
					else if (destination.HasVillage) //can't invade village
					{
						//Debug.LogError ("Cannot move to a village tile.");
						print("an't Move");
					}
					else if (destination.HasTree)
					{
						
						CurrentTile.updateBool("HasInfantry", false);
						CurrentTile.updateBool("HasUnit", false);
						
						destination.updateBool("HasInfantry", true);
						destination.updateBool("HasTree", false);
						CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
						//Debug.Log ("Tree hit. Past tile: " + past + " Current tile: "+ CurrentTile );
						Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
						destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
						Add(CurrentTile);
						//Debug.Log ("After Wood = " + Home.Wood);
					}
					
					// Test case 5: tile has a tombstone, move peasant there, but can't move again
					else if (destination.HasTomb)
					{
						CurrentTile.updateBool("HasInfantry", false);
						CurrentTile.updateBool("HasUnit", false);
						
						destination.updateBool("HasInfantry", true);
						destination.updateBool("HasTomb", false);
						destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
						CurrentTile = destination; //now the little fucker's destination tile is on the tomb! YAY DEAD PEOPLE
						Add(CurrentTile);
						
					}
					else
					{
						Debug.Log("This is a default case and should never execute. If it does, need to include something in above cases");
						//return false;
					}
				}
				else
					print("ITS GUARDED YOOO");
			}
			else
			{
				Debug.Log("Can't Invade non-neutral territory!");
				//foreach (Transform neighborTransform in destination.Neighbours)
				//{
				//    Tile neighborTile = neighborTransform.GetComponent<Tile>();
				//    neighborTile.isGuarded = false;
				//}
			}
		}
	}
	
	//this method will move ONE tile in the destination, and check whether that move is legal / what to do when the villager lands on it
	//This method will only allow a move on a tile in the village bundle.
	//If move is successful AND the villager can move again, return true
	//If move is unsuccessful OR the move is successful but the villager cannot move because he's blocked by something or if needs to stop (for example lands on a tomb)
	[RPC]
	public void Move(NetworkViewID currentTileID, NetworkViewID destinationID) //returns true only if successfully moved.
	{
		if (Network.isServer)
		{
			LinkedList<Tile> list = new LinkedList<Tile>();
			
			Tile CurrentTile = NetworkView.Find(currentTileID).transform.GetComponent<Tile>();
			Tile target = NetworkView.Find(destinationID).transform.GetComponent<Tile>();
			
			bool found = false;
			foreach (Transform dest in CurrentTile.Neighbours)
			{
				Tile desti = dest.GetComponent<Tile>();
				if (desti == target)
				{
					list.AddFirst(desti);
					found = true;
					break;
				}
				
			}
			if(!found)
				list = PathFinding.GetPathWithoutObstacles(CurrentTile, target);
			
			if (list == null)
			{
				Debug.LogError("List is empty.");
			}
			
			else
			{
				//Debug.Log ("Count: "+list.Count);
				foreach (Tile destination in list)
				{
					//attempt to move to tile t (one unit distance) and if can't move anymore / illegal move, stop moving...
					TileType(destination); //determines the type of the destination tile
					
					Destination = destination;
					
					GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().resetVisitedAll(); //reset all tiles to unvisited, just in case...no reason in particular
					
					
					
					if (_neutral)
						print("IT'S NEUTRAL BOYYYY " + destination.transform.name);
					if (_home)
					{
						// Test case 1: tile is empty, so move there and return true
						if (destination.IsEmpty)
						{
							CurrentTile.updateBool("HasInfantry", false);
							CurrentTile.updateBool("HasUnit", false);
							destination.updateBool("HasInfantry", true);
							destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", true);
							CurrentTile = destination;
							
							
						}
						// Test case 2: tile has a village, can't move there so return false
						else if (destination.HasVillage)
						{
							
							break;
						}
						// Test case 3: tile has a meadow, so move there, but cannot move further
						else if (destination.HasMeadow)
						{
							CurrentTile.updateBool("HasInfantry", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasInfantry", true);
							destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", true);
							CurrentTile = destination; //now the little fucker's destination tile is on the meadow square!
							
						}
						
						// Test case 4: tile has a tree
						else if (destination.HasTree)
						{
							CurrentTile.updateBool("HasInfantry", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasInfantry", true);
							destination.updateBool("HasTree", false);
							destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
							CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
							
							Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
							break;
							
							
						}
						
						// Test case 5: tile has a tombstone, move Infantry there, but can't move again
						else if (destination.HasTomb)
						{
							CurrentTile.updateBool("HasInfantry", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasInfantry", true);
							destination.updateBool("HasTomb", false);
							destination.Infantry.transform.GetComponent<Infantry>().updateBool("canMove", false);
							CurrentTile = destination; //now the little fucker's destination tile is on the tomb! YAY DEAD PEOPLE
							
							break;
						}
						
						// Test case 6: tile has an existing unit, so can't move there, can't proceed further
						else if (destination.HasUnit)
						{
							if (destination.HasWatchTower || destination.HasCannon)
							{
								Debug.Log ("Cannot move there.");
								break;
							}
							
							Infantry currentUnit = this;
							Unit targetUnit;
							
							if (destination.HasPeasant)
								targetUnit = destination.Peasant.transform.GetComponent<Peasant>();
							else if (destination.HasInfantry)
								targetUnit = destination.Infantry.transform.GetComponent<Infantry>();
							else if (destination.HasSoldier)
								targetUnit = destination.Soldier.transform.GetComponent<Soldier>();
							else
								targetUnit = destination.Knight.transform.GetComponent<Knight>();
							
							GetComponent<MergeUnits>().CheckForMerge(currentUnit, targetUnit, Home);
							break;
						}
						
						else
						{
							Debug.Log("This is a default case and should never execute. If it does, need to include something in above cases");
							break;
						}
					}
					
					else //this tile is not in village bundle, so illegal move.
					{
						Debug.LogError("Cannot move to a tile not in your village bundle!");
						break;
					}
					
				}
			}
			
			
		}
		
		
	}
	
	//	[RPC]
	public void HighlightPath(NetworkViewID currentTileID, NetworkViewID destinationID)
	{
		//		if(Network.isServer)
		//		{
		//			LinkedList<Tile> list = new LinkedList<Tile>();
		//
		//			Tile CurrentTile = NetworkView.Find(currentTileID).transform.GetComponent<Tile>();
		//			Tile target = NetworkView.Find(destinationID).transform.GetComponent<Tile>();
		//
		//			list = PathFinding.GetPathWithoutObstacles(CurrentTile, target);
		//
		//			if(list != null && list.Count > 0)
		//			{
		//				foreach (Tile t in list)
		//				{
		//					if(t.HasTree || t.HasTomb)
		//					{
		//						t.updateBool("HighlightedYellow", true);
		//						break;
		//					}
		//					else
		//					{
		//						t.updateBool("Highlighted", true);
		//					}
		//				}
		//			}
		//
		//		}
	}
	
	//	[RPC]
	public void ResetAllHighlight() //resets all tiles' highlighted to false...
	{
		//		if(Network.isServer)
		//		{
		//			//Debug.Log ("reset highlight called!");
		//			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Tile"))
		//			{
		//				Tile t = g.transform.GetComponent<Tile>();
		//				t.updateBool("Highlighted", false);
		//				t.updateBool("HighlightedYellow", false);
		//			}
		//		}
	}
	
	//checks to make sure tile t is of a different color as home tile's color, then adds that tile to the list of tiles in the village
	public void Add(Tile t)
	{
		Color homeColor = Home.transform.GetComponentInParent<Renderer>().material.color;
		Color tileColor = t.transform.GetComponentInParent<Renderer>().material.color;
		
		// If the Home tile colour is not the same as the destination, change the colour of the destionation tile
		if (homeColor != tileColor)
		{
			// Calls the split algorithm
			Home.AddTile(t);
			
			if (tileColor != Color.gray)
			{
				//				Debug.Log("Calling split method.");
				GetComponent<SplitVillages>().CheckForSplit(tileColor, t);
				print("SPLIIIIIIIIITTTTTTT");
			}
		}
	}
	
	public bool IsNeighbour(Tile destination)
	{
		List<Transform> Neighbours = CurrentTile.Neighbours;
		
		foreach (Transform t in Neighbours)
		{
			Tile tile = t.GetComponent<Tile>();
			if (tile == destination)
				return true;
		}
		return false;
	}
	
	public void OnGUI()
	{
		if (Chosen)
		{
			if (GUI.Button(new Rect(Screen.width*0.5f-150f, 40f, 200f, 30f), "Upgrade Unit"))
			{
				GetComponent<NetworkView>().RPC("UpgradeUnit", RPCMode.AllBuffered);
			}
		}
	}
	
	[RPC]
	public void UpgradeUnit ()
	{
		if (Home.Gold >= 10 && Home.ThisRank > Village.Rank.Hovel)
		{
			CurrentTile.updateBool("HasInfantry", false);
			CurrentTile.updateBool("HasSoldier", true);
			Home.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -10);
		}
		else
			Debug.Log ("Cannot upgrade: not enough gold.");
	}
	
	public void updateBool(string text, bool state)
	{
		GetComponent<NetworkView>().RPC("updateCanMoveHelper", RPCMode.AllBuffered, text, state);
	}
	
	[RPC]
	public void updateCanMoveHelper(string text, bool state)
	{
		if (text == "canMove")
			this.canMove = state;
	}
}