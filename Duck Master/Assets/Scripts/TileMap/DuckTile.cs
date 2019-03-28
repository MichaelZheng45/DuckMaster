public class DuckTile
{
    public enum ConnectionDirection
    {
        INVALID_DIRECTION = -1,
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }
    private const byte MAX_CONNECTIONS = 4;
	Connection[] mConnections;// = new Connection[MAX_CONNECTIONS];
	public enum TileType
	{
		INVALID_TYPE = -1,
		UnpassableBoth,
		UnpasssableDuck,
		UnpassableMaster,
		PassableBoth,
	}

	TileType mType;// = TileType.INVALID_TYPE;

	public DuckTile()
	{
		mConnections = new Connection[MAX_CONNECTIONS];
		mType = TileType.INVALID_TYPE;
	}

	public DuckTile(Connection[] connections, TileType type)
	{
		if(connections.Length <= MAX_CONNECTIONS)
		{
			mConnections = connections;
		}
		else
		{
			mConnections = new Connection[MAX_CONNECTIONS];
		}
		mType = type;
	}

    public bool SetConnectionDirection(ConnectionDirection direction, Connection connection)
    {
		if(ConnectionDirection.INVALID_DIRECTION == direction)
		{
			return false;
		}

		mConnections[(int)direction] = connection;
		return true;
    }

    public bool SetConnectionIndex(int index, Connection connection)
    {
		if(index < -1 || index > 4)
		{
			return false;
		}

		mConnections[index] = connection;
		return true;
    }

    public Connection GetConnectionDirection(ConnectionDirection direction)
    {
		if (ConnectionDirection.INVALID_DIRECTION == direction)
		{
			return null;
		}

		return mConnections[(int)direction];
	}

    public Connection GetConnectionIndex(int index)
    {
		if (index < -1 || index > 4)
		{
			return null;
		}

		return mConnections[index];
	}

    public byte GetCostDirection(ConnectionDirection direction)
    {
		if (ConnectionDirection.INVALID_DIRECTION == direction)
		{
			return Connection.MAX_COST;
		}

		return mConnections[(int)direction].mCost;
	}

	public byte GetCostIndex(int index)
	{
		if (index < -1 || index > 4)
		{
			return Connection.MAX_COST;
		}

		return mConnections[index].mCost;
	}
}
