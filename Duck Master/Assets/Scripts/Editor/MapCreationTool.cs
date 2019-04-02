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
	List<string[]> listGridSelStrings;

	string[] blockTypes = { "Ground", "Water", "Damp", "None" };
	int blockSelection;

	bool toggleSelector = false;

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
				List<string[]> oldList = listGridSelStrings;
				listGridSelStrings = new List<string[]>();
				for (int i = 0; i < verticalLevels; ++i)
				{
					if(oldList != null && i < oldList.Count)
					{
						listGridSelStrings.Add(oldList[i]);
					}
					else
					{
						listGridSelStrings.Add(new string[0]);
					}
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

			int height = 0, width = 0;

			if (currentLevelHeights != null && currentLevelWidths != null)
			{
				if (levelSelection >= 0 && levelSelection < currentLevelWidths.Length && levelSelection < currentLevelHeights.Length)
				{
					currentLevelWidths[levelSelection] = EditorGUILayout.IntField("Level Width", currentLevelWidths[levelSelection]);
					currentLevelHeights[levelSelection] = EditorGUILayout.IntField("Level Height", currentLevelHeights[levelSelection]);
				}

				height = currentLevelHeights[levelSelection];
				width = currentLevelWidths[levelSelection];

				if (lastLevelHeights[levelSelection] != height || lastLevelWidths[levelSelection] != width || lastLevelSelection != levelSelection)
				{
					Array.Copy(currentLevelWidths, lastLevelWidths, lastLevelWidths.Length);
					Array.Copy(currentLevelHeights, lastLevelHeights, lastLevelHeights.Length);
					string[] temp = listGridSelStrings[levelSelection];
					gridSelStrings = new string[height * width];
					Array.Copy(temp, gridSelStrings, (temp.Length > gridSelStrings.Length) ? gridSelStrings.Length : temp.Length);
					listGridSelStrings[levelSelection] = gridSelStrings;
				}

				if(height > 0 && width > 0)
				{
					gridSel = GUILayout.SelectionGrid(gridSel, gridSelStrings, width);
				}
				

				if (toggleSelector)
				{
					gridSelStrings[gridSel] = blockTypes[blockSelection];
				}
			}

			if (GUILayout.Button("Create Level"))
			{
				//List<List<DuckTile.TileType>> typeGrid, List< List<bool> > baitableGrid, List<List<bool>> heightChangeGrid, int height
				List<List<DuckTile.TileType>> typeGrid = new List<List<DuckTile.TileType>>();
				List<DuckTile.TileType> typeList;
				List<List<bool>> baitableGrid = new List<List<bool>>();
				List<bool> baitableList;
				List<List<bool>> heightChangeGrid = new List<List<bool>>();
				List<bool> heightChangeList;
				List<DuckTileGrid> tileGrids = new List<DuckTileGrid>();

				for(int i = 0; i < verticalLevels; ++i)
				{
					// current height is i
					for(int j = 0; j < height; ++j)
					{
						typeList = new List<DuckTile.TileType>();
						baitableList = new List<bool>();
						heightChangeList = new List<bool>();
						for(int k = 0; k < width; ++k)
						{
							int index = height * j + k;
							if(index < height * width)
							{
								string currentBlock = listGridSelStrings[i][index];
								//if(currentBlock == null)
								//{
								//	Debug.Log("Null");
								//}
								// string[] blockTypes = { "Ground", "Water", "Damp", "None" };
								if (currentBlock == blockTypes[0] || currentBlock == blockTypes[2])
								{
									// passable both
									typeList.Add(DuckTile.TileType.PassableBoth);
								}
								else if(currentBlock == blockTypes[1])
								{
									// unpassable master
									typeList.Add(DuckTile.TileType.UnpassableMaster);
								}
								else
								{
									// it's none/null aka null
									typeList.Add(DuckTile.TileType.INVALID_TYPE);
								}
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
				MapGenerator.GenerateMap(tileGrids);
			}
		}
		// TO DO: Add the option to grab a ton of tiles with right click
		// OR do it with numbers idk
		//Debug.Log(Event.current.button);
		lastVerticalLevels = verticalLevels;
		lastLevelSelection = levelSelection;
	}
}
