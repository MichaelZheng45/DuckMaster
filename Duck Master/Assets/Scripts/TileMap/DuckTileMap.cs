using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckTileGrid
{
    List<List<DuckTile>> mGrid;

    public DuckTileGrid()
    {
        mGrid = null;
    }

    public DuckTileGrid(int x, int y)
    {
        mGrid = new List<List<DuckTile>>();
        List<DuckTile> tempList;
        for (int j = 0; j < x; ++j)
        {
            tempList = new List<DuckTile>();
            for (int k = 0; k < x; ++k)
            {
                tempList.Add(new DuckTile());
                Debug.Log("X: " + k + " Y: " + j);
            }
            mGrid.Add(tempList);
        }
    }
    // Height change is for the fact that a tile might be walkable above but not walkable from below
    public DuckTileGrid(List<List<DuckTile.TileType>> typeGrid, List<List<bool>> baitableGrid, List<List<bool>> heightChangeGrid)
    {
        mGrid = new List<List<DuckTile>>();
        List<DuckTile> tempList;
        for(int j = 0; j < typeGrid.Count; ++j)
        {
            tempList = new List<DuckTile>();
            for (int k = 0; k < typeGrid[j].Count; ++k)
            {
                // j is y, k is x
                tempList.Add(new DuckTile(typeGrid[j][k], baitableGrid[j][k], heightChangeGrid[j][k]));
                Debug.Log("X: " + k + " Y: " + j + "\nType: " + typeGrid[j][k] + "\nBaitable: " + baitableGrid[j][k] + "\nHeight Change: " + heightChangeGrid[j][k]);
            }
            mGrid.Add(tempList);
        }
    }
}

public class DuckTileMap
{
    List<DuckTileGrid> mGridMap;
}
