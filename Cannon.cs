using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cannon : Unit
{
	public GUISkin GameGuiSkin;
	
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
	
	public bool canMove;
	public bool canShoot;
	private int woodCounter;
	public bool SelectShoot;
	
	public bool Chosen = false;
	private Vector3 position;
	private Ray ray;
	private RaycastHit hit;
	public Color turnColor;
	public Color currentColor;
	
	void Start()
	{
		//rank = UnitRank.Cannon;
		//Debug.Log("Cannon Start has been called");
		// Get current tile position
		CurrentTile = transform.GetComponentInParent<Tile>();
		//Debug.Log ("CurrentTile : "+CurrentTile);
		canShoot = true;
		
		//this will find the village associated with this Cannon, which will be the variable home.
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
		CurrentTile.HasCannon = true;
		//Debug.Log(CurrentTile);
		
		Glow = transform.FindChild("Glow").gameObject;
	}
	
	void Update()
	{
		turnColor = GameObject.Find("Engine").GetComponent<Engine>().currentPlayer;
		currentColor = CurrentTile.GetComponent<Renderer>().material.color;
		
		if (Input.GetKeyDown(KeyCode.K))
		{
			print("K Pressed");
			SelectShoot = true;
		}
		if (Chosen)
		{
			Glow.SetActive(true);
		}
		else
		{
			Glow.SetActive(false);
		}
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
		{
			Chosen = false;
		}
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		
		if (Chosen && Physics.Raycast(ray, out hit, 1000) && Input.GetMouseButtonDown(1) && hit.transform != transform) //attempt to move Cannon
		{
			
			Tile target = hit.transform.gameObject.GetComponent<Tile>();
			
			if (target == null) //if the object that you clicked is not a tile, try to find the parent root s.t. it's a Tile.
			{
				target = PathFind.findParentWithTag("Tile", hit.transform.gameObject).GetComponent<Tile>(); //try to set target to parent (wherever it is) Tile of the object clicked	
			}
			
			NetworkViewID targetID = target.transform.GetComponent<NetworkView>().viewID;
			
			if (target.transform.GetComponent<Renderer>().material.color != CurrentTile.transform.GetComponent<Renderer>().material.color) //try to invade the destination tile...
			{
				if(SelectShoot) GetComponent<NetworkView>().RPC("Shoot", RPCMode.AllBuffered, targetID);
				
				GetComponent<NetworkView>().RPC("Invade", RPCMode.AllBuffered, targetID);
			}
			else //just perform move 
			{
				GetComponent<NetworkView>().RPC("Move", RPCMode.AllBuffered, CurrentTile.GetComponent<NetworkView>().viewID, target.GetComponent<NetworkView>().viewID);
			}
			CurrentTile = transform.GetComponentInParent<Tile>(); //reset the currentTile to the tile this script is on. Do this even if the Cannon OBJECT (in game) is not on this tile. 
		}
	}
	
	// TODO: Delete after testing
	
	
	public void TileType(Tile destination)
	{
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
					CurrentTile.updateBool("HasCannon", false);
					CurrentTile.updateBool("HasUnit", false);
					destination.updateBool("HasCannon", true);
					CurrentTile = destination; //now the little fucker's destination tile is on the empty square!
					destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
					Add(CurrentTile);
				}
				else if (destination.HasMeadow)
				{
					CurrentTile.updateBool("HasCannon", false);
					CurrentTile.updateBool("HasUnit", false);
					destination.updateBool("HasCannon", true);
					destination.updateBool("HasMeadow", false);
					destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the meadow square!
					Add(CurrentTile);
				}
				
				// Test case 4: tile has a tree 
				else if (destination.HasTree)
				{
					//CurrentTile.updateBool("HasCannon", false);
					//CurrentTile.updateBool("HasUnit", false);
					
					//destination.updateBool("HasCannon", true);
					//destination.updateBool("HasTree", false);
					//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
					//CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
					//Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
					//Add(CurrentTile);
					
				}
				
				// Test case 5: tile has a tombstone, move Cannon there, but can't move again
				else if (destination.HasTomb)
				{
					//
				}
			}
			else if (_enemy) //can invade enemy tiles that have a peasant or non-villager tiles
			{
				foreach (Transform neighborTransform in destination.Neighbours)
				{
					Tile neighborTile = neighborTransform.GetComponent<Tile>();
					
					// Check to see whether the destination tile is guarded by Cannon or higher villager (not peasant).
					// If so, return false (can't move there) otherwise true.
					if (neighborTransform.GetComponent<Renderer>().material.color == destination.transform.GetComponent<Renderer>().material.color
					    && neighborTile.HasUnit == true && neighborTile.HasPeasant == false && neighborTile.HasInfantry == false && neighborTile.HasSoldier == false)
					{
						Debug.Log("Sorry, this tile is guarded");
						break;
					}
				}
				
				if (destination.IsEmpty || destination.HasMeadow)
				{
					//CurrentTile.updateBool("HasCannon", false);
					//CurrentTile.updateBool("HasUnit", false);
					//destination.updateBool("HasCannon", true);
					
					//if (destination.HasMeadow || !destination.HasRoad)
					//    destination.updateBool("HasMeadow", false);
					//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
					//CurrentTile = destination; // now the little fucker's destination tile is on the empty square!
					//Add(CurrentTile);
				}
				
				else if (destination.HasUnit)
				{
					if (destination.HasPeasant)
					{
						//CurrentTile.updateBool("HasCannon", false);
						//CurrentTile.updateBool("HasUnit", false);
						//destination.updateBool("HasPeasant", false);
						//destination.updateBool("HasCannon", true);
						//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
						//CurrentTile = destination; // now the little fucker's destination tile is on the conquered square!
						//Add(CurrentTile);
					}
					
					else if (destination.HasInfantry)
					{
						//CurrentTile.updateBool("HasCannon", false);
						//CurrentTile.updateBool("HasUnit", false);
						//destination.updateBool("HasInfantry", false);
						//destination.updateBool("HasCannon", true);
						//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
						//CurrentTile = destination; //now the little fucker's destination tile is on the conquered square!
						//Add(CurrentTile);
					}
					else
					{
						print("Not a peasant or Infantry");
					}
				}
				// Test case 4: tile has a tree 
				else if (destination.HasVillage) //can't invade village 
				{
					if (!destination.HasCastle)
					{
						
						//Village invadedVillage = destination.gameObject.GetComponentInChildren<Village>();
						
						//Home.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, invadedVillage.Gold);
						//Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, invadedVillage.Wood);
						
						//CurrentTile.updateBool("HasCannon", false);
						//CurrentTile.updateBool("HasUnit", false);
						//destination.updateBool("HasCannon", true);
						//destination.updateBool("HasVillage", false);
						//destination.updateBool("HasHovel", false);
						//destination.updateBool("HasTown", false);
						//destination.updateBool("HasFort", false);
						//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
						//CurrentTile = destination; //now the little fucker's destination tile is on the conquered square!
						//Add(CurrentTile);
					}
				}
				else if (destination.HasTree)
				{
					//Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
					
					//CurrentTile.updateBool("HasCannon", false);
					//CurrentTile.updateBool("HasUnit", false);
					
					//destination.updateBool("HasCannon", true);
					//destination.updateBool("HasTree", false);
					//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
					//CurrentTile = destination;
					//Add(CurrentTile);
				}
				
				// Test case 5: tile has a tombstone, move peasant there, but can't move again
				else if (destination.HasTomb)
				{
					//CurrentTile.updateBool("HasCannon", false);
					//CurrentTile.updateBool("HasUnit", false);
					
					//destination.updateBool("HasCannon", true);
					//destination.updateBool("HasTomb", false);
					//destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
					//CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
					//Add(CurrentTile);
				}
				else
				{
					Debug.Log("This is a default case and should never execute. If it does, need to include something in above cases");
				}
			}
			else
			{
				Debug.Log("Can't Invade non-neutral territory!");
			}
		}
	}
	
	// Checks to make sure tile t is of a different color as home tile's color, then adds that tile to the list of tiles in the village
	public void DestroyVill(Tile t)
	{
		Color homeColor = Home.transform.GetComponentInParent<Renderer>().material.color;
		Color tileColor = t.transform.GetComponentInParent<Renderer>().material.color;
		
		// If the Home tile colour is not the same as the destination, change the colour of the destionation tile
		if (homeColor != tileColor)
		{
			Home.AddTile(t);
			if (tileColor != Color.gray)
			{
				Debug.Log("Splitting at tile: " + t.Position);
				GetComponent<SplitVillages>().CheckForSplit(tileColor, t);
			}
			
			t.updateColor(Color.gray);
		}
	}
	public void Add(Tile t)
	{
		Color homeColor = Home.transform.GetComponentInParent<Renderer>().material.color;
		Color tileColor = t.transform.GetComponentInParent<Renderer>().material.color;
		
		// If the Home tile colour is not the same as the destination, change the colour of the destionation tile
		if (homeColor != tileColor)
		{
			Home.AddTile(t);
			
			if (tileColor != Color.gray)
			{
				Debug.Log("Splitting at tile: " + t.Position);
				GetComponent<SplitVillages>().CheckForSplit(tileColor, t);
			}
		}
	}
	
	public void DestroyVillage(Tile tile)
	{
		Debug.Log("Detroy village.");
		List<Tile> tilelist = new List<Tile>();
		tilelist = GetComponent<SplitVillages>().GetTiles(tile);
		tilelist.RemoveAt(0);
		
		GetComponent<SplitVillages>().TurnNeutral(tilelist);
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
			if (!found)
				list = PathFinding.GetPathWithoutObstacles(CurrentTile, target);
			
			if (list == null)
			{
				Debug.LogError("List is empty.");
			}
			
			else
			{
				foreach (Tile destination in list)
				{
					// Attempt to move to tile t (one unit distance) and if can't move anymore / illegal move, stop moving...
					TileType(destination); //determines the type of the destination tile 
					
					Destination = destination;
					
					GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().resetVisitedAll(); //reset all tiles to unvisited, just in case...no reason in particular
					
					if (_neutral)
						print("IT'S NEUTRAL BOYYYY");
					if (_home)
					{
						// Test case 1: tile is empty, so move there and return true
						if (destination.IsEmpty)
						{
							CurrentTile.updateBool("HasCannon", false);
							CurrentTile.updateBool("HasUnit", false);
							destination.updateBool("HasCannon", true);
							destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
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
							CurrentTile.updateBool("HasCannon", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasCannon", true);
							if (!destination.HasRoad)
								destination.updateBool("HasMeadow", false);
							
							destination.Cannon.transform.GetComponent<Cannon>().updateBool("canMove", false);
							CurrentTile = destination; //now the little fucker's destination tile is on the meadow square!
						}
						
						// Test case 4: tile has a tree 
						else if (destination.HasTree)
						{
							break;
						}
						
						// Test case 5: tile has a tombstone, move Cannon there, but can't move again
						else if (destination.HasTomb)
						{
							break;
						}
						
						// Test case 6: tile has an existing unit, so can't move there, can't proceed further
						else if (destination.HasUnit)
						{
							Debug.Log("Cannot move to a tile that already has an existing unit.");
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
		//					else if(t.HasMeadow)
		//					{
		//						t.updateBool("HighlightedRed", true);
		//						
		//					}
		//					else 
		//					{
		//						t.updateBool("Highlighted", true);
		//					}
		//				}
		//			}
		//		}
	}
	
	//	[RPC]
	public void ResetAllHighlight() //resets all tiles' highlighted to false...
	{
		//		if(Network.isServer)
		//		{
		//			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Tile"))
		//			{
		//				Tile t = g.transform.GetComponent<Tile>();
		//				t.updateBool("Highlighted", false);
		//				t.updateBool("HighlightedYellow", false);
		//				t.updateBool("HighlightedRed", false);
		//			}
		//		}
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
	
	[RPC]
	//this method will make the cannon attempt to shoot an enemy tile. If cannon does in fact shoot, return true else return false.
	public bool Shoot(NetworkViewID destinationID)
	{
		if (Network.isServer)
		{
			
			if (!canShoot)
			{
				Debug.LogError("Already shot this turn.");
				return false;
			}
			Tile destination = NetworkView.Find(destinationID).transform.GetComponent<Tile>();
			
			if (!WithinRange(destination))
			{
				Debug.LogError("Sorry, cannon is not within range. Please try to get within 2 tiles of the target.");
				return false;
			}
			else //within range, but check to see whether there is enough wood, and also if the target is either a villager or a village!
			{
				TileType(destination); //check the type of dest tile		
				if (Home.Wood < 1)
				{
					Debug.LogError("Sorry, need more wood to shoot the cannon!");
				}
				
				if (_enemy)
				{
					if (!destination.HasUnit && !destination.HasVillage) //give an error if the enemy tile is neither a unit nor a village.
					{
						Debug.LogError("Sorry, please select a valid target. You can only shoot enemy villagers or villages.");
					}
					else
					{
						if (destination.HasUnit) //cannons can destroy cannons too!
						{
							Home.Wood -= 1; //takes 1 wood to shoot
							
							//this block of code will make it so it kills ANY unit, including cannon.
							destination.updateBool("HasPeasant", false);
							destination.updateBool("HasInfantry", false);
							destination.updateBool("HasSoldier", false);
							destination.updateBool("HasKnight", false);
							destination.updateBool("HasCannon", false);
							destination.updateBool("HasUnit", false);
							destination.updateBool("HasTomb", true); //replace with a tombstone.
							
							updateCanShoot(false);
							
							return true;
						}
						else if (destination.HasVillage)
						{
							Home.Wood -= 1; //takes 1 wood to shoot
							
							Village villageShot = destination.Village.GetComponent<Village>(); //gets village associated with the tile
							villageShot.updateCannonShots(villageShot.CannonballShotsTaken + 1);
							int ShotsTakenAfter = villageShot.CannonballShotsTaken;
							
							if (destination.HasHovel) //kill the village, no exceptions. 
							{
								villageShot.updateCannonShots(0);
								destination.updateBool("HasHovel", false);
								destination.updateBool("HasVillage", false);
								DestroyVill(destination);
							}
							else if (destination.HasTown && ShotsTakenAfter == 2)
							{
								villageShot.updateCannonShots(0);
								destination.updateBool("HasTown", false);
								destination.updateBool("HasVillage", false);
								DestroyVill(destination);
							}
							else if (destination.HasFort && ShotsTakenAfter == 5)
							{
								villageShot.updateCannonShots(0);
								destination.updateBool("HasFort", false);
								destination.updateBool("HasVillage", false);
								DestroyVill(destination);
							}
							else if (destination.HasCastle && ShotsTakenAfter == 10)
							{
								villageShot.updateCannonShots(0);
								destination.updateBool("HasCastle", false);
								destination.updateBool("HasVillage", false);
								DestroyVill(destination);
							}
							
							updateCanShoot(false);
							
							return true;
						}
					}
				}
				else
				{
					Debug.LogError("Sorry, can't shoot non-enemy targets.");
				}
				return true;
			}
		}
		
		return false;
	}
	
	void OnGUI()
	{
		GUI.skin = GameGuiSkin;
		
		if (Chosen)
		{
			if (GUI.Button(new Rect(10, 280, 120, 50), "Shoot Cannon")) //activates buy knight button
			{
				SelectShoot = true;
				Debug.Log("Now please select a tile to shoot at!");
			}
		}
	}
	
	[RPC]
	public void updateCanShoot(bool state)
	{
		canShoot = state;
	}
	
	public bool WithinRange(Tile destination)
	{
		int destX = (int)destination.Position.x; //this is the int version of the destination tile's x position
		int destY = (int)destination.Position.y; //this is the int version of the destination tile's x position
		
		int thisX = (int)CurrentTile.Position.x; //this is the int version of the destination tile's x position
		int thisY = (int)CurrentTile.Position.y; //this is the int version of the destination tile's x position
		
		if (Mathf.Abs(thisX - destX) <= 2 && Mathf.Abs(thisY - destY) <= 2) return true; //if the diff b/t the X's and Y's of the dest and current coors are both <= 2, then return true else return false.
		else return false;
	}
}