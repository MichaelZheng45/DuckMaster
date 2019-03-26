using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
	//list out the ability to do things
	[SerializeField]
	private traversibleGround traverseData;

	//all conditions and possible ones
	public bool canMove;
	public bool canPickUp;
	public bool isHoldingDuck;

    //moving data
    [SerializeField] float mVelocity;

    //component data
    Transform playerTransform;

    //pathfinding data;
    public List<Vector3> tilePath;
    bool moving;
    int tilePathIndex;
    public float approachValue;

    //Will: need this for Altimeter stuff
    //Vector3 currentTilePos;

    void Start()
    {
        playerTransform = gameObject.transform;
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

	public traversibleGround getTraverseData()
	{
		return traverseData;
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
}
