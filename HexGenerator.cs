using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class HexGenerator : MonoBehaviour
{
	
	public Transform Tile;
	public Vector2 GridSize;
	Transform[,] GridArray;
	public int Buffer;
	public float Offset;
	
	public int players;
	
	private int _counter;
	private string _gameSaveName;
	public GUISkin style;
	private bool _enter; 
	private bool _displaySaveBox;
	private bool _displayLoadBox;
	
	NetworkView nView;
	
	// Use this for initialization
	void Start()
	{
		_displayLoadBox = false;
		_displaySaveBox = false;
		_gameSaveName = "";
		players = Network.connections.Length+1;
		nView = GetComponent<NetworkView>();
		if (nView.isMine)
		{
			CreateGrid();
			AssignColor();
			CreateNeighbours();
			ClearSingles();
		}
		newTurn(Color.green);
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
	
	public void CreateGrid()
	{
		int x = (int)GridSize.x;
		int y = (int)GridSize.y;
		int maxXZ = Mathf.Max(x, y);
		Camera.main.transform.position = new Vector3(maxXZ / 2f, maxXZ * Buffer, maxXZ / 8f);
		GridArray = new Transform[x, y];
		Transform newHex;
		
		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				if (j % 2 == 0)
					newHex = (Transform)Network.Instantiate(Tile, new Vector3(i + Offset, 0, j) * Buffer, Quaternion.Euler(90, 90, 180), 0);
				else
					newHex = (Transform)Network.Instantiate(Tile, new Vector3(i, 0, j) * Buffer, Quaternion.Euler(90, 90, 180), 0);
				
				newHex.name = string.Format("({0},{1})", i, j);
				newHex.parent = transform;
				newHex.GetComponent<Tile>().Position = new Vector2(i, j);
				GridArray[i, j] = newHex;
				//nView.RPC("addList",RPCMode.AllBuffered, new object[] {nView.viewID, newHex.GetComponent<NetworkView>().viewID, i, j});
			}
		}
		
	}
	
	[RPC]
	
	public void addList(NetworkViewID hex, NetworkViewID tile, int i, int j)
	{
		NetworkView tile1 = NetworkView.Find(tile);
		NetworkView hex1 = NetworkView.Find(hex);
		tile1.transform.name = string.Format("({0},{1})", i, j);
		tile1.transform.parent = hex1.transform;
		tile1.gameObject.GetComponent<Tile>().Position = new Vector2(i, j);
		
	}
	public void Reshuffle(int[] num)
	{
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		for (int t = 0; t < num.Length; t++)
		{
			int tmp = num[t];
			int r = UnityEngine.Random.Range(t, num.Length);
			num[t] = num[r];
			num[r] = tmp;
		}
	}
	
	[RPC]
	void AssignColor()
	{
		
		int[] plArray = new int[324];
		
		for (int i = 0; i < plArray.Length; i++)
		{
			if (i < plArray.Length / 4)
				plArray[i] = 0;
			else if (i > plArray.Length / 4 && i < plArray.Length / 2)
				plArray[i] = 1;
			else if (i > plArray.Length / 2 + plArray.Length / 4)
				plArray[i] = 3;
			else
				plArray[i] = 2;
		}
		if (players == 2)
		{
			for (int i = 0; i < plArray.Length; i++)
			{
				if (i < plArray.Length / 3)
					plArray[i] = 0;
				else if (i > plArray.Length / 3 && i < plArray.Length / 1.5)
					plArray[i] = 1;
				else
					plArray[i] = 3;
			}
		}
		
		else if (players == 3)
		{
			for (int i = 0; i < plArray.Length; i++)
			{
				if (i < plArray.Length / 3)
					plArray[i] = 0;
				else if (i > plArray.Length / 3 && i < plArray.Length / 1.5)
					plArray[i] = 1;
				else if (i > plArray.Length / 2 + plArray.Length / 4)
					plArray[i] = 2;
				else
					plArray[i] = 3;
			}
		}
		
		int[] resArray = new int[324];
		
		for (int i = 0; i < resArray.Length; i++)
		{
			if (i < resArray.Length / 5)
				resArray[i] = 1;
			else if (i > resArray.Length / 5 + 1 && i < resArray.Length / 5 + resArray.Length / 10)
				resArray[i] = 2;
			else
				resArray[i] = 0;
		}
		
		Reshuffle(plArray);
		Reshuffle(resArray);
		
		int counter = 0;
		int counter2 = 0;
		
		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				
				Transform colorHex;
				colorHex = GridArray[i, j];
				
				
				if (i < 1 || i > GridSize.x - 2 || j < 1 || j > GridSize.y - 2 )
					colorHex.GetComponent<Tile>().updateColor(Color.blue);
				
				else
				{
					//colorHex.GetComponent<Tile>().updateColor(plArray[counter]);
					//counter++;
					switch (plArray[counter])
					{
					case 0:
						colorHex.GetComponent<Tile>().updateColor(Color.yellow);
						counter++;
						break;
					case 1:
						colorHex.GetComponent<Tile>().updateColor(Color.green);
						counter++;
						break;
					case 2:
						colorHex.GetComponent<Tile>().updateColor(Color.red);
						counter++;
						break;
					case 3:
						colorHex.GetComponent<Tile>().updateColor(Color.gray);
						counter++;
						break;
					}
					
					switch (resArray[counter2])
					{
					case 1:
						colorHex.GetComponent<Tile>().updateBool("HasTree", true);
						counter2++;
						break;
					case 2:
						colorHex.GetComponent<Tile>().updateBool("HasMeadow", true);
						counter2++;
						break;
					default:
						counter2++;
						break;
					}
				}
			}
		}
	}
	
	void CreateNeighbours()
	{
		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				Transform t = GridArray[i, j];
				//	Tile t = til.GetComponent<Tile>();
				int x = (int)t.GetComponent<Tile>().Position.x;
				int y = (int)t.GetComponent<Tile>().Position.y;
				
				if (y % 2 == 0)
				{
					if (x != 0)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x - 1, y]);
					
					if (y != GridSize.y - 1)
					{
						//			Debug.Log (t.Position);
						//			Debug.Log(t.GetComponent<Tile>().Neighbours[0]);
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x, y + 1]);
					}
					
					if (y != GridSize.y - 1 && x != GridSize.x - 1)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x + 1, y + 1]);
					
					if (x != GridSize.x - 1)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x + 1, y]);
					
					if (y != 0 && x != GridSize.x - 1)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x + 1, y - 1]);
					
					if (y != 0)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x, y - 1]);
				}
				
				else
				{
					if (x != 0)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x - 1, y]);
					
					if (y != GridSize.y - 1)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x, y + 1]);
					
					if (y != 0 && x != 0)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x - 1, y - 1]);
					
					if (x != GridSize.x - 1)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x + 1, y]);
					
					if (x != 0 && y != GridSize.y - 1)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x - 1, y + 1]);
					
					if (y != 0)
						t.GetComponent<Tile>().Neighbours.Add(GridArray[x, y - 1]);
				}
			}
		}
	}
	
	void ClearSingles()
	{
		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				Transform t = GridArray[i, j];
				_counter = 1;
				
				if (t.GetComponent<Renderer>().material.color != Color.gray
				    && t.GetComponent<Renderer>().material.color != Color.blue && !t.GetComponent<Tile>().Visited)
				{
					CLHelper(t);
					
					if (_counter < 3)
						t.GetComponent<Tile>().updateColor(Color.gray);
				}
			}
		}
		
		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				Transform t = GridArray[i, j];
				_counter = 0;
				
				foreach (Transform tile in t.GetComponent<Tile>().Neighbours)
				{
					if (t.GetComponent<Renderer>().material.color == tile.GetComponent<Renderer>().material.color)
						_counter++;
				}
				
				if (_counter == 0)
					t.GetComponent<Tile>().updateColor(Color.gray);
			}
		}
		resetVisitedAll();
	}
	
	public void resetVisitedAll() //resets visited to false for ALL tiles
	{
		//reset all the visited to false!
		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				Transform t = GridArray[i, j];
				t.GetComponent<Tile>().Visited = false;
			}
		}
	}
	
	void CLHelper(Transform t)
	{
		t.GetComponent<Tile>().Visited = true;
		
		List<Transform> nhbs = t.GetComponent<Tile>().Neighbours;
		
		foreach (Transform tile in nhbs)
		{
			if (!tile.GetComponent<Tile>().Visited)
			{
				if (t.GetComponent<Renderer>().material.color == tile.GetComponent<Renderer>().material.color)
				{
					tile.GetComponent<Tile>().Visited = true;
					_counter++;
					
					if (_counter == 3)
					{
						tile.GetComponent<Tile>().updateBool("HasVillage", true);
						tile.GetComponent<Tile>().updateBool("HasHovel", true);
					}
					
					CLHelper(tile);
				}
			}
		}
	}
	
	public void growTrees()
	{
		if (Network.isServer)
		{
			for (int i = 0; i < GridSize.x; i++)
			{
				for (int j = 0; j < GridSize.y; j++)
				{
					Transform t = GridArray[i, j];
					Tile currentTile = t.GetComponent<Tile>();
					if (currentTile.HasTree)
					{
						//bool treeGrowth = false;
						if (UnityEngine.Random.value < 0.35)
						{
							foreach (Transform neigh in currentTile.Neighbours)
							{
								Tile neighborTile = neigh.GetComponent<Tile>();
								if (((neighborTile.IsEmpty || neighborTile.HasMeadow) && !neighborTile.HasRoad) && neigh.GetComponent<Renderer>().material.color != Color.blue)
								{
									neighborTile.updateBool("HasTree", true);
									neighborTile.updateBool("HasMeadow", false);
									//treeGrowth = true;
									break;
								}
							}
						}
					}
				}
			}
		}
	}
	
	public void newTurn(Color color)
	{
		if (Network.isServer)
		{
			for (int i = 0; i < GridSize.x; i++)
			{
				for (int j = 0; j < GridSize.y; j++)
				{
					Transform t = GridArray[i, j];
					Tile currentTile = t.GetComponent<Tile>();
					
					if (currentTile.GetComponent<Renderer>().material.color == color)
					{
						if (currentTile.HasTomb)
						{
							currentTile.updateBool("HasTomb", false);
							currentTile.updateBool("HasTree", true);
						}
					}
				}
			}
			
			for (int i = 0; i < GridSize.x; i++)
			{
				for (int j = 0; j < GridSize.y; j++)
				{
					Transform t = GridArray[i, j];
					Tile currentTile = t.GetComponent<Tile>();
					
					if (currentTile.GetComponent<Renderer>().material.color == color)
					{
						if (currentTile.HasPeasant)
						{
							Peasant p = t.FindChild("Peasant").GetComponent<Peasant>();
							p.updateTurn();
							if (p.turnCounter == 0)
							{
								if (p.cultivatingMeadow)
								{
									p.updateBool("cultivatingMeadow", false);
									currentTile.updateBool("HasMeadow", true);
								}
								
								if (p.buildingRoad)
								{
									
									p.updateBool("buildingRoad", false);
									currentTile.updateBool("HasRoad", true);
								}
								
								p.updateBool("canMove", true);
							}
						}
						else if (currentTile.HasInfantry)
						{
							Infantry p = t.FindChild("Infantry").GetComponent<Infantry>();
							p.updateBool("canMove", true);
						}
						else if (currentTile.HasSoldier)
						{
							Soldier p = t.FindChild("Soldier").GetComponent<Soldier>();
							p.updateBool("canMove", true);
						}
						else if (currentTile.HasKnight)
						{
							Knight p = t.FindChild("Knight").GetComponent<Knight>();
							p.updateBool("canMove", true);
						}
						else if (currentTile.HasCannon)
						{
							Cannon p = t.FindChild("Cannon").GetComponent<Cannon>();
							p.updateBool("canMove", true);
						}
					}
				}
			}
			for (int i = 0; i < GridSize.x; i++)
			{
				for (int j = 0; j < GridSize.y; j++)
				{
					Transform t = GridArray[i, j];
					Tile currentTile = t.GetComponent<Tile>();
					
					if (currentTile.GetComponent<Renderer>().material.color == color)
					{
						if (currentTile.HasMeadow)
						{
							Village v = currentTile.findVillage();
							v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, 2);
						}
						if (currentTile.IsEmpty || currentTile.HasUnit || currentTile.HasVillage)
						{
							Village v = currentTile.findVillage();
							v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, 1);
						}
					}
				}
			}
			for (int i = 0; i < GridSize.x; i++)
			{
				for (int j = 0; j < GridSize.y; j++)
				{
					Transform t = GridArray[i, j];
					Tile currentTile = t.GetComponent<Tile>();
					
					if (currentTile.GetComponent<Renderer>().material.color == color)
					{
						Village v = currentTile.findVillage();
						
						if (currentTile.HasUnit)
						{
							if (currentTile.HasPeasant)
							{
								v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -2);
							}
							else if (currentTile.HasInfantry)
							{
								v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -6);
							}
							
							else if (currentTile.HasSoldier)
							{
								v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -18);
							}
							
							else if (currentTile.HasKnight)
							{
								v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -64);
							}
							else if (currentTile.HasCannon)
							{
								v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -5);
							}
						}
						else
						{
							if (currentTile.HasCastle)
							{
								v.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -80);
							}
						}
						
						if (v.Gold < 0)
						{
							v.KillVillagers();
						}
					}
				}
			}
		}
	}
	
	void OnGUI()
	{
		GUI.skin = style;
		
		_enter = false;
		Event e = Event.current;        
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)        
			_enter = true;
		
		if (!_displaySaveBox && GUI.Button(new Rect(Screen.width-120f, Screen.height-85f, 100, 30), "Save Game"))
		{
			_displaySaveBox = true;
		}
		if (_displaySaveBox)
		{
			GUI.BeginGroup( new Rect(Screen.width*0.25f, Screen.height * 0.2f, Screen.width*0.5f, 30f));
			GUI.Label(new Rect(0, 0, Screen.width*0.5f, 30f), "<size=14> Please enter save file name:  </size>");
			GUI.EndGroup ();
			GUI.BeginGroup( new Rect(Screen.width * 0.5f-100f, Screen.height * 0.5f, 300f, 100f));
			
			_gameSaveName = GUILayout.TextField(_gameSaveName, GUILayout.Width (200));
			
			if (GUI.Button(new Rect (50,30,80,20), "Save") || _enter)
			{
				if (!string.IsNullOrEmpty(_gameSaveName.Trim()))
				{
					Save(_gameSaveName);
					print ("saved to" + Application.dataPath + "/SavedGames/"+_gameSaveName);
					_displaySaveBox = false;
				}
				
			}
			GUI.EndGroup();
		}
		
		if (!_displayLoadBox && GUI.Button(new Rect(Screen.width-120f, Screen.height-50f, 100, 30), "Load Game"))
		{
			_displayLoadBox = true;
		}
		if (_displayLoadBox)
		{
			GUI.BeginGroup( new Rect(Screen.width*0.25f, Screen.height * 0.2f, Screen.width*0.5f, 30f));
			GUI.Label(new Rect(0, 0, Screen.width*0.5f, 30f), "<size=14> Please enter load file name:  </size>");
			GUI.EndGroup ();
			GUI.BeginGroup( new Rect(Screen.width * 0.5f-100f, Screen.height * 0.5f, 300f, 100f));
			
			_gameSaveName = GUILayout.TextField(_gameSaveName, GUILayout.Width (200));
			
			if (GUI.Button(new Rect (50,30,80,30), "Load") || _enter)
			{
				if (!string.IsNullOrEmpty(_gameSaveName.Trim()))
				{
					Load(_gameSaveName);
					print ("loaded from" + Application.dataPath + "/SavedGames/"+_gameSaveName);
					_displayLoadBox = false;
				}
				
			}
			GUI.EndGroup();
		}
	}
	// For folder to show up
	public void Save(String saveName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.dataPath + "/SavedGames/" + saveName + ".dat");
		
		mapArray mData = new mapArray();
		
		for (int i = 0; i < GridSize.x; i++)
		{
			for (int j = 0; j < GridSize.y; j++)
			{
				mapData data = new mapData();
				
				Transform tile = GridArray[i, j];
				Tile t = tile.GetComponent<Tile>();
				Color col = tile.GetComponent<Renderer>().material.color;
				if (col == Color.gray)
					data.color = 0;
				else if (col == Color.blue)
					data.color = 1;
				else if (col == Color.green)
					data.color = 2;
				else if (col == Color.yellow)
					data.color = 3;
				else if (col == Color.red)
					data.color = 4;
				
				data.HasHovel = t.HasHovel;
				data.HasTown = t.HasTown;
				data.HasFort = t.HasFort;
				data.HasCastle = t.HasCastle;
				data.HasTree = t.HasTree;
				data.HasMeadow = t.HasMeadow;
				data.HasTomb = t.HasTomb;
				data.HasRoad = t.HasRoad;
				data.HasPeasant = t.HasPeasant;
				data.HasInfantry = t.HasInfantry;
				data.HasSoldier = t.HasSoldier;
				data.HasKnight = t.HasKnight;
				data.HasCannon = t.HasCannon;
				data.HasWatchTower = t.HasWatchTower;
				data.HasVillage = t.HasVillage;
				data.HasUnit = t.HasUnit;
				data.IsEmpty = t.IsEmpty;
				
				if (t.HasVillage)
				{
					data.gold = t.Village.GetComponent<Village>().Gold;
					data.wood = t.Village.GetComponent<Village>().Wood;
					data.shots = t.Village.GetComponent<Village>().CannonballShotsTaken;
				}
				
				mData.mArray[i, j] = data;
				
			}
		}
		
		bf.Serialize(file, mData);
		file.Close();
	}
	
	public void Load(String fileName)
	{
		if (File.Exists(Application.dataPath + "/SavedGames/"+ fileName +".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.dataPath + "/SavedGames/"+ fileName +".dat", FileMode.Open);
			mapArray mData = (mapArray)bf.Deserialize(file);
			file.Close();
			
			for (int i = 0; i < GridSize.x; i++)
			{
				for (int j = 0; j < GridSize.y; j++)
				{
					mapData data = mData.mArray[i, j];
					
					Transform tile = GridArray[i, j];
					Tile t = tile.GetComponent<Tile>();
					if (data.color == 0)
						t.updateColor(Color.gray);
					else if (data.color == 1)
						t.updateColor(Color.blue);
					else if (data.color == 2)
						t.updateColor(Color.green);
					else if (data.color == 3)
						t.updateColor(Color.yellow);
					else if (data.color == 4)
						t.updateColor(Color.red);
					
					t.updateBool("HasHovel", data.HasHovel);
					t.updateBool("HasTown", data.HasTown);
					t.updateBool("HasFort", data.HasFort);
					t.updateBool("HasCastle", data.HasCastle);
					t.updateBool("HasTree", data.HasTree);
					t.updateBool("HasMeadow", data.HasMeadow);
					t.updateBool("HasTomb", data.HasTomb);
					t.updateBool("HasRoad", data.HasRoad);
					t.updateBool("HasPeasant", data.HasPeasant);
					t.updateBool("HasInfantry", data.HasInfantry);
					t.updateBool("HasSoldier", data.HasSoldier);
					t.updateBool("HasKnight", data.HasKnight);
					t.updateBool("HasCannon", data.HasCannon);
					t.updateBool("HasWatchTower", data.HasWatchTower);
					t.updateBool("HasVillage", data.HasVillage);
					t.updateBool("HasUnit", data.HasUnit);
					t.updateBool("isEmpty", data.IsEmpty);
					
					if (data.HasVillage)
					{
						t.Village.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -t.Village.GetComponent<Village>().Gold + data.gold);
						t.Village.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, -t.Village.GetComponent<Village>().Wood + data.wood);
						t.Village.GetComponent<NetworkView>().RPC("updateCannonShots", RPCMode.AllBuffered, -t.Village.GetComponent<Village>().CannonballShotsTaken + data.shots);
					}
				}
			}
		}
	}
}


[Serializable]
class mapData
{
	public bool[] dataArray;
	public int color = 0;
	public bool HasHovel = false;
	public bool HasTown = false;
	public bool HasFort = false;
	public bool HasCastle = false;
	public bool HasTree = false;
	public bool HasMeadow = false;
	public bool HasTomb = false;
	public bool HasRoad = false;
	
	public bool IsEmpty = true;
	
	public bool HasPeasant = false;
	public bool HasInfantry = false;
	public bool HasSoldier = false;
	public bool HasKnight = false;
	public bool HasCannon = false;
	public bool HasWatchTower = false;
	
	public bool HasVillage = false;
	public bool HasUnit = false;
	
	public int gold = 0;
	public int wood = 0;
	public int shots = 0;
	
	public mapData()
	{
		dataArray = new bool[20];
		color = 0;
	}
}

[Serializable]
class mapArray
{
	public mapData[,] mArray;
	
	public mapArray()
	{
		mArray = new mapData[20, 20];
	}
}




