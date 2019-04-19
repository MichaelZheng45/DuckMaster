using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaitTypes
{
	INVALID = -1,
	ATTRACT,
	REPEL,
    PEPPER
}

/*
          O   O   W    W     W  O   O
          O   0    W  W W   W   O   0
          0   0     WW  WW W    0   0
           000       W    W      000
*/
public class BaitSystem : MonoBehaviour
{
	[Tooltip("Holds the data for the amount of baits")]
	[SerializeField]List<int> baitAmount;
	[SerializeField] List<GameObject> baitObjects; //prefab list

	List<GameObject> placedBaits;
	float heightAdd = .5f;


    private void Start()
    {
        placedBaits = new List<GameObject>();

		//temp
		//spawnBait(new Vector3(4, 0, 3), BaitTypes.REPEL);
		
    }
    //checks if that bait is available, returns true
    public bool checkBait(BaitTypes type)
	{
		int index = (int)type;
		if (baitAmount[index] > 0)
		{
			baitAmount[index]--;
			return true;
		}

		return false;
	}

    bool raycastToObject(Vector3 baitPosition, Vector3 duckPos)
    {
        //is within range find if anything is blocking in the way if nothing is or not the same verticality, put in list
        DuckTileMap tileMap = GameManager.Instance.GetTileMap();
        float interval = .33f;
        int currentHeight = tileMap.getTileFromPosition(duckPos).mHeight;
        Vector3 direction = baitPosition - duckPos;

        int processCount = 0;
        bool endOfRay = false;
        while (!endOfRay)
        {
            float length = interval * processCount;
            if(length < direction.magnitude)
            {
                DuckTile tile = tileMap.getTileFromPosition(duckPos + (length * direction.normalized));
                if(tile.mHeight != currentHeight || tile.mType == DuckTile.TileType.UnpassableBoth || tile.mType == DuckTile.TileType.UnpasssableDuck)
                {
                    return false;
                }
            }
            else
            {
                endOfRay = true;
            }
            processCount++;
        }
        return true;
    }

    public GameObject duckLOSBait(Vector3 forward,Vector3 duckPos, float attractRange)
    {
        List<GameObject> processedBaits = new List<GameObject>();
        if(placedBaits.Count == 0)
        {
            return null;
        }

        foreach(GameObject bait in placedBaits)
        {
            Vector3 baitPosition = bait.transform.position;

            if (Vector3.Dot((baitPosition - duckPos), forward) > 0 && (baitPosition - duckPos).magnitude < attractRange)
            {
                if(raycastToObject(baitPosition,duckPos))
                {
                    processedBaits.Add(bait);
                }
            }
        }

        if(processedBaits.Count > 0)
        {
            return getShortestBait(processedBaits, duckPos);
        }
        else
        {
            return null;
        }
    }

	public GameObject duckFindBait(Vector3 duckPos, float attractRange)
	{
        List<GameObject> processedBaits = new List<GameObject>();
        foreach (GameObject bait in placedBaits)
        {
            Vector3 baitPosition = bait.transform.position;
            if ((baitPosition - duckPos).magnitude < attractRange)
            {
                if (raycastToObject(baitPosition, duckPos))
                {
                    processedBaits.Add(bait);
                }
            }
        }

        if (processedBaits.Count > 0)
        {
            return getShortestBait(processedBaits, duckPos);
        }
        else
        {
            return null;
        }
    }

    GameObject getShortestBait(List<GameObject> baitList,Vector3 duckPos)
    {
        GameObject shortestBait = null;
        float distance = 0;

        foreach (GameObject bait in baitList)
        {
            float currentDistance = (bait.transform.position - duckPos).magnitude;
            if(shortestBait)
            {
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    shortestBait = bait;
                }
            }
            else
            {
                shortestBait = bait;
            }
        }
        distance = (shortestBait.transform.position - duckPos).magnitude;
        return shortestBait;
    }

	public void pickupNewBait(BaitTypes type)
	{
		baitAmount[(int)type]++;
	}

	public void removeBait(GameObject bait)
	{
		placedBaits.Remove(bait);
		Destroy(bait);
	}

	public void spawnBait(Vector3 pos, BaitTypes type)
	{
		//spawn bait
		GameObject newBait = Instantiate(baitObjects[(int)type], pos + new Vector3(0,heightAdd,0), gameObject.transform.rotation);
        baitAmount[(int)type]--;
		placedBaits.Add(newBait);
	}

    public int GetBaitAmount(BaitTypes type)
    {
        return baitAmount[(int)type];
    }

    public List<GameObject> GetBaitObjects()
    {
        return baitObjects;
    }

}
