using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unfreindlyScript : MonoBehaviour
{
	List<tile> untouchables;
	[SerializeField] int range;

	public Transform unitTransform;

    // Start is called before the first frame update
    void Start()
    {
		untouchables = new List<tile>();
		unitTransform = gameObject.transform;
		GameManager.Instance.addUnFriendly(this);
		GameManager.Instance.markUnfreindlies(ref untouchables, unitTransform.position, range);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	//if ever moving then add more code
}
