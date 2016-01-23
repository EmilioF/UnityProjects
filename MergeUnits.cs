using UnityEngine;
using System.Collections;

public class MergeUnits : MonoBehaviour
{
	private Village home;
	
	private Tile currentTile;
	private Tile targetTile;
	
	private Unit currentUnit;
	private Unit targetUnit;
	
	public void CheckForMerge (Unit unit1, Unit unit2, Village v)
	{
		home = v;
		
		currentUnit = unit1;
		currentTile = currentUnit.GetComponentInParent<Tile>();
		
		targetUnit = unit2;
		targetTile = targetUnit.GetComponentInParent<Tile>();
		
		Debug.Log ("Current unit is: " + currentUnit + " and it's on tile " + currentTile);
		Debug.Log ("Target unit is: " + targetUnit + " and it's on tile " + targetTile);
		
		int result = (int)currentUnit.rank + (int)targetUnit.rank;
		
		if (result <= (int)home.ThisRank + 1)
			Merge (result);
		else
			Debug.Log ("Cannot merge because this village does not have the required rank.");
	}
	
	public void Merge (int resultingUnit)
	{
		switch (resultingUnit)
		{
		case 1:
			Debug.Log("ERROR: attempting to merge into a peasant.");
			break;
			
		case 2:
			MergeHelper ();
			targetTile.updateBool("HasInfantry", true);
			break;
			
		case 3:
			MergeHelper ();
			targetTile.updateBool("HasSoldier", true);
			break;
			
		case 4:
			MergeHelper ();
			targetTile.updateBool("HasKnight", true);
			break;
			
		default:
			Debug.Log ("Default case.");
			break;
		}
	}
	
	public void MergeHelper ()
	{
		DestroyUnit(currentTile);
		DestroyUnit(targetTile);
		targetTile.updateBool("HasUnit", true);
	}
	
	public void DestroyUnit (Tile t)
	{
		t.updateBool("HasUnit", false);
		t.updateBool("HasPeasant", false);
		t.updateBool("HasSoldier", false);
		t.updateBool("HasInfantry", false);
		t.updateBool("HasKnight", false);
	}
}