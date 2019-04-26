using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DuckList
{
	public List<DuckTile> mList { get; set; }
	public DuckList(List<DuckTile> list)
	{
		mList = list;
	}
}

[System.Serializable]
public class DuckTileGrid
{
	[SerializeField]
    List<DuckList> mGrid;

    public DuckTileGrid()
    {
        mGrid = new List<DuckList>();
    }

	// Don't Use Please, Won't work
    public DuckTileGrid(int x, int y)
    {
        mGrid = new List<DuckList>();
        List<DuckTile> tempList;
        for (int j = 0; j < x; ++j)
        {
            tempList = new List<DuckTile>();
            for (int k = 0; k < x; ++k)
            {
                tempList.Add(new DuckTile());
            }
            mGrid.Add(new DuckList(tempList));
        }
    }

    // Height change is for the fact that a tile might be walkable above but not walkable from below
    public DuckTileGrid(List<List<DuckTile.TileType>> typeGrid, List<List<bool>> baitableGrid, List<List<bool>> heightChangeGrid, List<List<Vector3>> positionGrid, int height)
    {
        mGrid = new List<DuckList>();
        List<DuckTile> tempList;
        for(int j = 0; j < typeGrid.Count; ++j)
        {
            tempList = new List<DuckTile>();
            for (int k = 0; k < typeGrid[j].Count; ++k)
            {
                // j is y, k is x
                tempList.Add(new DuckTile(typeGrid[j][k], baitableGrid[j][k], heightChangeGrid[j][k], positionGrid[j][k], height));
            }
            mGrid.Add(new DuckList(tempList));
        }
    }

    public DuckTile GetTile(int x, int y)
    {
        if ((x > -1 && y > -1 && mGrid.Count > y && mGrid[y].mList.Count > x))
        {
            return mGrid[y].mList[x];
        }
        return null;
    }

	public List<DuckList> GetGrid()
	{
		return mGrid;
	}

	public int GetLength()
	{
		return mGrid.Count;
	}

	public int GetRowLength(int y)
	{
		if(y > -1 && y < mGrid.Count)
			return mGrid[y].mList.Count;
		return -1;
	}

	public void AddTile(int y, DuckTile tile)
	{
		if(y > -1 && y < mGrid.Count)
		{
			mGrid[y].mList.Add(tile);
		}
	}

	public void AddRow(List<DuckTile> duckTiles)
	{
		mGrid.Add(new DuckList(duckTiles));
	}
}

[System.Serializable]
public class DuckTileMap
{
    public List<DuckTileGrid> mGridMap { get; set; }
	public DuckTileGrid mHeightMap { get; set; }

    public DuckTileMap()
    {
        mGridMap = new List<DuckTileGrid>();
		mHeightMap = new DuckTileGrid();
    }

	// Don't Use Please, won't work
    public DuckTileMap(int x, int y, int height)
    {
        mGridMap = new List<DuckTileGrid>();
        for(int i = 0; i < height; ++i)
        {
            mGridMap.Add(new DuckTileGrid(x, y));
        }
		mHeightMap = new DuckTileGrid();
		CreateHeightMap();
		CreateConnections();
    }

    public DuckTileMap(List<DuckTileGrid> gridMap)
    {
        mGridMap = gridMap;
		mHeightMap = new DuckTileGrid();
		CreateHeightMap();
		CreateConnections();
    }

    public DuckTile GetTile(int x, int y, int height)
    {
        return mGridMap[height].GetTile(x, y);
    }

    public DuckTile getTileFromPosition(Vector3 position)
    {
        return mHeightMap.GetTile(Mathf.FloorToInt(position.z + .5f), Mathf.FloorToInt(position.x + .5f));
    }

    void CreateHeightMap()
    {
		DuckTile tempTile;
		for(int i = 0; i < mGridMap.Count; ++i)
		{
			DuckTileGrid grid = mGridMap[i];
			for (int j = 0; j < grid.GetLength(); ++j)
			{
				for(int k = 0; k < grid.GetRowLength(j); ++k)
				{
                    tempTile = grid.GetTile(k, j);
					if(j < mHeightMap.GetLength() && k < mHeightMap.GetRowLength(j) && tempTile != null && tempTile.mType != DuckTile.TileType.INVALID_TYPE)
					{
                        mHeightMap.GetGrid()[j].mList[k] = tempTile;
					}
					else
					{
						if(j >= mHeightMap.GetLength())
						{
							mHeightMap.AddRow(new List<DuckTile>());
						}
						if(k >= mHeightMap.GetRowLength(j) && tempTile != null)
						{
							mHeightMap.AddTile(j, tempTile);
						}
					}
				}
			}
		}
    }

