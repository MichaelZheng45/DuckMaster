using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //DuckTileGrid grid = new DuckTileGrid(10, 10);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
