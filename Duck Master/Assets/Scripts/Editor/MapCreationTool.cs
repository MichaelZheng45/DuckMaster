using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MapCreationTool : EditorWindow
{
	int verticalLevels = 0, lastVerticalLevels;

	string[] levelStringNums;
	int levelSelection = 0;
	int lastLevelSelection = 0;

	int[] currentLevelWidths, currentLevelHeights;
	int[] lastLevelWidths, lastLevelHeights;

	string[] gridSelStrings;
	int gridSel;
	// 3d array to 1d array
	// current index is (verticalLevel * widthOfLevel * heightOfLevel + yIndex * heightOfLevel + xIndex)
	string[] listGridSelStrings;

	string[] blockTypes = { "Ground", "Water", "None" };
	int blockSelection;

	bool toggleSelector = false;

	string scriptableObjectName = "";

	[MenuItem("Window/Level Generator")]
	static void Init()
	{
		MapCreationTool window = (MapCreationTool)EditorWindow.GetWindow(typeof(MapCreationTool));
		window.Show();
	}

	void OnGUI()
	{
		verticalLevels = EditorGUILayout.IntField("Max Number of Levels", verticalLevels);
		if (verticalLevels != 0)
		{
			if (lastVerticalLevels != verticalLevels)
			{
				levelStringNums = new string[verticalLevels];

				for (int i = 0; i < verticalLevels; i++)
				{
					levelStringNums[i] = "Level " + (i + 1);
				}
			}

			if (levelStringNums != null)
			{
				levelSelection = EditorGUILayout.Popup("Level Selection", levelSelection, levelStringNums);
			}

			if (currentLevelWidths == null || verticalLevels != currentLevelWidths.Length)
			{
				currentLevelWidths = new int[verticalLevels];
				if(lastLevelWidths != null)
				{
					Array.Copy(lastLevelWidths, currentLevelWidths, (lastLevelWidths.Length > currentLevelWidths.Length) ? currentLevelWidths.Length : lastLevelWidths.Length);
				}
				lastLevelWidths = new int[verticalLevels];
			}
			if (currentLevelHeights == null || verticalLevels != currentLevelHeights.Length)
			{
				currentLevelHeights = new int[verticalLevels];
				if(lastLevelHeights != null)
				{
					Array.Copy(lastLevelHeights, currentLevelHeights, (lastLevelHeights.Length > currentLevelHeights.Length) ? currentLevelHeights.Length : lastLevelHeights.Length);
				}
				lastLevelHeights = new int[verticalLevels];
			}

			blockSelection = EditorGUILayout.Popup("Block Selection", blockSelection, blockTypes);

			toggleSelector = EditorGUILayout.Toggle("Enable Map Changing", toggleSelector);

			int height = 0, width = 0, lastHeight = 0, lastWidth = 0;

			if (currentLevelHeights != null && currentLevelWidths != null)
			{
				if (levelSelection >= 0 && levelSelection < currentLevelWidths.Length && levelSelection < currentLevelHeights.Length)
				{
					currentLevelWidths[levelSelection] = EditorGUILayout.IntField("Level Width", currentLevelWidths[levelSelection]);
					currentLevelHeights[levelSelection] = EditorGUILayout.IntField("Level Height", currentLevelHeights[levelSelection]);
				}

				if (lastVerticalLevels != verticalLevels || lastLevelHeights[levelSelection] != currentLevelHeights[levelSelection] || lastLevelWidths[levelSelection] != currentLevelWidths[levelSelection] || lastLevelSelection != levelSelection)
				{
					int totalWidth = 0, totalHeight = 0;
					if (currentLevelWidths != null && currentLevelHeights != null)
					{
						for (int i = 0; i < verticalLevels; ++i)
						{
							totalWidth += currentLevelWidths[i];
							totalHeight += currentLevelHeights[i];
						}
					}

					string[] oldList = listGridSelStrings;
					listGridSelStrings = new string[verticalLevels * totalWidth * totalHeight];
					if(oldList != null)
					{
						Array.Copy(oldList, listGridSelStrings, (oldList.Length > listGridSelStrings.Length) ? listGridSelStrings.Length : oldList.Length);
					}
				}

				height = currentLevelHeights[levelSelection];
				width = currentLevelWidths[levelSelection];
				lastHeight = lastLevelHeights[levelSelection];
				lastWidth = lastLevelWidths[levelSelection];
				int lastLength = lastHeight * lastWidth;
				int lastStart = levelSelection * lastLength;
				int currentStart = levelSelection * height * width;

				if (lastLevelHeights[levelSelection] != height || lastLevelWidths[levelSelection] != width || lastLevelSelection != levelSelection)
				{
					Array.Copy(currentLevelWidths, lastLevelWidths, lastLevelWidths.Length);
					Array.Copy(currentLevelHeights, lastLevelHeights, lastLevelHeights.Length);
					// We need to copy a section of a 3d array to an array
					string[] temp = new string[lastLength];
					
					// Copy from the start of the grid relative to the rest to the temp array
					Array.Copy(listGridSelStrings, lastStart, temp, 0, lastLength);
					gridSelStrings = new string[height * width];
					Array.Copy(temp, gridSelStrings, (temp.Length > gridSelStrings.Length) ? gridSelStrings.Length : temp.Length);
				}

				if(height > 0 && width > 0)
				{
					gridSel = GUILayout.SelectionGrid(gridSel, gridSelStrings, width);
				}
				

				if (toggleSelector)
				{
					gridSelStrings[gridSel] = blockTypes[blockSelection];
				}

				if (gridSelStrings != null && listGridSelStrings != null && listGridSelStrings.Length > 0)
				{
					Array.Copy(gridSelStrings, 0, listGridSelStrings, lastStart, gridSelStrings.Length);
				}
			}

			GUILayout.Label("Level Name");
			scriptableObjectName = GUILayout.TextField(scriptableObjectName);

			if (GUILayout.Button("Create Level"))
			{
				GenerateMap(verticalLevels, listGridSelStrings, blockTypes, currentLevelHeights, currentLevelWidths, scriptableObjectName);
			}
		}
		// TO DO: Add the option to grab a ton of tiles with right click
		// OR do it with numbers idk
		//Debug.Log(Event.current.button);
		lastVerticalLevels = verticalLevels;
		lastLevelSelection = levelSelection;
	}

	void GenerateMap(int verticalLevels, string[] listGridSelStrings, string[] blockTypes, int[] levelHeights, int[] levelWidths, string scriptableObjectName)
	{
		GameObject groundObject = Resources.Load("Prefab/TilePack1/ground") as GameObject;
		GameObject waterObject = Resources.Load("Prefab/TilePack1/water") as GameObject;
		GameObject levelFold = GameObject.Find("LevelFolder");
		if (levelFold == null)
		{
			levelFold = new GameObject("LevelFolder");
		}

		foreach (Transform child in levelFold.transform)
		{
			GameObject.DestroyImmediate(child.gameObject);
		}

		List<List<DuckTile.TileType>> typeGrid;
		List<DuckTile.TileType> typeList;
		List<List<bool>> baitableGrid;
		List<bool> baitableList;
		List<List<bool>> heightChangeGrid;
		List<bool> heightChangeList;
		List<Vector3> positionsList;
		List<List<Vector3>> positionGrid;

		List<DuckTileGrid> tileGrids = new List<DuckTileGrid>();

		GameObject tileObj = null;

		for (int i = 0; i < verticalLevels; ++i)
		{
			// current height is i
			int height = levelHeights[i];
			int width = levelWidths[i];
			typeGrid = new List<List<DuckTile.TileType>>();
			baitableGrid = new List<List<bool>>();
			heightChangeGrid = new List<List<bool>>();
			positionGrid = new List<List<Vector3>>();

			for (int j = 0; j < height; ++j)
			{
				typeList = new List<DuckTile.TileType>();
				baitableList = new List<bool>();
				heightChangeList = new List<bool>();
				positionsList = new List<Vector3>();
				for (int k = 0; k < width; ++k)
				{
					int index = i * height * width + height * j + k;
					if (index < height * width)
					{
						// TO DO: center positions, fix instantiation ways
						string currentBlock = listGridSelStrings[index];
						Vector3 pos = new Vector3(j, i, k);

						// string[] blockTypes = { "Ground", "Water", "None" };
						if (currentBlock == blockTypes[0])
						{
							// passable both
							//typeList.Add(DuckTile.TileType.PassableBoth);
							tileObj = Instantiate(groundObject, pos, Quaternion.identity);
						}
						else if (currentBlock == blockTypes[1])
						{
							// unpassable master
							//typeList.Add(DuckTile.TileType.UnpassableMaster);
							tileObj = Instantiate(waterObject, pos, Quaternion.identity);
						}
						else
						{
							// it's none/null aka null
							//typeList.Add(DuckTile.TileType.INVALID_TYPE);
						}
						tileObj.transform.parent = levelFold.transform;
						//baitableList.Add(false);
						//heightChangeList.Add(false);
						//positionsList.Add(pos);
					}
				}
				//typeGrid.Add(typeList);
				//baitableGrid.Add(baitableList);
				//heightChangeGrid.Add(heightChangeList);
				//positionGrid.Add(positionsList);
			}
			//tileGrids.Add(new DuckTileGrid(typeGrid, baitableGrid, heightChangeGrid, positionGrid, i));
		}

		TileMapScriptableObject scriptableObject = ScriptableObject.CreateInstance<TileMapScriptableObject>();
		string assetString = "Assets/Resources/scriptableObjects/" + scriptableObjectName + ".asset";
		AssetDatabase.CreateAsset(scriptableObject, assetString);
		//scriptableObject.tileMap = new DuckTileMap(tileGrids);
		//Debug.Log(scriptableObject.tileMap);
		scriptableObject.verticalLevels = verticalLevels;
		scriptableObject.listGridSelStrings = listGridSelStrings;
		scriptableObject.blockTypes = blockTypes;
		scriptableObject.levelHeights = levelHeights;
		scriptableObject.levelWidths = levelWidths;
		Debug.Log(verticalLevels);
		EditorUtility.SetDirty(scriptableObject);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}
