using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaitTypes
{
	INVALID = -1,
	ATTRACT,
	REPEL
}

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
		spawnBait(new Vector3(4, 0, 3), BaitTypes.ATTRACT);
		
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

    public GameObject duckLOSBait(Vector3 duckPos, float attractRange, DuckRotationState rotation)
    {
        List<GameObject> processedBaits = new List<GameObject>();
        if(placedBaits.Count == 0)
        {
            return null;
        }

        foreach(GameObject bait in placedBaits)
        {
            Vector3 baitPosition = bait.transform.position;
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            if (Vector3.Dot(forward,(baitPosition - duckPos))> 0 && (baitPosition - duckPos).magnitude < attractRange)
            {
                //is within range find if anything is blocking in the way if nothing is or not the same verticality, put in list
                processedBaits.Add(bait);
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
                //is within range find if anything is blocking in the way if nothing is or not the same verticality, put in list
                processedBaits.Add(bait);
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
        GameObject shortestBait = baitList[0];
        baitList.Remove(shortestBait);
        float distance = (shortestBait.transform.position - duckPos).magnitude;

        foreach (GameObject bait in baitList)
        {
            float currentDistance = (bait.transform.position - duckPos).magnitude;
            if (currentDistance < distance)
            {
                distance = currentDistance;
                shortestBait = bait;
            }
        }

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
		placedBaits.Add(newBait);
	}

}
