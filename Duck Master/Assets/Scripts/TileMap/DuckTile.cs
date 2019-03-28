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

	bool mBaitable;

	// Default Constructor
	public DuckTile()
	{
		mConnections = new Connection[MAX_CONNECTIONS];
		mType = TileType.INVALID_TYPE;
		mBaitable = false;
	}

	// Sets to connections and type and baitable
	// If connections is invalid it is set to a new instead
	public DuckTile(Connection[] connections, TileType type, bool baitable)
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
		mBaitable = baitable;
	}

	// Set the specific direction connection
	// Returns false if failed
    public bool SetConnectionDirection(ConnectionDirection direction, Connection connection)
    {
		if(ConnectionDirection.INVALID_DIRECTION == direction)
		{
			return false;
		}

		mConnections[(int)direction] = connection;
		return true;
    }

	// Set the index connection
	// Returns if false if failed
    public bool SetConnectionIndex(int index, Connection connection)
    {
		if(index < -1 || index > 4)
		{
			return false;
		}

		mConnections[index] = connection;
		return true;
    }

	// Get the direction connection
	// Returns null if fails
    public Connection GetConnectionDirection(ConnectionDirection direction)
    {
		if (ConnectionDirection.INVALID_DIRECTION == direction)
		{
			return null;
		}

		return mConnections[(int)direction];
	}

	// Get index connection
	// Returns null if fails
    public Connection GetConnectionIndex(int index)
    {
		if (index < -1 || index > 4)
		{
			return null;
		}

		return mConnections[index];
	}

	// Get direction connection cost
	// Returns MAX_COST if fails
    public byte GetCostDirection(ConnectionDirection direction)
    {
		if (ConnectionDirection.INVALID_DIRECTION == direction)
		{
			return Connection.MAX_COST;
		}

		return mConnections[(int)direction].mCost;
	}

	// Get index connection cost
	// Returns MAX_COST if fails
	public byte GetCostIndex(int index)
	{
		if (index < -1 || index > 4)
		{
			return Connection.MAX_COST;
		}

		return mConnections[index].mCost;
	}
}
