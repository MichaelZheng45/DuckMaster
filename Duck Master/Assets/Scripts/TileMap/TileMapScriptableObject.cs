using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "Tile Map/Holder", order = 1)]
public class TileMapScriptableObject : ScriptableObject
{
	public DuckTileMap tileMap;
}