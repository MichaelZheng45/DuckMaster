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

    //How high above should the duck be
    private float aboveTileHeight = .5f;
    //check to start following
    [SerializeField]
    private bool startFollowing;
    //the range to start following
    [SerializeField]
    private float followThreshold;
    // velocity to follow
    [SerializeField]
    private float followVelocity;
    //circle size of the target point, so not a direct movement to target
    [SerializeField]
    private float targetRadius;
    // the distance to then update new target
    [SerializeField]
    private float toPointDistance;

    //list of the targets
    private Queue<Vector3> positionListData;

    //timer to create new target point path
    [SerializeField]
    private float updatePositionTime;
    private float updateTimeCount = 0;
    private int positionCount = 0;
    private Vector3 targetPoint;


    [Header("Pathfinding Data")]
    private int tilePathIndex;
    [SerializeField]
    //distance to change to next node in path
    private float pathApproachValue;
    [SerializeField]
    //velocity of path
    private float pathVelocity;
    //pathfinding data
    private List<Vector3> tilePath;

    [Header("Hold Data")]
    //hold data
    [SerializeField]
    //height for duck when being held
    private float duckHeightAtHold;

    //throw data
    [Header("Throw Data")]
    private Vector3 startingPos;
    private Vector3 targetPos;
    [SerializeField]
    private float startingVelocity;
    [SerializeField]
    private float gravity;
    private float maxAirTime;
    private float currentAirTime;
    private Vector3 initialVelocity;

    //run data
    [Header("Run Data")]
    [SerializeField]
    //range to start fleeing
    private float fleeRange;
    private Vector3 runTar;
    private float runToApproach = .3f;
    [SerializeField]
    //run away speed
    private float runVelocity;

    //bait data
    [Header("Bait Data")]
    [SerializeField]
    private GameObject baitSystemObject;
    [SerializeField]
    private float attractDistance = 3;

    private BaitSystem baitSystem;
    private GameObject targetBait;
    private float duckBaitedVelocity = .1f;
    private float duckAtBaitDistance = .2f;

    [Header("Misc")]
    [SerializeField]
    private Transform playerTransform;
    private DuckRotation mDuckRotation;
    private Transform duckTransform;

    //frameCount
    private float runCheckPerFrame = .5f;
    private float frameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        ChangeDuckState(DuckStates.FOLLOW);
        tilePath = new List<Vector3>();
        positionListData = new Queue<Vector3>();

        mDuckRotation = gameObject.GetComponent<DuckRotation>();
        baitSystem = baitSystemObject.GetComponent<BaitSystem>();
        duckTransform = gameObject.transform;
    }

    private void Update()
    {
        //if (duckTransform == null)
        //    print("Duck transform is NULL - duckbehavior");

        //every or so frame check if duck is near unfreindlies
        if (frameCount > runCheckPerFrame && mDuckState != DuckStates.RUN)
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
                mDuckRotation.rotateDuck(runTar-duckTransform.position);

                ChangeDuckState(DuckStates.RUN);

                positionListData.Clear(); //clear all follow positions
            }

            //check for baits (line of sight)
            if (mDuckState == DuckStates.RETURN)
            {
                //check bait system for objects in line of sight
                DuckRotationState rotation = mDuckRotation.currentRotation;
                //GameObject target = mBaitSystem.duckLOSBait(duckTransform.position, attractDistance, rotation);
                GameObject target = baitSystem.duckLOSBait(transform.position, attractDistance, rotation);


                if (target != null)
                {
                    ChangeDuckState(DuckStates.BAITED);
                    targetBait = target;
                }
            }
        }
        frameCount += Time.deltaTime;
    }

    void ChangeDuckState(DuckStates newDuckstate)
    {
        mDuckState = newDuckstate;
        UpdateAnimationState();
    }

    void UpdateAnimationState()
    {
        switch (mDuckState)
        {
            case DuckStates.INAIR:
                AnimationEventStuff.DuckmasterThrowing();
                break;
            case DuckStates.RUN:
                AnimationEventStuff.DuckWalkingChange(true);
                break;
            case DuckStates.RETURN:
                AnimationEventStuff.DuckWalkingChange(true);
                break;
            case DuckStates.STILL:
                AnimationEventStuff.DuckWalkingChange(false);
                break;
            case DuckStates.HELD:
                AnimationEventStuff.DuckWalkingChange(false);
                break;

            default:

                break;

        }
    }

    void FixedUpdate()
    {

        if (mDuckState == DuckStates.RUN) //run away ducko! The unfriendlies
        {
            Vector3 dir = (runTar - duckTransform.position);
            if (dir.magnitude < runToApproach)
            {
                ChangeDuckState(DuckStates.STILL);
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
                //duckTransform.position = new Vector3(xPos, yPos, zPos);
                transform.position = new Vector3(xPos, yPos, zPos);
            }
            else
            {
                ChangeDuckState(DuckStates.STILL);
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
                AnimationEventStuff.DuckWalkingChange(startFollowing);
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
            else if (mDuckState == DuckStates.BAITED)
            {

            }

            if (mDuckState == DuckStates.HELD)
            {
                //print("Player position duck: " + playerTransform.position.ToString());
                //print("duck position duck beh " + duckTransform.position.ToString());
                //duckTransform.position = playerTransform.position + new Vector3(0, duckHeightAtHold, 0);
                transform.position = playerTransform.position + new Vector3(0, duckHeightAtHold, 0);
                transform.rotation = playerTransform.rotation;
            }
        }
    }

    //move through the given path
    void movePaths()
    {
        //Vector3 direction = (tilePath[tilePathIndex] - duckTransform.position);
        Vector3 direction = (tilePath[tilePathIndex] + new Vector3(0,aboveTileHeight,0) - transform.position);
       
        duckTransform.position += direction.normalized * pathVelocity;


        //approaches the next tile, update new target tile to move to
        if (direction.magnitude < pathApproachValue)
        {
            tilePathIndex--;
           
            if (tilePathIndex < 0)
            {
                tilePath.Clear();
                ChangeDuckState(DuckStates.FOLLOW);

                //begin following
                targetPoint = positionListData.Dequeue();
            
            }
            else
            {
                mDuckRotation.rotateDuck((tilePath[tilePathIndex] - transform.position).normalized);
            }

        }

        //updateTimer for follow, this way it will move towards player from pathfinding
        //then will have an already follow path to follow once follow takes over
        updateTimer();
    }

    void followPlayer()
    {
        float playerDistance = (new Vector2(duckTransform.position.x, duckTransform.position.z) - new Vector2(playerTransform.position.x, playerTransform.position.z)).magnitude;
        if (playerDistance > followThreshold)
        {
            AnimationEventStuff.DuckWalkingChange(true);
        }
        else
        {
            AnimationEventStuff.DuckWalkingChange(false);
        }
        //check if out of threshold
        if (playerDistance > followThreshold && startFollowing == false)
        {
            startFollowing = true;
            addnewPos();

        }
        else if (playerDistance < followThreshold)
        {
            if(positionCount != 0)
            {
                //reset data
                //Debug.Log("Resetting");
                targetPoint = Vector3.zero;
                startFollowing = false;
                positionListData.Clear();
                updateTimeCount = 0;
                positionCount = 0;
            }
        }

        if (startFollowing)
        {
            //check if the target is null, add new target
            if (targetPoint == Vector3.zero)
            {
                //if there are none in the list, create new one
                if (positionCount == 0)
                {
                    Debug.Log("Adding Pos Test 1");
                    addnewPos();
                }
                targetPoint = positionListData.Dequeue();
                mDuckRotation.rotateDuck((targetPoint - duckTransform.position).normalized);
                positionCount--;
            }

            //find direction and follow

            Vector3 dir = targetPoint - duckTransform.position;
            duckTransform.position += dir.normalized * followVelocity;        

            //check if approaching distance
            if (dir.magnitude < toPointDistance)
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
        newPos += new Vector3(Random.Range(-targetRadius * 100, targetRadius * 100) / 100,-aboveTileHeight, Random.Range(-targetRadius * 100, targetRadius * 100) / 100);
        positionListData.Enqueue(newPos);
        positionCount++;
    }

    //send in the new path to be read and activate return
    public void applyNewPath(List<Vector3> newPath)
    {
        ChangeDuckState(DuckStates.RETURN);
        tilePath = newPath;
        tilePathIndex = tilePath.Count - 1;
        mDuckRotation.rotateDuck((tilePath[tilePathIndex] - transform.position).normalized);
    }

    public bool isRecallable()
    {
        if (mDuckState == DuckStates.STILL)
        {
            return true;
        }
        return false;
    }

    public void pickUpDuck()
    {
        ChangeDuckState(DuckStates.HELD);
        positionListData.Clear();
        //place duck ontop of player 
    }

    void runToBait()
    {
        Vector3 direction = targetBait.transform.position - duckTransform.position;
        duckTransform.position += direction.normalized * duckBaitedVelocity;
        // transform.position += direction.normalized * duckBaitedVelocity;

        if (direction.magnitude < duckAtBaitDistance)
        {
            baitSystem.removeBait(targetBait);
            //targetBait = baitSystem.duckFindBait(duckTransform.position, attractDistance);
            targetBait = baitSystem.duckFindBait(transform.position, attractDistance);
            if (targetBait == null)
            {
                ChangeDuckState(DuckStates.STILL);
            }
        }
    }

    public void throwDuck(Vector3 target)
    {
        ChangeDuckState(DuckStates.INAIR);

        startingPos = duckTransform.position;
        //startingPos = transform.position;
        targetPos = target;

        Vector3 dir = new Vector3(targetPos.x - startingPos.x, 0, targetPos.z - startingPos.z);

        float distance = dir.magnitude;
        float heightDiff = targetPos.y - startingPos.y; //difference in height between two points
        float theta = Mathf.Atan((Mathf.Pow(startingVelocity, 2) + Mathf.Sqrt(Mathf.Pow(startingVelocity, 4) + gravity * (gravity * distance * distance + (2 * heightDiff * Mathf.Pow(startingVelocity, 2))))) / (gravity * distance));

        heightDiff = startingPos.y - targetPos.y - 1; //initial height compared to the ground 0, which is tile position + 1
        maxAirTime = (startingVelocity * Mathf.Sin(theta) + Mathf.Sqrt(Mathf.Pow(startingVelocity * Mathf.Sin(theta), 2) + 2 * gravity * heightDiff)) / gravity;
        currentAirTime = 0;

        mDuckRotation.rotateDuck(dir.normalized);
        dir = dir.normalized * Mathf.Cos(theta);
        initialVelocity = new Vector3(dir.x, Mathf.Sin(theta), dir.z) * startingVelocity;


    }
}
