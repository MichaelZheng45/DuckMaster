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
	[SerializeField] List<GameObject> baitObjects;

	List<GameObject> placedBaits;

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

    public Vector3 duckLOSBait(Vector3 pos, float attractRange, DuckRotationState rotation)
    {
        foreach(GameObject bait in placedBaits)
        {
            Vector3 pos = bait.transform.position;
        }
        return Vector3.zero;
    }

	public Vector3 duckFindBait(Vector3 pos, float attractRange)
	{
		return Vector3.zero;
	}

	public void pickupNewBait(BaitTypes type)
	{
		baitAmount[(int)type]++;
	}

	public void removeBait(GameObject bait)
	{
		placedBaits.Remove(bait);
	}

	public void spawnBait(Vector3 pos, BaitTypes type)
	{
		//spawn bait
		GameObject newBait = Instantiate(baitObjects[(int)type], pos, gameObject.transform.rotation);
		placedBaits.Add(newBait);
	}

}
