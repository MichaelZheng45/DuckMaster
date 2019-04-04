using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altimeter : LogicInput
{
    [SerializeField] int triggerHeight = 0;
    bool active;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CheckAltitude(Vector3 pos)
    {
        //print("pos for altitude is " + pos.ToString());
        //print("Floored " + Mathf.FloorToInt(pos.x).ToString() + ", " + Mathf.FloorToInt(pos.z));
        //DuckTile current = GameManager.Instance.tileMap.mHeightMap.GetTile(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
        //DuckTileMap map = GameManager.Instance.tileMap;
        //DuckTileGrid grid = GameManager.Instance.tileMap.mHeightMap;

        //if (current != null)
        //{
        //    if (current.mHeight == triggerHeight)
        //    {
        //        print("Altimeter activated!");
        //        active = true;
        //    }
        //    else
        //    {
        //        print("Altimeter deactivated");
        //        active = false;
        //    }
        //}
        //
        //else
        //    print("current tile is null");
    }

    public override bool IsActive()
    {
        return active;
    }
}

