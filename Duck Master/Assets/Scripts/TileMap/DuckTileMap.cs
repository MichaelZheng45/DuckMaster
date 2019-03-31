using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckTileGrid
{
    List<List<DuckTile>> mGrid;

    public DuckTileGrid()
    {
        mGrid = new List<List<DuckTile>>();
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

    public DuckTile GetTile(int x, int y)
    {
        return mGrid[y][x];
    }
}

public class DuckTileMap
{
    List<DuckTileGrid> mGridMap;

    public DuckTileMap()
    {
        mGridMap = new List<DuckTileGrid>();
    }

    public DuckTileMap(int x, int y, int height)
    {
        mGridMap = new List<DuckTileGrid>();
        for(int i = 0; i < height; ++i)
        {
            mGridMap.Add(new DuckTileGrid(x, y));
        }
    }

    public DuckTileMap(List<DuckTileGrid> gridMap)
    {
        mGridMap = gridMap;
    }

    public DuckTile GetTile(int x, int y, int height)
    {
        return mGridMap[height].GetTile(x, y);
    }

    void CreateConnections()
    {
        List<List<int>> heightMap = new List<List<int>>();
        for(int i = mGridMap.Count - 1; i >= 0; i--)
        {

        }
    }
}
