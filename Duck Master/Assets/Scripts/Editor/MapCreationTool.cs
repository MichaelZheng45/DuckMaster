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

	string[] blockTypes = { "Ground", "Water", "None" };
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
				GameObject.Find("GameManager").GetComponent<MapGenerator>().GenerateMap(verticalLevels, listGridSelStrings, blockTypes, currentLevelHeights, currentLevelWidths);
			}
		}
		// TO DO: Add the option to grab a ton of tiles with right click
		// OR do it with numbers idk
		//Debug.Log(Event.current.button);
		lastVerticalLevels = verticalLevels;
		lastLevelSelection = levelSelection;
	}
}
