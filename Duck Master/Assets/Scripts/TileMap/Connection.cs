using UnityEngine;

[System.Serializable]
public class Connection
{
	//public byte MAX_COST = 255;

	public Connection()
	{
		mFromIndex = new Vector3(-1, -1, -1);
		mToIndex = new Vector3(-1, -1, -1);
		mDuckCost = 255;
		mMasterCost = 255;
	}

	public Connection(Vector3 fromIndex, Vector3 toIndex, byte duckCost, byte masterCost)
	{
		mFromIndex = fromIndex;
		mToIndex = toIndex;
		mDuckCost = duckCost;
		mMasterCost = masterCost;
	}

	//public DuckTile mFromTile { get; set; }
	//public DuckTile mToTile { get; set; }
	public Vector3 mFromIndex { get; set; }
	public Vector3 mToIndex { get; set; }
    public byte mDuckCost { get; set; }
    public byte mMasterCost { get; set; }
}
