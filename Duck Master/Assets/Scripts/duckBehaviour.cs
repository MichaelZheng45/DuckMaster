using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DuckStates
{
	INVALID = -1,
	STILL,
	FOLLOW,
	HELD,
	RETURN,
	RUN,
	INAIR,
	TRAPPED,
	BAITED
}

public class duckBehaviour : MonoBehaviour
{
	public DuckStates mDuckState;
	public DuckStates GetDuckStates() { return mDuckState; }

	//follow data
	[Header("Follow Data")]
	[SerializeField] bool startFollowing; //check to start following
	[SerializeField] float followThreshold; //the range to start following
	[SerializeField] float followVelocity; // velocity to follow
	[SerializeField] float targetRadius; //circle size of the target point, so not a direct movement to target
	[SerializeField] float toPointDistance; // the distance to then update new target
	Queue<Vector3> positionListData; //list of the targets
	[SerializeField] float updatePositionTime; //timer to create new target point path
	float updateTimeCount = 0;
	int positionCount = 0;
	Vector3 targetPoint;

	[Header("Pathfinding Data")]
	//pathfindin data
	List<Vector3> tilePath;
	int tilePathIndex;
	[SerializeField] float pathApproachValue; //distance to change to next node in path
	[SerializeField] float pathVelocity; //velocity of path

	[Header("Hold Data")]
	//hold data
	[SerializeField] float duckHeightAtHold; //height for duck when being held

	//throw data
	[Header("Throw Data")]
	Vector3 startingPos;
	Vector3 targetPos;
	[SerializeField] float startingVelocity; //self explanatory
	[SerializeField] float gravity;
	float maxAirTime;
	float currentAirTime;
	Vector3 initialVelocity;

	//run data
	[Header("Run Data")]
	[SerializeField] float fleeRange; //range to start fleeing
	Vector3 runTar;
	float runToApproach = .3f;
	[SerializeField]float runVelocity; //run away speed

	//bait data
	[Header("Bait Data")]
	[SerializeField] GameObject baitSystemObject;
	[SerializeField] float attractDistance = 3; //<--temp	
	BaitSystem baitSystem;
    GameObject targetBait;
    float duckBaitedVelocity = .1f;
    float duckAtBaitDistance = .2f;

	[Header("Misc")]
	[SerializeField] Transform playerTransform;
	BaitSystem mBaitSystem;
    DuckRotation mDuckRotation;
	Transform duckTransform;

	//frameCount
	float runCheckPerFrame = .5f;
	float frameCount = 0;

	// Start is called before the first frame update
	void Start()
    {
		mDuckState = DuckStates.FOLLOW;
		tilePath = new List<Vector3>();
        positionListData = new Queue<Vector3>();

        mDuckRotation = gameObject.GetComponent<DuckRotation>();
		baitSystem = baitSystemObject.GetComponent<BaitSystem>();
        duckTransform = gameObject.transform;
	
    }

	private void Update()
	{
		//every or so frame check if duck is near unfreindlies
		if(frameCount > runCheckPerFrame && mDuckState != DuckStates.RUN)
		{
			//flee from unfreindlies
			runTar = GameManager.Instance.checkToRun(fleeRange);
			frameCount = 0;

			if (runTar != Vector3.zero)
			{
				if (mDuckState == DuckStates.INAIR)
				{
					runTar = playerTransform.position;
				}
				mDuckState = DuckStates.RUN;
				positionListData.Clear(); //clear all follow positions
			}

			//check for baits (line of sight)
			if(mDuckState == DuckStates.RETURN)
			{
                //check bait system for objects in line of sight
                DuckRotationState rotation = mDuckRotation.GetRotationState();
                GameObject target = mBaitSystem.duckLOSBait(duckTransform.position, attractDistance, rotation);

                if(target != null)
                {
                    mDuckState = DuckStates.BAITED;
                    targetBait = target;
                }
			}
		}
		frameCount+= Time.deltaTime;
	}

	void FixedUpdate()
	{
		
		if (mDuckState == DuckStates.RUN) //run away ducko! The unfriendlies
		{
			Vector3 dir = (runTar - duckTransform.position);
			if (dir.magnitude < runToApproach)
			{
				mDuckState = DuckStates.STILL;
			}
			else
			{
				duckTransform.position += dir.normalized * runVelocity;
			}
		}
		else if (mDuckState == DuckStates.INAIR)
		{
			currentAirTime += Time.deltaTime;
			if (currentAirTime < maxAirTime)
			{
				float xPos = startingPos.x + (initialVelocity.x * currentAirTime);
				float yPos = startingPos.y + (initialVelocity.y * currentAirTime) - ((gravity * currentAirTime * currentAirTime) / 2);
				float zPos = startingPos.z + (initialVelocity.z * currentAirTime);
				duckTransform.position = new Vector3(xPos, yPos, zPos);
			}
			else
			{
				mDuckState = DuckStates.STILL;
				//check if landed on geyser
				Vector3 target = GameManager.Instance.checkGeyser(targetPos, startingPos);
				if (target != Vector3.zero)
				{
					throwDuck(target);
				}
			}
		}
		else
		{
			int tilePathCount = tilePath.Count;
			if (mDuckState == DuckStates.FOLLOW) //follow
			{
				if (positionListData.Count == 0)
				{
					addnewPos();
				}
				followPlayer();
			}
			else if (mDuckState == DuckStates.RETURN && tilePathCount != 0) //recall
			{
				movePaths();
			}
            else if(mDuckState == DuckStates.BAITED)
            {
                
            }

			if (mDuckState == DuckStates.HELD)
			{
				duckTransform.position = playerTransform.position + new Vector3(0, duckHeightAtHold, 0);
			}
		}
	}

