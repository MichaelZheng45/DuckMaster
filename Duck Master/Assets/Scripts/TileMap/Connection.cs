public class Connection
{
	public const byte MAX_COST = 255;

	public Connection()
	{
		mFromTile = null;
		mToTile = null;
		mCost = 255;
	}

	public Connection(DuckTile fromTile, DuckTile toTile, byte cost = MAX_COST)
	{
		mFromTile = fromTile;
		mToTile = toTile;
		mCost = cost;
	}

    public DuckTile mFromTile { get; set; }
    public DuckTile mToTile { get; set; }
    public byte mCost { get; set; }
}
