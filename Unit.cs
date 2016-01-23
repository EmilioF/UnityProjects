using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public abstract class Unit : MonoBehaviour
{
	public enum UnitRank { Peasant = 1, Infantry = 2, Soldier = 3, Knight = 4 };
	
	public UnitRank rank;
}