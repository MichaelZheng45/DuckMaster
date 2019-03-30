using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnStyleCollider : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "duck")
		{
			gameObject.GetComponentInParent<turnstyles>().updateInput(gameObject);
		}
	}
}