	public void CreateConnections()
	{
		DuckTile currentTile, rightTile, bottomTile;
		currentTile = rightTile = bottomTile = null;
		Connection rightConnection, bottomConnection;
		rightConnection = bottomConnection = null;
		Vector3 rightTileIndex, bottomTileIndex;
		rightTileIndex = bottomTileIndex = Vector3.positiveInfinity;
		//bool rightHeightPassable = true, bottomHeightPassable;
		for (int j = 0; j < mHeightMap.GetLength(); ++j)
		{
			for (int k = 0; k < mHeightMap.GetRowLength(j); ++k)
			{
				currentTile = mHeightMap.GetTile(k, j);
				rightTile = mHeightMap.GetTile(k + 1, j);
				bottomTile = mHeightMap.GetTile(k, j + 1);
				Vector3 currentTileIndex = new Vector3(k, j, currentTile.mHeight);

				if(rightTile != null)
				{
					rightTileIndex = new Vector3(k + 1, j, rightTile.mHeight);
					rightConnection = new Connection(currentTileIndex, rightTileIndex, 255, 255);
				}
				else
				{
					rightTileIndex = Vector3.positiveInfinity;
					rightConnection = null;
				}
				
				if(bottomTile != null)
				{
					bottomTileIndex = new Vector3(k, j + 1, bottomTile.mHeight);
					bottomConnection = new Connection(currentTileIndex, bottomTileIndex, 255, 255);
				}
				else
				{
					bottomTileIndex = Vector3.positiveInfinity;
					bottomConnection = null;
				}
				
				if (rightConnection != null && (currentTile.mHeight == rightTile.mHeight || (currentTile.mHeight != rightTile.mHeight && currentTile.mHeightChange && rightTile.mHeightChange)))
				{
					if (currentTile.GetDuckPassable() && rightTile.GetDuckPassable())
					{
						// right connection is duck passable
						rightConnection.mDuckCost = 1;
					}
					if (currentTile.GetMasterPassable() && rightTile.GetMasterPassable())
					{
						// right connection is master passable
						rightConnection.mMasterCost = 1;
					}
				}
				if (bottomConnection != null && (currentTile.mHeight == bottomTile.mHeight || (currentTile.mHeight != bottomTile.mHeight && currentTile.mHeightChange && bottomTile.mHeightChange)))
				{
					if (currentTile.GetDuckPassable() && bottomTile.GetDuckPassable())
					{
						// bottom connection duck passable
						bottomConnection.mDuckCost = 1;
					}
					if (currentTile.GetMasterPassable() && bottomTile.GetMasterPassable())
					{
						// bottom connection master passable
						bottomConnection.mMasterCost = 1;
					}
				}

				currentTile.SetConnectionDirection(DuckTile.ConnectionDirection.RIGHT, rightConnection);
				currentTile.SetConnectionDirection(DuckTile.ConnectionDirection.DOWN, bottomConnection);
				if(rightConnection != null)
				{
					rightTile.SetConnectionDirection(DuckTile.ConnectionDirection.LEFT, new Connection(rightConnection.mToIndex, rightConnection.mFromIndex, rightConnection.mDuckCost, rightConnection.mMasterCost));
				}
				if(bottomConnection != null)
				{
					bottomTile.SetConnectionDirection(DuckTile.ConnectionDirection.UP, new Connection(bottomConnection.mToIndex, bottomConnection.mFromIndex, bottomConnection.mDuckCost, bottomConnection.mMasterCost));
				}
			}
		}
	}

    public Vector3 GetCenterPos()
    {
        return mHeightMap.GetTile(mHeightMap.GetLength() / 2, mHeightMap.GetRowLength(mHeightMap.GetLength() / 2) / 2).mPosition;
    }
}