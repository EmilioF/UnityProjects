using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplitVillages : MonoBehaviour
{
    private Tile _currentTile;
    private Color _color; // colour of the player we're currently invading
    private List<List<Tile>> _list;

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

    // check for possible splitting in the village
    public void CheckForSplit(Color color, Tile currentTile)
    {
        Debug.Log("Checking for split.");

        _color = color;
        _currentTile = currentTile;

        _list = new List<List<Tile>>();
        List<Tile> temp = new List<Tile>();
        List<Tile> tileList = new List<Tile>();

        // Get a list of all list of tiles
        foreach (Transform tile in _currentTile.Neighbours)
        {
            // get the tile component from the transform
            Tile t = tile.GetComponent<Tile>();

            Color tileColor = t.transform.GetComponentInParent<Renderer>().material.color;

            if (tileColor == color && !temp.Contains(t))
            {
                tileList = GetTiles(t);

                Debug.Log("Tile t is: " + t);
                //				Debug.Log ("Tile list size is: " + tileList.Count);
                //				Debug.Log ("List is: " + _list + " or tilelist is: " + tileList);

                if (!(_list.Contains(tileList)))
                {
                    //					Debug.Log ("Tilelist is not in _list.");
                    _list.Add(tileList);
                    temp.AddRange(tileList);
                }

                if (CheckSize(tileList))
                {
                    TurnNeutral(tileList);
                }
            }
        }

        if (_list.Count > 1)
        {
            Debug.Log("There are " + _list.Count + " lists.");
            Split();
        }

        // reset all lists
        _list = new List<List<Tile>>();
        temp = new List<Tile>();
        tileList = new List<Tile>();
    }

    public void Split()
    {
        foreach (List<Tile> tl in _list)
        {
            if (CheckForVillage(tl))
            {
                GenerateVillage(tl);
            }
        }
    }

    public bool CheckForVillage(List<Tile> tileList)
    {
        foreach (Tile t in tileList)
        {
            if (t.HasVillage)
                return false;
        }

        return true;
    }

    public bool CheckSize(List<Tile> tileList)
    {
        if (tileList.Count < 3)
            return true;

        return false;
    }

    public void TurnNeutral(List<Tile> tileList)
    {
        foreach (Tile t in tileList)
        {
            t.updateColor(Color.gray);

            if (t.HasVillage)
            {
                t.updateBool("HasVillage", false);
                t.updateBool("HasTree", true);
                t.updateBool("HasHovel", false);
                t.updateBool("HasFort", false);
                t.updateBool("HasTown", false);
                t.updateBool("HasCastle", false);
            }

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
    }

    public bool CheckForEmpty(List<Tile> tileList)
    {
        foreach (Tile t in tileList)
        {
            if (t.IsEmpty)
                return true;
        }

        return false;
    }

    public void GenerateVillage(List<Tile> tileList)
    {
        if (CheckSize(tileList))
        {
            TurnNeutral(tileList);
            return;
        }

        if (!CheckForEmpty(tileList))
        {
            Debug.LogError("There are no empty tiles, so there is no place to generate an empty village.");
			TurnNeutral(tileList);
            return;
        }

        int r = UnityEngine.Random.Range(0, tileList.Count);

        if (tileList[r].IsEmpty)
        {
            tileList[r].updateBool("HasVillage", true);
            tileList[r].updateBool("HasHovel", true);
        }
        else
            GenerateVillage(tileList);
    }
}