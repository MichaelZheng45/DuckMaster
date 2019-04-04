using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cogs : MonoBehaviour
{
    [SerializeField]int state = 0; 
    int getState() { return state; }

	bool disableTurn = false;
	float disableTimer = 1.5f;
	float disableCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(disableTurn)
		{
			disableCounter += Time.deltaTime;
			if(disableCounter > disableTimer)
			{
				disableTurn = false;
			}
		}
    }

	//+ means clockwise, - means counterclockwise
	public void updateInput(int rotateDirection)
	{
		if(disableTurn == false)
		{
			disableTurn = true;
			disableCounter = 0;
			state += rotateDirection;

			transform.rotation = Quaternion.Euler(new Vector3(0, 90*state, 0));

			//send input out
		}

	}
}
