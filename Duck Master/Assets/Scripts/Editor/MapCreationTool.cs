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
	List<List<List<string>>> listGridSelStrings;

	Dictionary<string, GameObject> blockNameObjectPairs;
	// First is what you see in editor
	// Second is the location from 'Prefab' that it loads from
	Dictionary<string, string> prefabLocationsTypesPairs = new Dictionary<string, string>()
	{
		{ "Ground",			"TilePack1/ground" },
		{ "Water",			"TilePack1/water" },
		{ "Big Tree",		"TilePack1/NaturalWallType" },
		{ "Rock",			"TilePack1/rock1" },
		{ "Short Wall",		"TilePack1/shortwall" },
		{ "None",			"" },
	};

	// These are the names of the buttons, this list should be the same order as prefabNames
	// "None" needs to always be last
	string[] blockTypes;
	int defaultBlockIndex;
	int blockSelectionIndex;

	bool toggleSelector = false;

	string scriptableObjectName = "";

    bool baitFoldout = false;

    int attractQuantity = 0, repelQuantity = 0, pepperQuantity = 0;

	[MenuItem("Window/Level Generator")]
	static void Init()
	{
		MapCreationTool window = (MapCreationTool)EditorWindow.GetWindow(typeof(MapCreationTool));
		window.Show();
	}

	void OnGUI()
	{
		// initialize what I'm going to show in editor
		if(blockTypes == null)
		{
			blockTypes = new List<string>(prefabLocationsTypesPairs.Keys).ToArray();
			defaultBlockIndex = blockTypes.Length - 1;
		}

		// create the dictionary for easy use later
		if(blockNameObjectPairs == null)
		{
			blockNameObjectPairs = new Dictionary<string, GameObject>();
			for (int i = 0; i < blockTypes.Length - 1; ++i)
			{
				blockNameObjectPairs[blockTypes[i]] = Resources.Load("Prefab/" + prefabLocationsTypesPairs[blockTypes[i]]) as GameObject;
			}
		}

        // Loading Stuff
		GUILayout.Label("Load An Existing Tile Map");
		loadingScriptableObject = EditorGUILayout.ObjectField("Object to Load", loadingScriptableObject, typeof(UnityEngine.Object), false) as TileMapScriptableObject;

		if(GUILayout.Button("Load Scriptable Object"))
		{
			if(loadingScriptableObject != null)
			{
				LoadScriptableObject();
			}
		}

        GUILayout.Label("Bait numbers for the current level");
        baitFoldout = EditorGUILayout.Foldout(baitFoldout, "Bait numbers for the current level");
        if(baitFoldout)
        {
            attractQuantity = EditorGUILayout.IntField("Attract Quantity", attractQuantity);
            repelQuantity = EditorGUILayout.IntField("Repel Quantity", repelQuantity);
            pepperQuantity = EditorGUILayout.IntField("Pepper Quantity", pepperQuantity);
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
				if (levelSelection == verticalLevels)
					levelSelection--;
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
				Array.Copy(currentLevelWidths, lastLevelWidths, lastLevelWidths.Length);
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
				Array.Copy(currentLevelHeights, lastLevelHeights, lastLevelHeights.Length);
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
				GenerateMap(verticalLevels, tempString, blockTypes, currentLevelHeights, currentLevelWidths, scriptableObjectName, true);
			}
		}

		if(Event.current.keyCode == KeyCode.F1 && Event.current.type == EventType.KeyUp)
		{
			toggleSelector = !toggleSelector;
			EditorWindow.focusedWindow.Repaint();
		}
		else if(Event.current.keyCode == KeyCode.F2 && Event.current.type == EventType.KeyUp)
		{
			ChangeAll(blockSelectionIndex);
			EditorWindow.focusedWindow.Repaint();
		}

		lastVerticalLevels = verticalLevels;
		lastLevelSelection = levelSelection;
	}

	void GenerateMap(int verticalLevels, string[] listGridSelStrings, string[] blockTypes, int[] levelHeights, int[] levelWidths, string scriptableObjectName = "", bool save = false)
	{
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

					// if it's not a none tile
					if (currentBlock != blockTypes[blockTypes.Length - 1])
					{
                        Debug.Log(currentBlock);
						tileObj = Instantiate(blockNameObjectPairs[currentBlock], pos, Quaternion.identity);
						tileObj.transform.parent = levelFold.transform;
					}

					index++;
				}
			}
		}

		if (save && scriptableObjectName != "")
		{
			string assetString = "scriptableObjects/Level_Data/" + scriptableObjectName;
			TileMapScriptableObject scriptableObject = Resources.Load(assetString) as TileMapScriptableObject;
			if (scriptableObject == null)
			{
				scriptableObject = ScriptableObject.CreateInstance<TileMapScriptableObject>();
				AssetDatabase.CreateAsset(scriptableObject, "Assets/Resources/" + assetString + ".asset");
			}

			scriptableObject.verticalLevels = verticalLevels;
			scriptableObject.listGridSelStrings = listGridSelStrings;
			scriptableObject.blockTypes = blockTypes;
			scriptableObject.levelHeights = levelHeights;
			scriptableObject.levelWidths = levelWidths;
            scriptableObject.attractQuantity = attractQuantity;
            scriptableObject.repelQuantity = repelQuantity;
            scriptableObject.pepperQuantity = pepperQuantity;
			EditorUtility.SetDirty(scriptableObject);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
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

		currentLevelHeights = new List<int>(loadingScriptableObject.levelHeights).ToArray();
		currentLevelWidths = new List<int>(loadingScriptableObject.levelWidths).ToArray();
		lastLevelHeights = new List<int>(loadingScriptableObject.levelHeights).ToArray();
		lastLevelWidths = new List<int>(loadingScriptableObject.levelWidths).ToArray();
		scriptableObjectName = loadingScriptableObject.name;

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

        attractQuantity = loadingScriptableObject.attractQuantity;
        repelQuantity = loadingScriptableObject.repelQuantity;
        pepperQuantity = loadingScriptableObject.pepperQuantity;

		GenerateMap(verticalLevels, loadingScriptableObject.listGridSelStrings, blockTypes, currentLevelHeights, currentLevelWidths);
	}
}
