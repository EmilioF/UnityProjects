using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Tile : MonoBehaviour
{
	public Vector2 Position;
	public bool Visited = false;
	public List<Transform> Neighbours = new List<Transform>();

	public Color myColor;

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

	public bool Highlighted;
	public bool HighlightedRed;
	public bool HighlightedYellow;

	public Tile pathParent;

	public GameObject Tree;
	public GameObject Grave;
	public GameObject Village;
	public GameObject Meadow;
	public GameObject Road;

	public GameObject Peasant;
	public GameObject Infantry;
	public GameObject Soldier;
	public GameObject Knight;
	public GameObject Cannon;
	public GameObject WatchTower;

	public GameObject Hovel;
	public GameObject Town;
	public GameObject Fort;
	public GameObject Castle;

	public GameObject Highlight;
	public GameObject HighlightRed;
	public GameObject HighlightYellow;

	//GUI
//	private bool _upgradeUnit;
//	public GUISkin style;

	NetworkView netView;

	// Use this for initialization
	void Start()
	{
//		_upgradeUnit = false;

		Tree.SetActive(false);
		Village.SetActive(false);
		Grave.SetActive(false);
		Meadow.SetActive(false);
		Road.SetActive(false);
		Peasant.SetActive(false);
		Infantry.SetActive(false);
		Soldier.SetActive(false);
		Knight.SetActive(false);
		Cannon.SetActive(false);

		Hovel.SetActive(false);
		Town.SetActive(false);
		Fort.SetActive(false);
		Castle.SetActive(false);

		WatchTower.SetActive(false);

		Highlight.SetActive(false);
		HighlightRed.SetActive(false);
		HighlightYellow.SetActive(false);

	}

	// Update is called once per frame
	void Update()
	{
		if (HasVillage)
		{
			HasTree = false;
			HasMeadow = false;
			Village.SetActive(true);
		}
		else
			Village.SetActive(false);

		if (HasTree)
			Tree.SetActive(true);
		else
			Tree.SetActive(false);

		if (HasMeadow)
			Meadow.SetActive(true);
		else
			Meadow.SetActive(false);

		if (HasTomb)
			Grave.SetActive(true);
		else
			Grave.SetActive(false);

		if (HasRoad)
			Road.SetActive(true);
		else
			Road.SetActive(false);

		if (HasPeasant)
		{
			Peasant.SetActive(true);
			HasUnit = true;
		}
		else
			Peasant.SetActive(false);

		if (HasInfantry)
		{
			Infantry.SetActive(true);
			HasUnit = true;
		}
		else
			Infantry.SetActive(false);

		if (HasSoldier)
		{
			Soldier.SetActive(true);
			HasUnit = true;
		}
		else
			Soldier.SetActive(false);

		if (HasKnight)
		{
			Knight.SetActive(true);
			HasUnit = true;
		}
		else
			Knight.SetActive(false);

		if (HasCannon)
		{
			Cannon.SetActive(true);
			HasUnit = true;
		}
		else
		{
			Cannon.SetActive(false);
		}

		if(HasWatchTower)
		{
			WatchTower.SetActive(true);
			HasUnit = true;
		}
		else
		{
			WatchTower.SetActive(false);
		}

		if (HasHovel)
		{
			Hovel.SetActive(true);
			HasVillage = true;
		}
		else
		{
			Hovel.SetActive(false);
		}
		if (HasTown)
		{
			Town.SetActive(true);
			HasVillage = true;
		}
		else
		{
			Town.SetActive(false);
		}
		if (HasFort)
		{
			Fort.SetActive(true);
			HasVillage = true;
		}
		else
		{
			Fort.SetActive(false);
		}
		if (HasCastle)
		{
			Castle.SetActive(true);
			HasVillage = true;
		}
		else
		{
			Castle.SetActive(false);
		}

		if (Highlighted)
		{
			Highlight.SetActive(true);
		}
		else {
			Highlight.SetActive(false);
		}

		if (HighlightedRed)
		{
			HighlightRed.SetActive(true);
		}
		else {
			HighlightRed.SetActive(false);
		}

		if (HighlightedYellow)
		{
			HighlightYellow.SetActive(true);
		}
		else {
			HighlightYellow.SetActive(false);
		}

		if (!HasVillage && !HasTree && !HasTomb && !HasMeadow && !HasPeasant
		    && !HasInfantry && !HasSoldier && !HasKnight && !HasCannon && !HasWatchTower && !HasRoad
		    && !HasHovel && !HasFort && !HasCastle && !HasTown)
			IsEmpty = true;
		else
			IsEmpty = false;

		if (!HasUnit)
		{
			Peasant.GetComponent<Peasant>().Chosen = false;
			Infantry.GetComponent<Infantry>().Chosen = false;
			Soldier.GetComponent<Soldier>().Chosen = false;
			//Knight.GetComponent<Knight>().Chosen = false;
//			_upgradeUnit = true;
		}
//		else
//			_upgradeUnit = false;
	}

//	void OnGUI(){
//		GUI.skin = style;
//
//		if(_upgradeUnit)
//		{
//			GUI.BeginGroup( new Rect(Screen.width*0.5f-150f, 0f, 300f, 30f));
//			if(GUI.Button(new Rect(0, 0, 300, 30), "Upgrade Unit"))
//			{
//				Unit currentUnit = null;
//
//				if (HasPeasant)
//					currentUnit = Peasant.transform.GetComponent<Peasant>();
//				else if (HasInfantry)
//					currentUnit = Infantry.transform.GetComponent<Infantry>();
//				else if (HasSoldier)
//					currentUnit = Soldier.transform.GetComponent<Soldier>();
//				else if (HasKnight)
//					currentUnit = Knight.transform.GetComponent<Knight>();
//
//				Village v = Village.transform.GetComponent<Village>();
//				if (currentUnit != null)
//					GetComponent<UpgradeUnit>().CheckForUpgrade(currentUnit, v);
//				else
//					Debug.Log("current unit is null");
//			}
//			GUI.EndGroup ();
//		}
//	}

	void print()
	{
		foreach (Transform tile in Neighbours)
			Debug.Log(tile.GetComponent<Tile>().Position);
	}

	public void updateColor(Color color)
	{
		int col = 0;

		netView = GetComponent<NetworkView>();

		if (color == Color.yellow)
			col = 0;
		if (color == Color.green)
			col = 1;
		if (color == Color.red)
			col = 2;
		if (color == Color.gray)
			col = 3;
		if (color == Color.blue)
			col = 4;


		netView.RPC("updateColorHelp", RPCMode.AllBuffered, new object[] { col });

	}

	[RPC]
	public void updateColorHelp(int color)
	{
		switch (color)
		{
		case 0:
			gameObject.GetComponent<Renderer>().material.color = Color.yellow;
			break;
		case 1:
			gameObject.GetComponent<Renderer>().material.color = Color.green;
			break;
		case 2:
			gameObject.GetComponent<Renderer>().material.color = Color.red;
			break;
		case 3:
			gameObject.GetComponent<Renderer>().material.color = Color.gray;
			break;
		case 4:
			gameObject.GetComponent<Renderer>().material.color = Color.blue;
			break;
		}
	}

	public void updateBool(string text, bool state)
	{
		netView = GetComponent<NetworkView>();
 		netView.RPC("updateBoolHelp", RPCMode.AllBuffered, new object[] { text, state });

	}

	[RPC]
	public void updateBoolHelp(string text, bool state)
	{
		if (text == "HasMeadow")
			this.HasMeadow = state;
		if (text == "HasTree")
			this.HasTree = state;
		if (text == "HasVillage")
			this.HasVillage = state;
		if (text == "HasTomb")
			this.HasTomb = state;
		if (text == "HasPeasant")
		{
			this.HasPeasant = state;
			this.Peasant.SetActive(state);
		}
		if (text == "HasInfantry")
		{
			this.HasInfantry = state;
			this.Infantry.SetActive(state);
		} if (text == "HasSoldier")
		{
			this.HasSoldier = state;
			this.Soldier.SetActive(state);
		} if (text == "HasUnit")
			this.HasUnit = state;
		if (text == "HasKnight")
		{
			this.HasKnight = state;
			this.Knight.SetActive(state);
		}
		if (text == "HasCannon")
		{
			this.HasCannon = state;
			this.Cannon.SetActive(state);
		} 
		if (text == "HasWatchTower")
		{
			this.HasWatchTower = state;
			this.WatchTower.SetActive(state);
		} if (text == "HasHovel")
		{
			this.HasHovel = state;
			this.Hovel.SetActive(state);
			GameObject.Find("Player(Clone)").GetComponent<Player>().updateNumberOfVillages(transform.GetComponent<Renderer>().material.color, state);

		}
		if (text == "HasTown")
		{
			this.HasTown = state;
			this.Town.SetActive(state);
			GameObject.Find("Player(Clone)").GetComponent<Player>().updateNumberOfVillages(transform.GetComponent<Renderer>().material.color, state);
		}
		if (text == "HasFort")
		{
			this.HasFort = state;
			this.Fort.SetActive(state);
			GameObject.Find("Player(Clone)").GetComponent<Player>().updateNumberOfVillages(transform.GetComponent<Renderer>().material.color, state);
		}
		if (text == "HasCastle")
		{
			this.HasCastle = state;
			this.Castle.SetActive(state);
			GameObject.Find("Player(Clone)").GetComponent<Player>().updateNumberOfVillages(myColor, state);
		}
		if (text == "HasRoad")
			this.HasRoad = state;
		if (text == "Highlighted")
			this.Highlighted = state;
		if (text == "HighlightedRed")
			this.HighlightedRed = state;
		if (text == "HighlightedYellow")
			this.HighlightedYellow = state;
	}
	public Village findVillage()
	{
		List<Tile> tilelist = new List<Tile>();
		tilelist = GetTiles(this);

		foreach (Tile t in tilelist)
		{
			if (t.HasVillage)
				return t.Village.transform.GetComponent<Village>();
		}

		return null;
	}

	public bool isGuarded(string unit)
	{
		foreach(Transform t in Neighbours)
		{
			if (t.GetComponent<Renderer>().material.color == this.transform.GetComponent<Renderer>().material.color)
			{
				Tile neigh = t.GetComponent<Tile>();

				if (neigh.HasInfantry)
				{
					if (unit == "Infantry")
						return true;
				}
				if (neigh.HasSoldier)
				{
					if (unit == "Soldier")
						return true;
				}
				if (neigh.HasKnight)
				{
					if (unit == "Knight")
						return true;
				}
			}
		}
		return false;
	}

	// Get a list of all the tiles that are connected using BFS
	public List<Tile> GetTiles(Tile root)
	{
		List<Tile> tileList = new List<Tile>();

		Queue<Tile> q = new Queue<Tile>();

		q.Enqueue(root);

		while (q.Count > 0)
		{
			// pop the first item in the queue
			Tile current = q.Dequeue();
			tileList.Add(current);

			// if nothing pops, then all tiles have been found
			if (current == null)
				continue;

			// loop through all of root's neighbouring tiles
			foreach (Transform tile in current.Neighbours)
			{
				myColor = root.GetComponent<Renderer>().material.color;
				Color tileColor = tile.GetComponent<Renderer>().material.color;

				// get the tile component from the transform
				Tile t = tile.GetComponent<Tile>();

				// if the two colours match, then the tile is a neighbour
				// Enqueue the tile if it is not found in _tiles and in q
				if (myColor == tileColor && !(tileList.Contains(t)) && !(q.Contains(t)))
					q.Enqueue(t);
			}
		}
		return tileList;
	}

}