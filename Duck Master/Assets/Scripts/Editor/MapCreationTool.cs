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
					int totalSize = 0;
					if (currentLevelWidths != null && currentLevelHeights != null)
					{
						for (int i = 0; i < verticalLevels; ++i)
						{
							totalSize += currentLevelWidths[i] * currentLevelHeights[i];
						}
					}

					string[] oldList = listGridSelStrings;
					listGridSelStrings = new string[totalSize];
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

        string assetString = "scriptableObjects/Level_Data/" + scriptableObjectName;// + ".asset";
        TileMapScriptableObject scriptableObject = Resources.Load(assetString) as TileMapScriptableObject;
        if(scriptableObject == null)
        {
            scriptableObject = ScriptableObject.CreateInstance<TileMapScriptableObject>();
            AssetDatabase.CreateAsset(scriptableObject, "Assets/Resources/" + assetString + ".asset");
        }
        
		scriptableObject.verticalLevels = verticalLevels;
		scriptableObject.listGridSelStrings = listGridSelStrings;
		scriptableObject.blockTypes = blockTypes;
		scriptableObject.levelHeights = levelHeights;
		scriptableObject.levelWidths = levelWidths;
		EditorUtility.SetDirty(scriptableObject);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}
