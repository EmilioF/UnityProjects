using UnityEngine;
using System.Collections;

public class UpgradeUnit : MonoBehaviour
{
	private Village home;
	private Unit currentUnit;
	private Tile currentTile;
	
	public void CheckForUpgrade (Unit unit, Village v)
	{
		home = v;
		currentUnit = unit;
		currentTile = currentUnit.GetComponentInParent<Tile>();
		
		int target = (int)currentUnit.rank;
		
		if (home.Gold >= 10 && currentUnit.rank != Unit.UnitRank.Knight)
			Upgrade(target);
		else
			Debug.Log ("Cannot upgrade: not enough gold or already at max rank (knight).");
	}

	public void Upgrade (int target)
	{
		switch (target)
		{
		case 1:
			currentTile.updateBool("HasPeasant", false);
			currentTile.updateBool("HasInfantry", true);
			home.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -10);
			break;
			
		case 2:
			currentTile.updateBool("HasInfantry", false);
			currentTile.updateBool("HasSoldier", true);
			home.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -10);
			break;
			
		case 3:
			currentTile.updateBool("HasSoldier", false);
			currentTile.updateBool("HasKnight", true);
			home.GetComponent<NetworkView>().RPC("updateGold", RPCMode.AllBuffered, -10);
			break;
			
		case 4:
			Debug.Log("ERROR: attempting to upgrade a knight.");
			break;
			
		default:
			Debug.Log ("Default case.");
			break;
		}
	}
}