using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Tile Map/Holder", order = 1)]
public class TileMapScriptableObject : ScriptableObject
{
	//public DuckTileMap tileMap;
	public int verticalLevels;
	public string[] listGridSelStrings;
	public string[] blockTypes;
	public int[] levelHeights;
	public int[] levelWidths;
}