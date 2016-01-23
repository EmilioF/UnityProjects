using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Peasant : Unit
{
	public Village Home;
	public Tile CurrentTile;
	public Tile Destination;
	public PathFinding PathFind;
	public GameObject Glow;
	public GameObject Box;
	
	private bool _neutral;
	private bool _home;
	private bool _enemy;
	private bool _water;
	private bool moving;
	
	public bool canMove;
	private int woodCounter;
	public int turnCounter;
	public bool buildingRoad;
	public bool cultivatingMeadow;
	
	// TODO: Delete after testing
	public bool Chosen = false;
	private Vector3 position;
	private Ray ray;
	private RaycastHit hit;
	public Color turnColor;
	public Color currentColor;
	
	private Tile _mergeUnit;
	
	void Start ()
	{
		rank = UnitRank.Peasant;
		CurrentTile = transform.GetComponentInParent<Tile>();

		// This will find the village associated with this peasant, which will be the variable home.
		PathFind = transform.gameObject.GetComponentInParent<PathFinding>();
		List<Tile> tiles = PathFind.GetTiles(CurrentTile);
		
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

		CurrentTile.HasPeasant = true;
		
		Glow = transform.FindChild("Glow").gameObject;
//		Box = transform.FindChild("IsBusy").gameObject;

		Box.SetActive(false);
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

		if (canMove)
		{
			Box.SetActive(false);
		}
		else
		{
			Box.SetActive(true);
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
			Chosen = false;
		
		if (Input.GetKeyDown(KeyCode.M) && Chosen)
			cultivateMeadow();
		if (Input.GetKeyDown(KeyCode.R) && Chosen)
			buildRoad();
		
		if (turnColor == currentColor && canMove)
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			//			GetComponent<NetworkView>().RPC("ResetAllHighlight", RPCMode.AllBuffered);
			//Here is how the mechanism will work: Chosen when peasant created is false. When you click on peasant, chosen is always true.
			//If you right click, chosen = false. If you click on a tile when chosen is true, peasant will attempt to move there.
			
			//            if (Chosen && Physics.Raycast(ray, out hit, 1000) && Input.GetButtonDown("leftclick") && hit.transform != transform) //attempt to move peasant
			//            {
			//
			//                Tile target = hit.transform.gameObject.GetComponent<Tile>();
			//
			//                if (target == null) //if the object that you clicked is not a tile, try to find the parent root s.t. it's a Tile.
			//                {
			////                    Debug.Log("BITCH PLEASE ");
			//                    target = PathFind.findParentWithTag("Tile", hit.transform.gameObject).GetComponent<Tile>(); //try to set target to parent (wherever it is) Tile of the object clicked
			//                }
			//				else
			//				{
			//					GetComponent<NetworkView>().RPC("HighlightPath", RPCMode.AllBuffered, CurrentTile.GetComponent<NetworkView>().viewID, target.GetComponent<NetworkView>().viewID);
			////                	Debug.Log("target is: " + target);
			//				}
			//			}
			//
			if (Chosen && Physics.Raycast(ray, out hit, 1000) && Input.GetMouseButtonDown(1) && hit.transform != transform) //attempt to move peasant
			{
				
				Tile target = hit.transform.gameObject.GetComponent<Tile>();
				
				if(target == null) //if the object that you clicked is not a tile, try to find the parent root s.t. it's a Tile.
				{
					Debug.Log ("BITCH PLEASE ");
					target = PathFind.findParentWithTag("Tile" , hit.transform.gameObject).GetComponent<Tile>(); //try to set target to parent (wherever it is) Tile of the object clicked
				}
				
				Debug.Log ("target is: " + target);
				
				
				NetworkViewID targetID = target.transform.GetComponent<NetworkView>().viewID;
				
				if (target.transform.GetComponent<Renderer>().material.color == Color.gray) //try to invade the destination tile...
				{
					GetComponent<NetworkView>().RPC("Invade", RPCMode.AllBuffered, targetID);
				}
				else //just perform move
				{
					GetComponent<NetworkView>().RPC("Move", RPCMode.AllBuffered, CurrentTile.GetComponent<NetworkView>().viewID, target.GetComponent<NetworkView>().viewID);
				}
				
				CurrentTile = transform.GetComponentInParent<Tile>(); //reset the currentTile to the tile this script is on. Do this even if the peasant OBJECT (in game) is not on this tile.
			}
		}
	}
	
	public void buildRoad()
	{
		this.updateBool("buildingRoad",true);
		this.setTurn(2);
		this.updateBool("canMove", false);
	}
	
	public void cultivateMeadow()
	{
		this.updateBool("cultivatingMeadow", true);
		this.setTurn(1);
		this.updateBool("canMove", false);
	}
	
	public void TileType (Tile destination)
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
	
	[RPC]
	public void HighlightPath(NetworkViewID currentTileID, NetworkViewID destinationID)
	{
		if(Network.isServer)
		{
			LinkedList<Tile> list = new LinkedList<Tile>();
			
			Tile CurrentTile = NetworkView.Find(currentTileID).transform.GetComponent<Tile>();
			Tile target = NetworkView.Find(destinationID).transform.GetComponent<Tile>();
			
			list = PathFinding.GetPathWithoutObstacles(CurrentTile, target);
			
			if(list != null && list.Count > 0)
			{
				foreach (Tile t in list)
				{
					if(t.HasTree || t.HasTomb)
					{
						t.updateBool("HighlightedYellow", true);
						break;
					}
					else
					{
						t.updateBool("Highlighted", true);
					}
				}
			}
			
		}
	}
	
	[RPC]
	public void ResetAllHighlight() //resets all tiles' highlighted to false...
	{
		if(Network.isServer)
		{
			//Debug.Log ("reset highlight called!");
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("Tile"))
			{
				Tile t = g.transform.GetComponent<Tile>();
				t.updateBool("Highlighted", false);
				t.updateBool("HighlightedYellow", false);
			}
		}
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
					CurrentTile.updateBool("HasPeasant", false);
					CurrentTile.updateBool("HasUnit", false);
					//Debug.Log ("After tile is: "+CurrentTile);
					destination.updateBool("HasPeasant", true);
					destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the empty square!
					Add(CurrentTile);
					//return true; //filler code
				}
				else if (destination.HasMeadow)
				{
					CurrentTile.updateBool("HasPeasant", false);
					CurrentTile.updateBool("HasUnit", false);
					destination.updateBool("HasPeasant", true);
					destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the meadow square!
					Add(CurrentTile);
				}
				
				// Test case 4: tile has a tree
				else if (destination.HasTree)
				{
					CurrentTile.updateBool("HasPeasant", false);
					CurrentTile.updateBool("HasUnit", false);
					
					destination.updateBool("HasPeasant", true);
					destination.updateBool("HasTree", false);
					destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
					Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
					Add(CurrentTile);					
				}
				
				// Test case 5: tile has a tombstone, move peasant there, but can't move again
				else if (destination.HasTomb)
				{
					CurrentTile.updateBool("HasPeasant", false);
					CurrentTile.updateBool("HasUnit", false);
					
					destination.updateBool("HasPeasant", true);
					destination.updateBool("HasTomb", false);
					destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", false);
					CurrentTile = destination; //now the little fucker's destination tile is on the tomb! YAY DEAD PEOPLE
					Add(CurrentTile);
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
	
	//this method will move ONE tile in the destination, and check whether that move is legal / what to do when the villager lands on it
	//This method will only allow a move on a tile in the village bundle.
	//If move is successful AND the villager can move again, return true
	//If move is unsuccessful OR the move is successful but the villager cannot move because he's blocked by something or if needs to stop (for example lands on a tomb)
	[RPC]
	public void Move (NetworkViewID currentTileID, NetworkViewID destinationID) //returns true only if successfully moved.
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
					//attempt to move to tile t (one unit distance) and if can't move anymore / illegal move, stop moving...
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
							
							CurrentTile.updateBool("HasUnit", false);
							destination.updateBool("HasPeasant", true);
							print("trying to move before");
							destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", true);
							print("trying to move after");
							CurrentTile.updateBool("HasPeasant", false);
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
							CurrentTile.updateBool("HasPeasant", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasPeasant", true);
							destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", true);
							CurrentTile = destination; //now the little fucker's destination tile is on the meadow square!
						}
						
						// Test case 4: tile has a tree
						else if (destination.HasTree)
						{
							CurrentTile.updateBool("HasPeasant", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasPeasant", true);
							destination.updateBool("HasTree", false);
							destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", false);
							CurrentTile = destination; //now the little fucker's destination tile is on the tree tile!
							
							Home.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, 1);
							break;
						}
						
						// Test case 5: tile has a tombstone, move peasant there, but can't move again
						else if (destination.HasTomb)
						{
							CurrentTile.updateBool("HasPeasant", false);
							CurrentTile.updateBool("HasUnit", false);
							
							destination.updateBool("HasPeasant", true);
							destination.updateBool("HasTomb", false);
							destination.Peasant.transform.GetComponent<Peasant>().updateBool("canMove", false);
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
							
							Peasant currentUnit = this;
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
	
	//checks to make sure tile t is of a different color as home tile's color, then adds that tile to the list of tiles in the village
	public void Add(Tile t)
	{
		// If the Home tile colour is not the same as the destination, change the colour of the destionation tile
		if (Home.transform.GetComponentInParent<Renderer>().material.color != t.gameObject.transform.GetComponent<Renderer>().material.color)
		{
			Home.AddTile(t);
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
	
	
	public void updateBool(string text, bool state)
	{
		GetComponent<NetworkView>().RPC("updateCanMoveHelper",RPCMode.AllBuffered, text, state);
	}
	
	[RPC]
	public void updateCanMoveHelper(string text, bool state)
	{
		if(text == "canMove")
			this.canMove = state;
		if (text == "buildingRoad")
			this.buildingRoad = state;
		if (text == "cultivatingMeadow")
			this.cultivatingMeadow = state;
	}
	
	public void updateTurn()
	{
		GetComponent<NetworkView>().RPC("updateTurnHelper", RPCMode.AllBuffered);
	}
	
	public void setTurn(int turn)
	{
		GetComponent<NetworkView>().RPC("setTurnHelper", RPCMode.AllBuffered,turn);
	}
	
	[RPC]
	public void setTurnHelper(int turn)
	{
		this.turnCounter = turn;
	}
	
	[RPC]
	public void updateTurnHelper()
	{
		if (this.turnCounter > 0)
			turnCounter--;
	}
	private void Merge(Tile destination)
	{
		Unit targetUnit = destination.GetComponent<Unit>();
		GetComponent<MergeUnits>().CheckForMerge(this, targetUnit, Home);
		_mergeUnit = null;
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
		if (Network.isServer)
		{
			Village v = CurrentTile.findVillage();
			
			if (v.Gold >= 10)
			{
				CurrentTile.updateBool("HasPeasant", false);
				CurrentTile.updateBool("HasInfantry", true);
				Home.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -10);
			}
			else
				Debug.Log ("Cannot upgrade: not enough gold or v.");
		}
	}
}