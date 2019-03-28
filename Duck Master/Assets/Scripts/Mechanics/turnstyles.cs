using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnstyles : MonoBehaviour
{
	// Start is called before the first frame update

	[SerializeField] GameObject downCollider;
	[SerializeField] GameObject upCollider;
	[SerializeField] int min;
	[SerializeField] int max;
	[SerializeField] int targetvalue;

	int output = 0;
	int getOutput(){ return output; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void updateInput(GameObject collider)
	{
		if (upCollider == collider)
		{
			output++;
		}
		else
		{
			output--;
		}
	}
}
