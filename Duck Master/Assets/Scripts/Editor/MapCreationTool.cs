using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public class MapCreationTool : EditorWindow
{
	TileMapScriptableObject loadingScriptableObject;

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
	//string[] listGridSelStrings;
	List<List<List<string>>> listGridSelStrings;

	string[] blockTypes = { "Ground", "Water", "None" };
	int defaultBlockIndex = 2;
	int blockSelectionIndex;

	bool toggleSelector = false;

	string scriptableObjectName = "";

	[MenuItem("Window/Level Generator")]
	static void Init()
	{
		MapCreationTool window = (MapCreationTool)EditorWindow.GetWindow(typeof(MapCreationTool));
		window.Show();
	}

    /* TO DO: Fix copying issues
                1: Doesn't scale down in size
                2: Overwrites data
     */
	void OnGUI()
	{
		GUILayout.Label("Load An Existing Tile Map");
		loadingScriptableObject = EditorGUILayout.ObjectField("Object to Load", loadingScriptableObject, typeof(UnityEngine.Object), false) as TileMapScriptableObject;

		if(GUILayout.Button("Load Scriptable Object"))
		{
			if(loadingScriptableObject != null)
			{
				LoadScriptableObject();
			}
		}

		GUILayout.Label("Layer Change / Selection");
		verticalLevels = EditorGUILayout.IntField("Max Number of Layers", verticalLevels);
		// make sure we have layers
		if (verticalLevels != 0)
		{
			// setup new layers on number change
			if (lastVerticalLevels != verticalLevels)
			{
				toggleSelector = false;
				levelStringNums = new string[verticalLevels];

				for (int i = 0; i < verticalLevels; i++)
				{
					levelStringNums[i] = "Level " + (i + 1);
				}

				// copy list of grids
				List<List<List<string>>> oldList = listGridSelStrings;
				listGridSelStrings = new List<List<List<string>>>();
				for(int i = 0; i < verticalLevels; ++i)
				{
					if(oldList != null && oldList.Count > i)
					{
						listGridSelStrings.Add(oldList[i]);
					}
					else
					{
						listGridSelStrings.Add(new List<List<string>>());
					}
				}
			}

			// showing the level selection dropdown
			if (levelStringNums != null)
			{
				levelSelection = EditorGUILayout.Popup("Layer Selection", levelSelection, levelStringNums);
			}

			// if the widths are null create them, or if they're changed copy them
			if (currentLevelWidths == null || verticalLevels != currentLevelWidths.Length)
			{
				currentLevelWidths = new int[verticalLevels];
				if(lastLevelWidths != null)
				{
					Array.Copy(lastLevelWidths, currentLevelWidths, (lastLevelWidths.Length > currentLevelWidths.Length) ? currentLevelWidths.Length : lastLevelWidths.Length);
				}
				lastLevelWidths = new int[verticalLevels];
			}
			// if the height are null create them, or if they're changed copy them
			if (currentLevelHeights == null || verticalLevels != currentLevelHeights.Length)
			{
				currentLevelHeights = new int[verticalLevels];
				if(lastLevelHeights != null)
				{
					Array.Copy(lastLevelHeights, currentLevelHeights, (lastLevelHeights.Length > currentLevelHeights.Length) ? currentLevelHeights.Length : lastLevelHeights.Length);
				}
				lastLevelHeights = new int[verticalLevels];
			}

			GUILayout.Label("Block Selectors");

			// select the initial block generated
			defaultBlockIndex = EditorGUILayout.Popup("Initial Block Selection", defaultBlockIndex, blockTypes);

			// selection for which block you're using
			blockSelectionIndex = EditorGUILayout.Popup("Block Selection", blockSelectionIndex, blockTypes);

			EditorGUILayout.PrefixLabel("Grid Changing");

			// whether you want to change the grid or not
			toggleSelector = EditorGUILayout.Toggle("Enable Grid Changing", toggleSelector);

			if (GUILayout.Button("Change All") && listGridSelStrings.Count > 0)
			{
				ChangeAll(blockSelectionIndex);
			}

			GUILayout.Label("Grid Dimensions");

			int height = 0, width = 0, lastHeight = 0, lastWidth = 0;

			// make sure they're not null
			if (currentLevelHeights != null && currentLevelWidths != null)
			{
				// make sure the level selection is valid, and that it's less than the number of grid dimensions
				if (levelSelection >= 0 && levelSelection < currentLevelWidths.Length && levelSelection < currentLevelHeights.Length)
				{
					currentLevelWidths[levelSelection] = EditorGUILayout.IntField("Level Width", currentLevelWidths[levelSelection]);
					currentLevelHeights[levelSelection] = EditorGUILayout.IntField("Level Height", currentLevelHeights[levelSelection]);
				}

                height = currentLevelHeights[levelSelection];
				width = currentLevelWidths[levelSelection];
				lastHeight = lastLevelHeights[levelSelection];
				lastWidth = lastLevelWidths[levelSelection];

				// if the size has changed copy all the data and display the new grid
                if (lastHeight != height || lastWidth != width || lastLevelSelection != levelSelection)
                {
					if (lastHeight < height)
					{
						// need to add rows
						for (int i = lastHeight; i < height; ++i)
						{
							listGridSelStrings[levelSelection].Add(new List<string>());
						}
					}
					else if (lastHeight > height)
					{
						// need to remove rows
						listGridSelStrings[levelSelection].RemoveRange(height, lastHeight - height);
					}

					for (int i = 0; i < listGridSelStrings[levelSelection].Count; ++i)
					{
						// need to add columns
						if (lastWidth < width || listGridSelStrings[levelSelection][i].Count < width)
						{
							int count = (width - lastWidth > width - listGridSelStrings[levelSelection][i].Count) ?
								width - lastWidth : width - listGridSelStrings[levelSelection][i].Count;
							for (int j = 0; j < count; ++j)
							{
								listGridSelStrings[levelSelection][i].Add(blockTypes[defaultBlockIndex]);
							}
						}
						// need to remove columns
						else if (lastWidth > width)
						{
							listGridSelStrings[levelSelection][i].RemoveRange(width, lastWidth - width);
						}

					}

					// copy the currents to the last frames
					Array.Copy(currentLevelWidths, lastLevelWidths, lastLevelWidths.Length);
                    Array.Copy(currentLevelHeights, lastLevelHeights, lastLevelHeights.Length);

					// create a new grid of strings for display
                    gridSelStrings = new string[height * width];
                }

				// copy the respective contents to the grid for display
				if(gridSelStrings != null)
				{
					for (int j = 0; j < height; ++j)
					{
						for (int k = 0; k < width; ++k)
						{
							gridSelStrings[width * j + k] = listGridSelStrings[levelSelection][j][k];
						}
					}
				}

				// if we have a grid display it and change the text to the current selection if need be
                if (height > 0 && width > 0)
				{
					gridSel = GUILayout.SelectionGrid(gridSel, gridSelStrings, width);

					if (toggleSelector && gridSelStrings.Length > 0)
					{
						gridSelStrings[gridSel] = blockTypes[blockSelectionIndex];
					}
				}
				
				// copy the changes back over
				if (gridSelStrings != null && listGridSelStrings != null && listGridSelStrings.Count > 0)
				{
					for(int j = 0; j < height; ++j)
					{
						for(int k = 0; k < width; ++k)
						{
							listGridSelStrings[levelSelection][j][k] = gridSelStrings[width * j + k];
						}
					}
				}
			}

			GUILayout.Label("Level Name");
			scriptableObjectName = GUILayout.TextField(scriptableObjectName);

			if (GUILayout.Button("Create Level"))
			{
				// need to copy listrgridselstrings into an array
				int totalSize = 0;
				for(int i = 0; i < verticalLevels; ++i)
				{
					totalSize += currentLevelHeights[i] * currentLevelWidths[i];
				}

				string[] tempString = new string[totalSize];
				int index = 0;
				for (int i = 0; i < listGridSelStrings.Count; ++i)
				{
					for(int j = 0; j < listGridSelStrings[i].Count; ++j)
					{
						for(int k = 0; k < listGridSelStrings[i][j].Count; ++k)
						{
							tempString[index] = listGridSelStrings[i][j][k];
							index++;
						}
					}
				}
				GenerateMap(verticalLevels, tempString, blockTypes, currentLevelHeights, currentLevelWidths, scriptableObjectName);
			}
		}

		if(Event.current.keyCode == KeyCode.Z && Event.current.type == EventType.KeyUp)
		{
			toggleSelector = !toggleSelector;
			EditorWindow.focusedWindow.Repaint();
		}
		else if(Event.current.keyCode == KeyCode.X && Event.current.type == EventType.KeyUp)
		{
			ChangeAll(blockSelectionIndex);
			EditorWindow.focusedWindow.Repaint();
		}

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

		for (int i = levelFold.transform.childCount; i > 0; --i)
		{
			DestroyImmediate(levelFold.transform.GetChild(0).gameObject);
		}

		GameObject tileObj = null;
		int index = 0;

		for (int i = 0; i < verticalLevels; ++i)
		{
			// current height is i
			int height = levelHeights[i];
			int width = levelWidths[i];

			for (int j = 0; j < height; ++j)
			{
				for (int k = 0; k < width; ++k)
				{
					string currentBlock = listGridSelStrings[index];
					Vector3 pos = new Vector3(j, i, k);

					// string[] blockTypes = { "Ground", "Water", "None" };
					if (currentBlock == blockTypes[0])
					{
						// passable both
						tileObj = Instantiate(groundObject, pos, Quaternion.identity);
						tileObj.transform.parent = levelFold.transform;
					}
					else if (currentBlock == blockTypes[1])
					{
						// unpassable master
						tileObj = Instantiate(waterObject, pos, Quaternion.identity);
						tileObj.transform.parent = levelFold.transform;
					}
					index++;
				}
			}
		}

		TileMapScriptableObject scriptableObject = ScriptableObject.CreateInstance<TileMapScriptableObject>();
		string assetString = "Assets/Resources/scriptableObjects/Level_Data/" + scriptableObjectName + ".asset";
		AssetDatabase.CreateAsset(scriptableObject, assetString);
		scriptableObject.verticalLevels = verticalLevels;
		scriptableObject.listGridSelStrings = listGridSelStrings;
		scriptableObject.blockTypes = blockTypes;
		scriptableObject.levelHeights = levelHeights;
		scriptableObject.levelWidths = levelWidths;
		EditorUtility.SetDirty(scriptableObject);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	void ChangeAll(int selectionIndex)
	{
		for (int j = 0; j < listGridSelStrings[levelSelection].Count; ++j)
		{
			for (int k = 0; k < listGridSelStrings[levelSelection][j].Count; ++k)
			{
				listGridSelStrings[levelSelection][j][k] = blockTypes[selectionIndex];
			}
		}
	}

	void LoadScriptableObject()
	{
		verticalLevels = loadingScriptableObject.verticalLevels;
		blockTypes = new List<string>(loadingScriptableObject.blockTypes).ToArray();

		currentLevelHeights = new List<int>(loadingScriptableObject.levelHeights).ToArray();
		currentLevelWidths = new List<int>(loadingScriptableObject.levelWidths).ToArray();
		lastLevelHeights = new List<int>(loadingScriptableObject.levelHeights).ToArray();
		lastLevelWidths = new List<int>(loadingScriptableObject.levelWidths).ToArray();

		listGridSelStrings = new List<List<List<string>>>();

		gridSelStrings = new string[currentLevelHeights[0] * currentLevelWidths[0]];

		int index = 0;
		for(int i = 0; i < verticalLevels; ++i)
		{
			listGridSelStrings.Add(new List<List<string>>());
			for(int j = 0; j < currentLevelHeights[i]; ++j)
			{
				listGridSelStrings[i].Add(new List<string>());
				for(int k = 0; k < currentLevelWidths[i]; ++k)
				{
					listGridSelStrings[i][j].Add(loadingScriptableObject.listGridSelStrings[index]);
					index++;
				}
			}
		}

		// TO DO: change so this doesn't change the scriptable object
		GenerateMap(verticalLevels, loadingScriptableObject.listGridSelStrings, blockTypes, currentLevelHeights, currentLevelWidths, loadingScriptableObject.name);
	}
}
