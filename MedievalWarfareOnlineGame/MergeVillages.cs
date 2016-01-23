using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MergeVillages : MonoBehaviour
{
	public List<Tile> Tiles;
	
	public Village Village1; // our current village
	public Village Village2; // possible village to merge with
	public Village Village3; // possible village to merge with
	
	private Color _color;
	private Tile _root;
	
	private Village _newVillage;
	
	void Start()
	{
		Tiles = new List<Tile>();
		
		// Get this village
		Village1 = gameObject.GetComponent<Village>();
		
		// Get the tile the village belongs to
		_root = Village1.CurrentTile;
		
		// Get all the tiles in that village
		Tiles = GetTiles(_root);
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
				_color = root.GetComponent<Renderer>().material.color;
				Color tileColor = tile.GetComponent<Renderer>().material.color;
				
				// get the tile component from the transform
				Tile t = tile.GetComponent<Tile>();
				
				// if the two colours match, then the tile is a neighbour
				// Enqueue the tile if it is not found in _tiles and in q
				if (_color == tileColor && !(tileList.Contains(t)) && !(q.Contains(t)))
					q.Enqueue(t);
			}
		}
		return tileList;
	}
	
	// Check if it's possible to merge two villages together
	public void CheckForMerge()
	{
		// Get a new list of linked tiles
		Tiles = GetTiles(_root);
		
		//Debug.Log("Total number of village tiles is: " + Tiles.Count);
		
		// we start with 1 village
		int count = 1;
		
		foreach (Tile t in Tiles)
		{
			if (t.HasVillage)
			{
				// if village found is not the current village
				if (Village1 != t.Village.GetComponent<Village>() && Village2 == null)
				{
					// save this village in a variable called village2
					Village2 = t.Village.GetComponent<Village>();
					count++;
				}
				
				// else if the village found is not village1 nor village2
				else if (Village1 != t.Village.GetComponent<Village>() && Village2 != t.Village.GetComponent<Village>())
				{
					// save this village in a variable called village3
					Village3 = t.Village.GetComponent<Village>();
					count++;
				}
			}
		}
		
		// if two villages are found
		if (count == 2)
		{
			MergeTwo();
			Village2 = null; // after merging, reset the village var to null to be reused
		}
		
		// if three villages are found
		else if (count == 3)
		{
			MergeThree();
			Village2 = null;
			Village3 = null;
		}
	}
	
	public void MergeTwo()
	{
		// the new village is the one that will remain if the two villages merge
		_newVillage = CompareVillages(Village1, Village2);
		
		// the new village will have all the resources (wood, gold, tiles)
		_newVillage.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, -_newVillage.Wood + Village1.Wood + Village2.Wood);
		_newVillage.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -_newVillage.Gold + Village1.Gold + Village2.Gold);
		_newVillage.VillageTerritory = Tiles;
		
		// if the new village is the current village
		if (_newVillage == Village1)
			DestroyVillage(Village2);
		
		// else if the new village is the other village 
		else
			DestroyVillage(Village1);
	}
	
	public void MergeThree()
	{
		// the new village is the one that will remain if the three villages merge
		Village temp = CompareVillages(Village1, Village2);
		_newVillage = CompareVillages(temp, Village3);
		
		// the new village will have all the resources (wood, gold, tiles)
		_newVillage.GetComponent<NetworkView>().RPC("updateWood", RPCMode.AllBuffered, -_newVillage.Wood + Village1.Wood + Village2.Wood + Village3.Wood);
		_newVillage.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -_newVillage.Gold + Village1.Gold + Village2.Gold + Village3.Gold);
		_newVillage.VillageTerritory = Tiles;
		
		// if the new village is the current village
		// then the other two villages' flags are turned off
		if (_newVillage == Village1)
		{
			DestroyVillage(Village2);
			DestroyVillage(Village3);
		}
		
		// else if the new village is village2
		else if (_newVillage == Village2)
		{
			DestroyVillage(Village1);
			DestroyVillage(Village3);
		}
		// else if the new village is village 3
		else
		{
			DestroyVillage(Village1);
			DestroyVillage(Village2);
		}
	}
	
	// compare two villages together
	public Village CompareVillages(Village a, Village b)
	{
		int aSize = a.VillageTerritory.Count;
		int bSize = b.VillageTerritory.Count;
		
		// first compare the rank (Hovel, Fort, Town)
		if (a.ThisRank.CompareTo(b.ThisRank) > 0)
			return a;
		else if (a.ThisRank.CompareTo(b.ThisRank) < 0)
			return b;
		// otherwise, compare the size of the territory
		else
		{
			if (aSize > bSize)
				return a;
			else
				return b;
		}
	}
	
	public void DestroyVillage(Village v)
	{
		
		v.CurrentTile.updateBool("HasVillage", false);
		v.CurrentTile.updateBool("HasMeadow", true);
		v.CurrentTile.updateBool("HasHovel", false);
		v.CurrentTile.updateBool("HasFort", false);
		v.CurrentTile.updateBool("HasTown", false);
		v.CurrentTile.updateBool("HasCastle", false);
		
	}
}