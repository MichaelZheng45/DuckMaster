using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altimeter : MonoBehaviour, LogicInput
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
		DuckTile current = GameManager.Instance.tileMap.mHeightMap.GetTile(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));

        if (current != null)
        {
            if (current.mHeight == triggerHeight)
            {
                print("Altimeter activated!");
                active = true;
            }
            else
            {
                print("Altimeter deactivated");
                active = false;
            }
        }

        else
            print("current tile is null");
    }

    public bool IsActive()
    {
        return active;
    }
}

