using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogCollider : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "duck")
		{
			Vector3 colliderPosition = collision.gameObject.transform.position;
			Vector3 objPosition = transform.position;
			Vector3 originPosition = gameObject.transform.parent.position; 
			float x = objPosition.x;
			float z = objPosition.z;

			float originX = originPosition.x;
			float originZ = originPosition.z;

			Vector2 cogDirection = new Vector2(z - originZ, -(x - originX));
			Vector2 collisionDirection = new Vector2(colliderPosition.x - objPosition.x, colliderPosition.z - objPosition.z);

			if(Vector2.Dot(cogDirection,collisionDirection) > 0)
			{
				gameObject.GetComponentInParent<cogs>().updateInput(1);
			}
			else
			{
				gameObject.GetComponentInParent<cogs>().updateInput(-1);
			}
		
		}
	}

}
