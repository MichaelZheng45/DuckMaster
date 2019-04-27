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
    Dictionary<BaitTypes, int> baitAmount = new Dictionary<BaitTypes, int>();
    [SerializeField] GameObject baitObject; //prefab list

    List<BaitTypeHolder> placedBaits = new List<BaitTypeHolder>();
    float heightAdd = .5f;

    private void Start()
    {

        //temp
        //spawnBait(new Vector3(4, 0, 3), BaitTypes.REPEL);

    }
    //checks if that bait is available, returns true
    public bool checkBait(BaitTypes type)
    {
        if (baitAmount[type] > 0)
        {
            baitAmount[type]--;
            return true;
        }

        return false;
    }

    public void setBaitAmounts(int attract, int repel, int pepper)
    {
        baitAmount.Add(BaitTypes.ATTRACT, attract);
        baitAmount.Add(BaitTypes.REPEL, repel);
        baitAmount.Add(BaitTypes.PEPPER, pepper);
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
            if (length < direction.magnitude)
            {
                DuckTile tile = tileMap.getTileFromPosition(duckPos + (length * direction.normalized));
                if (tile.mHeight != currentHeight || tile.mType == DuckTile.TileType.UnpassableBoth || tile.mType == DuckTile.TileType.UnpasssableDuck)
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

    public BaitTypeHolder duckLOSBait(Vector3 forward, Vector3 duckPos, float attractRange)
    {
        List<BaitTypeHolder> processedBaits = new List<BaitTypeHolder>();
        if (placedBaits.Count == 0)
        {
            return null;
        }

        foreach (BaitTypeHolder bait in placedBaits)
        {
            Vector3 baitPosition = bait.transform.position;
            Debug.Log(Vector3.Dot((baitPosition - duckPos), forward));
            if (Vector3.Dot((baitPosition - duckPos), forward) >= 0 && (baitPosition - duckPos).magnitude < attractRange)
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

    public BaitTypeHolder duckFindBait(Vector3 duckPos, float attractRange)
    {
        List<BaitTypeHolder> processedBaits = new List<BaitTypeHolder>();
        foreach (BaitTypeHolder bait in placedBaits)
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

    BaitTypeHolder getShortestBait(List<BaitTypeHolder> baitList, Vector3 duckPos)
    {
        BaitTypeHolder shortestBait = null;
        float distance = 0;

        foreach (BaitTypeHolder bait in baitList)
        {
            float currentDistance = (bait.transform.position - duckPos).magnitude;
            if (shortestBait)
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
        return shortestBait.GetComponent<BaitTypeHolder>();
    }

    public void pickupNewBait(BaitTypes type)
    {
        baitAmount[type]++;
    }

    public void removeBait(BaitTypeHolder bait)
    {
        placedBaits.Remove(bait);
        baitAmount[bait.GetBaitType()]++;
        Destroy(bait.gameObject);
    }

    public void spawnBait(Vector3 pos, BaitTypes type)
    {
        //spawn bait
        if (baitAmount[type] > 0)
        {
            GameObject g = Instantiate(baitObject, pos + new Vector3(0, heightAdd, 0), gameObject.transform.rotation);
            g.GetComponent<BaitTypeHolder>().SetBaitType(type);
            baitAmount[type]--;
            placedBaits.Add(g.GetComponent<BaitTypeHolder>());
           // if (baitAmount[type] == 0)
           //     FindObjectOfType<UIManager>().SetBaitType("INVALID");
        }
    }

    //Will: Need a reference to the GameObject for the dispenser
    public GameObject spawnDispenserBait(Vector3 pos, BaitTypes type)
    {
        //spawn bait
        GameObject g = Instantiate(baitObject, pos + new Vector3(0, heightAdd, 0), gameObject.transform.rotation);
        g.GetComponent<BaitTypeHolder>().SetBaitType(type);
        placedBaits.Add(g.GetComponent<BaitTypeHolder>());
        return g;
    }

    public int GetBaitAmount(BaitTypes type)
    {
        return baitAmount[type];
    }

    /*
   public List<GameObject> GetBaitObjects()
   {
       return baitObjects;
   }
   */
}
