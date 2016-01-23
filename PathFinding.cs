using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
	public List<Tile> _path;
	
	public List<Tile> _tiles;
	private Color _color;
	
	
	// Use this for initialization
	void Start ()
	{
		_tiles = new List<Tile>();
		//_tiles = GetTiles(_root);
	}
	
	// FIND PARENT WITH TAG
	public GameObject findParentWithTag(string tagToFind) {
		return findParentWithTag(tagToFind, this.gameObject);
	}
	
	public GameObject findParentWithTag(string tagToFind, GameObject startingObject) {
		Transform parent = startingObject.transform.parent;
		while (parent != null) { 
			if (parent.tag == tagToFind) {
				return parent.gameObject as GameObject;
			}
			parent = parent.transform.parent;
		}
		return null;
	}
	
	// Get a list of all the tiles that are connected using BFS IN THE VILLAGE BUNDLE
	public List<Tile> GetTiles (Tile root)
	{
		Queue<Tile> q = new Queue<Tile>();
		
		q.Enqueue(root);
		
		while (q.Count > 0)
		{
			// pop the first item in the queue
			Tile current = q.Dequeue();
			_tiles.Add(current);
			
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
				if (_color == tileColor && !(_tiles.Contains(t)) && !(q.Contains(t)))
					q.Enqueue(t);
			}
		}
		
		return _tiles;
	}
	
	//BFS Alg w/o any particular heuristic (NOT A*) found from http://www.peachpit.com/articles/article.aspx?p=101142
	public static LinkedList<Tile> GetPath (Tile start, Tile end)
	{		
		//list of visited tiles
		List<Tile> closedList = new List<Tile>();
		
		//list of tiles	to visit
		Queue<Tile> openList = new Queue<Tile>();
		openList.Enqueue(start);
		start.pathParent = null;
		
		while (openList.Count != 0)
		{
			Tile t = openList.Dequeue();
			if(t == end)
			{
				//path found!
				return constructPath(end);
			}
			else 
			{
				closedList.Add (t);
				//add neighbors to open list
				foreach (Transform neighbor in t.Neighbours)
				{
					Tile neighborTile = neighbor.GetComponent<Tile>();
					
					if(!closedList.Contains(neighborTile) && !openList.Contains(neighborTile) && 
					   neighbor.GetComponent<Renderer>().material.color == start.transform.GetComponent<Renderer>().material.color)
						
						//					if(!closedList.Contains(neighborTile) && !openList.Contains(neighborTile))
					{
						neighborTile.pathParent = t;
						openList.Enqueue(neighborTile);
					}
				}	
			}
		}
		
		//Debug.Log ("No path found...");
		return null; //no path found
	}
	
	
	//BFS Alg, same as above method, except can't have obstacles (such as villagers, village) in the way.
	public static LinkedList<Tile> GetPathWithoutObstacles (Tile start, Tile end)
	{		
		//list of visited tiles
		List<Tile> closedList = new List<Tile>();
		
		//list of tiles	to visit
		Queue<Tile> openList = new Queue<Tile>();
		openList.Enqueue(start);
		start.pathParent = null;
		
		while (openList.Count != 0)
		{
			Tile t = openList.Dequeue();
			if(t == end)
			{
				//path found!
				return constructPath(end);
			}
			else 
			{
				closedList.Add (t);
				//add neighbors to open list
				foreach (Transform neighbor in t.Neighbours)
				{
					Tile neighborTile = neighbor.GetComponent<Tile>();
					
					if(!closedList.Contains(neighborTile) && !openList.Contains(neighborTile) && 
					   neighbor.GetComponent<Renderer>().material.color == start.transform.GetComponent<Renderer>().material.color 
					   && !neighborTile.HasVillage && !neighborTile.HasTree && (!neighborTile.HasMeadow && !neighborTile.HasRoad)) //if same color and no village or unit is on that neighbor tile
						
						//					if(!closedList.Contains(neighborTile) && !openList.Contains(neighborTile))
					{
						neighborTile.pathParent = t;
						openList.Enqueue(neighborTile);
					}
				}	
			}
		}
		
		//Debug.Log ("No path found...");
		return null; //no path found
	}
	
	public static LinkedList<Tile> constructPath(Tile t) //returns list starting from start to end
	{
		LinkedList<Tile> path = new LinkedList<Tile>();
		while(t.pathParent != null)
		{
			path.AddFirst(t);
			t = t.pathParent;
		}
		
		Debug.Log ("Path is: ");
		foreach (Tile tile in path)
		{
			Debug.Log (tile);
		}
		
		return path;
	}
}