	//move through the given path
	void movePaths()
	{
		Vector3 direction = (tilePath[tilePathIndex] - duckTransform.position);
		
		duckTransform.position += direction.normalized * pathVelocity;

		//approaches the next tile, update new target tile to move to
		if (direction.magnitude < pathApproachValue)
		{
			tilePathIndex--;
		}

		//updateTimer for follow, this way it will move towards player from pathfinding
		//then will have an already follow path to follow once follow takes over
		updateTimer();

		if (tilePathIndex < 0)
		{
			tilePath.Clear();
			mDuckState = DuckStates.FOLLOW;

			//begin following
			targetPoint = positionListData.Dequeue();
		}
	}

	void followPlayer()
    {
        //check if out of threshold
        if((duckTransform.position - playerTransform.position).magnitude > followThreshold && startFollowing == false)
        {
            startFollowing = true;
            addnewPos();
   
        }
        else if((duckTransform.position - playerTransform.position).magnitude < followThreshold)
        {
            //reset data
            targetPoint = Vector3.zero;
            startFollowing = false;
            positionListData.Clear();
            updateTimeCount = 0;
            positionCount = 0;
        }

        if(startFollowing)
        {
            //check if the target is null, add new target
            if(targetPoint == Vector3.zero)
            {
                //if there are none in the list, create new one
               if(positionCount == 0)
               {
                    addnewPos();
               }
               targetPoint = positionListData.Dequeue();
               positionCount--;
            }

            //find direction and follow
            Vector3 dir = targetPoint - duckTransform.position;
            duckTransform.position += dir.normalized * followVelocity;        

            //check if approaching distance
            if(dir.magnitude < toPointDistance)
            {
                targetPoint = Vector3.zero;
                positionCount--;
            }

			updateTimer();
        }
    }

	//update timer to create a follow path
	void updateTimer()
	{
		updateTimeCount += Time.deltaTime;
		if (updateTimeCount > updatePositionTime)
		{
			addnewPos();
			updateTimeCount = 0;
		}
	}

	//find new target position in the follow path
    void addnewPos()
    {
		Vector3 newPos = playerTransform.position;
        newPos += new Vector3(Random.Range(-targetRadius * 100, targetRadius * 100) / 100, 0, Random.Range(-targetRadius * 100, targetRadius * 100) / 100);
        positionListData.Enqueue(newPos);
        positionCount++;
    }

	//send in the new path to be read and activate return
	public void applyNewPath(List<Vector3> newPath)
	{
		mDuckState = DuckStates.RETURN;
		tilePath = newPath;
		tilePathIndex = tilePath.Count - 1;
	}

	public bool isRecallable()
	{
		if(mDuckState == DuckStates.STILL)
		{
			return true;
		}
		return false;
	}

	public void pickUpDuck()
	{
		mDuckState = DuckStates.HELD;
		positionListData.Clear();
		//place duck ontop of player 
	}

    void runToBait()
    {
        Vector3 direction = targetBait.transform.position - duckTransform.position;
        duckTransform.position += direction.normalized * duckBaitedVelocity;

        if(direction.magnitude < duckAtBaitDistance)
        {
            baitSystem.removeBait(targetBait);
            targetBait = baitSystem.duckFindBait(duckTransform.position, attractDistance);
            if(targetBait == null)
            {
                mDuckState = DuckStates.STILL;
            }
        }
    }

	public void throwDuck(Vector3 target)
	{
		mDuckState = DuckStates.INAIR;

		startingPos = duckTransform.position;
		targetPos = target;

		Vector3 dir = new Vector3(targetPos.x - startingPos.x, 0, targetPos.z - startingPos.z);

		float distance = dir.magnitude;
		float heightDiff = targetPos.y - startingPos.y; //difference in height between two points
		float theta = Mathf.Atan((Mathf.Pow(startingVelocity, 2) + Mathf.Sqrt(Mathf.Pow(startingVelocity, 4) + gravity*(gravity*distance*distance + (2*heightDiff*Mathf.Pow(startingVelocity,2))))) / (gravity * distance));

		heightDiff = startingPos.y - targetPos.y - 1; //initial height compared to the ground 0, which is tile position + 1
		maxAirTime = (startingVelocity * Mathf.Sin(theta) + Mathf.Sqrt(Mathf.Pow(startingVelocity*Mathf.Sin(theta),2) + 2*gravity*heightDiff))/gravity;
		currentAirTime = 0;

		dir = dir.normalized * Mathf.Cos(theta);
		initialVelocity = new Vector3(dir.x, Mathf.Sin(theta), dir.z) * startingVelocity;
	}
}
