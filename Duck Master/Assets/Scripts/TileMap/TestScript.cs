using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		/*
		List<DuckTile.TileType> temp1 = new List<DuckTile.TileType>()
		{ DuckTile.TileType.PassableBoth, DuckTile.TileType.PassableBoth, DuckTile.TileType.PassableBoth, DuckTile.TileType.PassableBoth };
		List<DuckTile.TileType> temp2 = new List<DuckTile.TileType>()
		{ DuckTile.TileType.UnpassableBoth };
		List<DuckTile.TileType> temp3 = new List<DuckTile.TileType>()
		{ DuckTile.TileType.UnpassableMaster, DuckTile.TileType.UnpasssableDuck, DuckTile.TileType.PassableBoth };
		List<List<DuckTile.TileType>> tileType = new List<List<DuckTile.TileType>>() { temp1, temp2, temp3 };

		List<bool> baitable1 = new List<bool>() { false, true, true, false };
		List<bool> baitable2 = new List<bool>() { true };
		List<bool> baitable3 = new List<bool>() { false, false, false };
		List<List<bool>> baitable = new List<List<bool>>() { baitable1, baitable2, baitable3 };

		List<bool> change1 = new List<bool>() { true, false, false, true };
		List<bool> change2 = new List<bool>() { false };
		List<bool> change3 = new List<bool>() { true, true, true };
		List<List<bool>> change = new List<List<bool>>() { change1, change2, change3 };

		DuckTileGrid grid = new DuckTileGrid(tileType, baitable, change);

		temp1 = new List<DuckTile.TileType>()
		{ DuckTile.TileType.PassableBoth, DuckTile.TileType.PassableBoth, DuckTile.TileType.PassableBoth, DuckTile.TileType.PassableBoth };
		temp2 = new List<DuckTile.TileType>()
		{ DuckTile.TileType.UnpassableBoth };
		temp3 = new List<DuckTile.TileType>()
		{ DuckTile.TileType.UnpassableMaster, DuckTile.TileType.UnpasssableDuck, DuckTile.TileType.PassableBoth };
		tileType = new List<List<DuckTile.TileType>>() { temp3, temp2, temp1 };

		baitable1 = new List<bool>() { false, true, true, false };
		baitable2 = new List<bool>() { true };
		baitable3 = new List<bool>() { false, false, false };
		baitable = new List<List<bool>>() { baitable3, baitable2, baitable1 };

		change1 = new List<bool>() { true, false, false, true };
		change2 = new List<bool>() { false };
		change3 = new List<bool>() { true, true, true };
		change = new List<List<bool>>() { change3, change2, change1 };

		DuckTileGrid grid2 = new DuckTileGrid(tileType, baitable, change);

		DuckTileMap map = new DuckTileMap(new List<DuckTileGrid>() { grid, grid2 });
		*/

		List<List<DuckTile.TileType>> tileTypes = new List<List<DuckTile.TileType>>() {
			new List<DuckTile.TileType>() { DuckTile.TileType.PassableBoth,  DuckTile.TileType.PassableBoth,  DuckTile.TileType.PassableBoth },
			new List<DuckTile.TileType>() { DuckTile.TileType.INVALID_TYPE,  DuckTile.TileType.INVALID_TYPE,  DuckTile.TileType.INVALID_TYPE },
			new List<DuckTile.TileType>() { DuckTile.TileType.INVALID_TYPE,  DuckTile.TileType.INVALID_TYPE,  DuckTile.TileType.INVALID_TYPE } };

		List<List<bool>> baitable = new List<List<bool>>
		{
			new List<bool>() { true, true, true },
			new List<bool>() { true, true, true },
			new List<bool>() { true, true, true }
		};

		List<List<bool>> heightChange = new List<List<bool>>
		{
			new List<bool>() { true, true, true },
			new List<bool>() { false, false, false },
			new List<bool>() { false, false, false }
		};

		DuckTileGrid grid1 = new DuckTileGrid(tileTypes, baitable, heightChange, 0);



		tileTypes = new List<List<DuckTile.TileType>>() {
			new List<DuckTile.TileType>() { DuckTile.TileType.INVALID_TYPE,  DuckTile.TileType.INVALID_TYPE,  DuckTile.TileType.INVALID_TYPE },
			new List<DuckTile.TileType>() { DuckTile.TileType.PassableBoth,  DuckTile.TileType.PassableBoth,  DuckTile.TileType.PassableBoth },
			new List<DuckTile.TileType>() { DuckTile.TileType.PassableBoth,  DuckTile.TileType.PassableBoth,  DuckTile.TileType.PassableBoth } };

		baitable = new List<List<bool>>
		{
			new List<bool>() { true, true, true },
			new List<bool>() { true, true, true },
			new List<bool>() { true, true, true }
		};

		heightChange = new List<List<bool>>
		{
			new List<bool>() { false, false, false },
			new List<bool>() { true, true, true },
			new List<bool>() { false, false, false }
		};

		DuckTileGrid grid2 = new DuckTileGrid(tileTypes, baitable, heightChange, 1);

		DuckTileMap map = new DuckTileMap(new List<DuckTileGrid>() { grid1, grid2 });
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
