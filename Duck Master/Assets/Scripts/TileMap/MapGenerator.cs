using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[SerializeField]
	GameObject groundObject;
	[SerializeField]
	GameObject waterObject;
	[SerializeField]
	GameObject dampObject;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void GenerateMap(int verticalLevels, List<string[]> listGridSelStrings, string[] blockTypes, int[] levelHeights, int[] levelWidths)
	{
		GameObject levelFold = GameObject.Find("LevelFolder");
		if(levelFold == null)
		{
			levelFold = new GameObject("LevelFolder");
		}

		foreach (Transform child in levelFold.transform)
		{
			GameObject.DestroyImmediate(child.gameObject);
		}

		List<List<DuckTile.TileType>> typeGrid = new List<List<DuckTile.TileType>>();
		List<DuckTile.TileType> typeList;
		List<List<bool>> baitableGrid = new List<List<bool>>();
		List<bool> baitableList;
		List<List<bool>> heightChangeGrid = new List<List<bool>>();
		List<bool> heightChangeList;
		List<DuckTileGrid> tileGrids = new List<DuckTileGrid>();
		GameObject tileObj = null;

		for (int i = 0; i < verticalLevels; ++i)
		{
			// current height is i
			int height = levelHeights[i];
			int width = levelWidths[i];
			for (int j = 0; j < height; ++j)
			{
				typeList = new List<DuckTile.TileType>();
				baitableList = new List<bool>();
				heightChangeList = new List<bool>();
				for (int k = 0; k < width; ++k)
				{
					int index = height * j + k;
					if (index < height * width)
					{
						// TO DO: center positions, fix instantiation ways
						string currentBlock = listGridSelStrings[i][index];
						Vector3 pos = new Vector3(j, i, k);

						// string[] blockTypes = { "Ground", "Water", "Damp", "None" };
						if (currentBlock == blockTypes[0] || currentBlock == blockTypes[2])
						{
							// passable both
							typeList.Add(DuckTile.TileType.PassableBoth);

							// TO DO CHANGE ME PLEASE?
							if(currentBlock == blockTypes[0])
							{
								tileObj = Instantiate(groundObject, pos, Quaternion.identity);
							}
							else
							{
								tileObj = Instantiate(dampObject, pos, Quaternion.identity);
							}
						}
						else if (currentBlock == blockTypes[1])
						{
							// unpassable master
							typeList.Add(DuckTile.TileType.UnpassableMaster);
							tileObj = Instantiate(waterObject, pos, Quaternion.identity);
						}
						else
						{
							// it's none/null aka null
							typeList.Add(DuckTile.TileType.INVALID_TYPE);
						}
						tileObj.transform.parent = levelFold.transform;
						baitableList.Add(false);
						heightChangeList.Add(false);
					}
				}
				typeGrid.Add(typeList);
				baitableGrid.Add(baitableList);
				heightChangeGrid.Add(heightChangeList);
			}
			tileGrids.Add(new DuckTileGrid(typeGrid, baitableGrid, heightChangeGrid, i));
		}

		TileMapScriptableObject temp = Resources.Load("scriptableObjects/TileMapHolder") as TileMapScriptableObject;
		temp.tileMap = new DuckTileMap(tileGrids);
	}
}
