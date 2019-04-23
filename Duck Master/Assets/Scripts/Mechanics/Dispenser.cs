using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
	[SerializeField] BaitTypes baitType;

	public void SpawnBait()
	{
		Vector3 position = transform.position;
		position = new Vector3(position.x, 0, position.y) + (transform.forward * 1);
		GameManager.Instance.GetBait().spawnDispenserBait(position, baitType);
	}
}
