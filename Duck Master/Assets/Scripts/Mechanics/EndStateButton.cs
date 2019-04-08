using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStateButton : MonoBehaviour
{
	// Start is called before the first frame update

	[SerializeField]
    private float duckDistanceReq;

    Transform thisTransform;
    void Start()
    {
        thisTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        if (tag == "Player")
        {
			//check for duckTransform and see if nearby  
			if ((thisTransform.position - GameManager.Instance.getduckTrans().position).magnitude < duckDistanceReq)
            {

			}
        }
    }
}
