using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
	//all conditions and possible ones
	public bool canMove;
	public bool canPickUp;
	public bool isHoldingDuck;

    //moving data
    [SerializeField] float mVelocity;

    //component data
    Transform playerTransform;
	BaitSystem mBaitSystem;

    //pathfinding data;
    public List<Vector3> tilePath;
    bool moving;
    int tilePathIndex;
    public float approachValue;

	//throw data
	[SerializeField]float throwDistance = 4;

    void Start()
    {
        playerTransform = gameObject.transform;
		mBaitSystem = gameObject.GetComponent<BaitSystem>();
        moving = false ;
        tilePath = new List<Vector3>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(moving)
        {
            movePaths(); 
        }
    }

    //move through the path list
    void movePaths()
    {
        Vector3 direction = (tilePath[tilePathIndex] - playerTransform.position);

        playerTransform.position += direction.normalized * mVelocity;

        if(direction.magnitude < approachValue)
        {
            tilePathIndex--;
        }

        if(tilePathIndex < 0)
        {
            moving = false;
        }
    }

    public void applyNewPath(List<Vector3> newPath)
    {
        moving = true;
        tilePath = newPath;
        tilePathIndex = tilePath.Count -1;
    }

    //Will: need this for Zipline stuff
    public bool CheckMoving()
    {
        return moving;
    }

    //Will: Altimeter processing
    private void OnTriggerEnter(Collider other)
    {
        //Should probably switch over to a tag system maybe, but this will do for now
        if (other.gameObject.name == "ground(Clone)")
        {
            GameManager.Instance.GetAltimeter().CheckAltitude(other.transform.position);
        }
    }

	//only throw onto standard block tiles
	public void throwBait(Vector3 target, BaitTypes type) //TO DO: LOOK AT TILE AND IF IT IS STANDARD TILE THROW IT
	{
		//check for distance
		if ((gameObject.transform.position - target).magnitude <= throwDistance && mBaitSystem.checkBait(type))
		{
			//throw object
			mBaitSystem.spawnBait(target, type);
		}
		else
		{
			//Ran out of bait
		}
	}
}
