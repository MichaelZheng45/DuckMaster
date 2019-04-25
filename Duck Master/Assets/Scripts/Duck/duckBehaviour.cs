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
    BAITED,
    AT_APPLEBEES,
    BAIT_REPEL
}

public class duckBehaviour : MonoBehaviour
{
    public DuckStates mDuckState { get; set; }

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
    private float followVelocity = .0225f;
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
    //height for duck when being held
    [SerializeField] float duckHeightAtHold = .2f;

    //throw data
    [Header("Throw Data")]
    //fudge number to find totalTime
    float throwTimeFudge = .2f;
    float currentTime;
    float totalTime;
    float parabolaA;
    float parabolaB;
    float parabolaC;
    float throwDistance;
    Vector3 direction;
    Vector3 startingPos;
    Vector3 targetPos;
    //run data
    [Header("Run Data")]
    [SerializeField]
    //range to start fleeing
    private float fleeRange;
    private float runToApproach = .3f;
    [SerializeField]
    //run away speed
    private float runVelocity;
    Vector3 runTarget;
    [SerializeField]
    float verticalityChangeSpeed = 0.1f; //Will: the speed at which the duck lerps when it is moving to a lower tile

    //bait data
    [Header("Bait Data")]
    // FIX REFERENCE
    private GameObject baitSystemObject;
    [SerializeField]
    private float attractDistance = 3; //distance where the duck will come to the bait
    int repelDistance = 5; //how far will the duck run away
    int pepperLaunchDistance = 3;    //how far will the duck be launched via APPLEBEES
    private BaitSystem baitSystem;
    private BaitTypeHolder targetBait;
    private Vector3 repelTargetPos;
    private float duckBaitedVelocity = .05f;
    private float duckAtBaitDistance = .2f;
    private float duckAtRepelBaitDistance = 1.2f;

    // FIX REFERENCE
    private Transform playerTransform;
    private DuckRotation mDuckRotation;
    private Transform duckTransform;
    private Transform playerNeckTransform;

    //frameCount
    private float runCheckPerFrame = .3f;
    private float frameCount = 0;

    //animation bool checks
    bool beingThrown = false;
    bool startled = false;
    bool beingPickedUp = false;
    private GameObject playerHand;

    // Start is called before the first frame update
    void Start()
    {
        ChangeDuckState(DuckStates.FOLLOW);
        tilePath = new List<Vector3>();
        positionListData = new Queue<Vector3>();

        mDuckRotation = gameObject.GetComponent<DuckRotation>();
        duckTransform = gameObject.transform;

        playerTransform = GameObject.Find("Player").transform;
        baitSystemObject = playerTransform.GetChild(0).gameObject;

        baitSystem = baitSystemObject.GetComponent<BaitSystem>();

        playerHand = GameObject.FindGameObjectWithTag("Hand");
        playerNeckTransform = GameObject.FindGameObjectWithTag("Neck").transform;
    }

    private void Update()
    {
        //if (duckTransform == null)
        //    print("Duck transform is NULL - duckbehavior");

        //every or so frame check if duck is near unfreindlies
        if (frameCount > runCheckPerFrame && mDuckState != DuckStates.RUN)
        {
            //flee from unfreindlies
            runTarget = GameManager.Instance.checkToRun(fleeRange);
            frameCount = 0;

            if (runTarget != Vector3.zero)
            {
                startingPos = duckTransform.position;
                targetPos = runTarget;
                runTarget += new Vector3(0, aboveTileHeight, 0);
                mDuckRotation.rotateDuck(runTarget - duckTransform.position);

                ChangeDuckState(DuckStates.RUN);

                positionListData.Clear(); //clear all follow positions
            }

            //check for baits (line of sight)
            if (mDuckState == DuckStates.RETURN)
            {
                //check bait system for objects in line of sight
                BaitTypeHolder target = baitSystem.duckLOSBait(duckTransform.forward, transform.position, attractDistance);

                if (target != null)
                {
                    mDuckRotation.rotateDuck(target.transform.position - duckTransform.position);
                    ChangeDuckState(DuckStates.BAITED);
                    targetBait = target.GetComponent<BaitTypeHolder>();
                }
            }
        }
        frameCount += Time.deltaTime;
    }

    //duck is being prepped to throw (duck position is linked to player's hand)
    public void prepThrow()
    {
        beingThrown = true;
        AnimationEventStuff.Throw();
    }

    //duck is startled,maybe have a animation of the duck startled?
    public void startleduck()
    {
        startled = true;
        AnimationEventStuff.Whistle();
    }
    public void prepPickup()
    {
        AnimationEventStuff.Pickup();
    }


    void ChangeDuckState(DuckStates newDuckstate)
    {
        if (mDuckState == DuckStates.INAIR && newDuckstate != DuckStates.INAIR)
            AnimationEventStuff.DuckInAirChange(false);

        mDuckState = newDuckstate;
        UpdateAnimationState();
    }

    void UpdateAnimationState()
    {

        if (mDuckState == DuckStates.RUN || mDuckState == DuckStates.RETURN)
        {
            AnimationEventStuff.DuckWalkingChange(true);
        }
        else
        {
            AnimationEventStuff.DuckWalkingChange(false);
        }

        if (mDuckState == DuckStates.HELD)
        {
            AnimationEventStuff.Pickup();
            AnimationEventStuff.DuckHeldChange(true);
        }
        else
        {
            AnimationEventStuff.DuckHeldChange(false);
        }

        if (mDuckState == DuckStates.INAIR)
        {
            AnimationEventStuff.DuckInAirChange(true);
        }
    }

    void FixedUpdate()
    {

        if (mDuckState == DuckStates.RUN) //run away ducko! The unfriendlies
        {
            Vector3 dir = (runTarget - duckTransform.position);
            if (dir.magnitude < runToApproach)
            {
                ChangeDuckState(DuckStates.STILL);
            }
            else
            {
                duckTransform.position += dir.normalized * runVelocity;
            }
        }
        else if (mDuckState == DuckStates.INAIR || mDuckState == DuckStates.AT_APPLEBEES)
        {
            travelInAir();
        }
        else if (mDuckState == DuckStates.BAIT_REPEL)
        {
            repelledBait();
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
                attractedToBait();
            }

            if (mDuckState == DuckStates.HELD)
            {
                if (beingThrown || beingPickedUp)
                {
                    duckTransform.position = playerHand.transform.position;
                }
                else
                {
                    duckTransform.position = playerNeckTransform.position + playerNeckTransform.rotation * new Vector3(0, duckHeightAtHold, 0);

                }
                transform.rotation = playerTransform.rotation;
            }
        }
    }

    //flowing in air
    private void travelInAir()
    {
        currentTime += Time.deltaTime;
        if (currentTime < totalTime)
        {
            float n = throwDistance * (currentTime / totalTime);
            float yPos = startingPos.y + parabolaA * Mathf.Pow(n, 2) + parabolaB * n + parabolaC;
            Vector3 forward = startingPos + new Vector3(direction.x, 0, direction.z) * n;
            duckTransform.position = new Vector3(forward.x, yPos, forward.z);
        }
        else
        {
            //bool stillPeppered = false;

            //check if landed on geyser
            //Vector3 target = GameManager.Instance.checkGeyser(targetPos, startingPos);
            if (mDuckState == DuckStates.AT_APPLEBEES)
            {
                ChangeDuckState(DuckStates.BAITED);
                //   stillPeppered = true;

                // if (target == Vector3.zero)
                // {
                lookForBait();
                //}
            }
            else
            {
                ChangeDuckState(DuckStates.STILL);
            }

            //if there is geyser rethrow
            //if (target != Vector3.zero)
            // {
            //     throwDuck(target, stillPeppered);
            // }
        }
    }

    //bait
    private void attractedToBait()
    {
        Vector3 baitDirection = targetBait.transform.position - duckTransform.position;
        duckTransform.position += baitDirection.normalized * duckBaitedVelocity;

        if (duckAtBaitDistance > baitDirection.magnitude || (targetBait.GetBaitType() == BaitTypes.REPEL && duckAtRepelBaitDistance > baitDirection.magnitude))
        {
            baitSystem.removeBait(targetBait);

            Vector3 direction = mDuckRotation.findDirection();
            DuckTile furthestTile = null;
            DuckTileMap tileMap = GameManager.Instance.GetTileMap();
            int currentHeight = tileMap.getTileFromPosition(duckTransform.position).mHeight;

            if (targetBait.GetBaitType() == BaitTypes.REPEL)
            {
                //raycast to find furthest tile to move to
                //search move in opposite direction five spaces away
                for (int count = 1; count < repelDistance; count++)
                {
                    DuckTile currentTile = tileMap.getTileFromPosition(duckTransform.position + (-1 * direction * (count - .5f)));
                    if (currentTile != null && currentTile.mHeight == currentHeight && currentTile.GetDuckPassable())
                    {
                        furthestTile = currentTile;
                    }
                }

                if (furthestTile != null)
                {
                    repelTargetPos = furthestTile.mPosition + new Vector3(0, aboveTileHeight, 0);
                    mDuckRotation.rotateDuck(repelTargetPos - duckTransform.position);
                    ChangeDuckState(DuckStates.BAIT_REPEL);
                }
                else
                {
                    lookForBait();
                }
            }
            else if (targetBait.GetBaitType() == BaitTypes.PEPPER)
            {
                //reverse raycast to find furthest tile to move to
                for (int count = pepperLaunchDistance; count > 0; count--)
                {
                    DuckTile currentTile = tileMap.getTileFromPosition(duckTransform.position + (direction * (count + .5f)));
                    if (currentTile != null && currentTile.mHeight <= currentHeight && currentTile.GetDuckPassable())
                    {
                        furthestTile = currentTile;
                        count = 0; //break out of loop
                    }
                }

                if (furthestTile != null)
                {
                    Vector3 pepperTarget = furthestTile.mPosition;
                    throwDuck(pepperTarget, true);
                }
                else
                {
                    lookForBait();
                }
            }
            else
            {
                lookForBait();
            }
        }
    }

    private void repelledBait()
    {
        Vector3 deBaitDirection = repelTargetPos - duckTransform.position;
        duckTransform.position += deBaitDirection.normalized * duckBaitedVelocity;
        if (duckAtBaitDistance > deBaitDirection.magnitude)
        {
            lookForBait();
        }
    }

    private void lookForBait()
    {
        //look fro another bait
        targetBait = baitSystem.duckFindBait(duckTransform.position, attractDistance);
        if (targetBait == null)
        {
            ChangeDuckState(DuckStates.STILL);
            GameManager.Instance.duckRecall();
        }
        else //rotate duck
        {
            ChangeDuckState(DuckStates.BAITED);
            mDuckRotation.rotateDuck(targetBait.transform.position - duckTransform.position);
        }
    }

    //move through the given path
    void movePaths()
    {
        Vector3 direction = (tilePath[tilePathIndex] + new Vector3(0, aboveTileHeight, 0) - duckTransform.position);

        DuckTile currentTile = GameManager.Instance.GetTileMap().getTileFromPosition(duckTransform.position);
        DuckTile tile = GameManager.Instance.GetTileMap().getTileFromPosition(tilePath[tilePathIndex]);

        if (tile.mHeight < currentTile.mHeight)
            duckTransform.position = Vector3.MoveTowards(duckTransform.position, new Vector3(tile.mPosition.x, aboveTileHeight + tile.mPosition.y + currentTile.mHeight, tile.mPosition.z), verticalityChangeSpeed);

        else
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
        //check if out of threshold
        if (playerDistance > followThreshold && startFollowing == false)
        {
            startFollowing = true;
            addnewPos();

        }
        else if (playerDistance < followThreshold)
        {
            if (positionCount != 0)
            {
                //reset data
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
                    addnewPos();
                }
                targetPoint = positionListData.Dequeue();
                mDuckRotation.rotateDuck((targetPoint - duckTransform.position).normalized);
                positionCount--;
            }

            //find direction and follow
            float realVelocity;
            if (playerDistance > 1)
            {
                realVelocity = followVelocity + .01f;
            }
            else
            {
                realVelocity = followVelocity;
            }
            Vector3 dir = targetPoint - duckTransform.position;
            duckTransform.position += dir.normalized * realVelocity;

            //check if approaching distance
            if (dir.magnitude < toPointDistance)
            {
                targetPoint = Vector3.zero;
                positionCount--;
            }

            updateTimer();
        }
        else
        {
            FindObjectOfType<UIManager>().SetNewState(UIState.Pickup);
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
        newPos += new Vector3(Random.Range(-targetRadius * 100, targetRadius * 100) / 100, -aboveTileHeight, Random.Range(-targetRadius * 100, targetRadius * 100) / 100);
        positionListData.Enqueue(newPos);
        positionCount++;
    }

    //send in the new path to be read and activate return
    public void applyNewPath(List<Vector3> newPath)
    {
        //turn off startle animation
        startled = false;
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
        beingPickedUp = true;
        ChangeDuckState(DuckStates.HELD);
        positionListData.Clear();
        //place duck ontop of player 
    }
    public void endPickup()
    {
        beingPickedUp = false;
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

    public void throwDuck(Vector3 target, bool peppered = false)
    {
        //animation being thrown is off
        beingThrown = false;

        if (peppered)
        {
            ChangeDuckState(DuckStates.AT_APPLEBEES);
        }
        else
        {
            ChangeDuckState(DuckStates.INAIR);
        }
        // https://www.desmos.com/calculator/eryeuvorvr

        currentTime = 0;
        Vector3 Difference = target + new Vector3(0, aboveTileHeight, 0) - duckTransform.position;
        throwDistance = new Vector3(Difference.x, 0, Difference.z).magnitude;
        direction = new Vector3(Difference.x, 0, Difference.z).normalized;
        startingPos = duckTransform.position;
        targetPos = target;

        mDuckRotation.rotateDuck(direction.normalized);
        totalTime = throwDistance * (throwTimeFudge);

        Vector2 initialPoint = Vector2.zero;
        Vector2 finalPoint = new Vector2(throwDistance, Difference.y);
        Vector2 midpoint = new Vector2(finalPoint.x / 2, finalPoint.x * .4f + (finalPoint.y + finalPoint.y * -.2f));

        float A1 = -Mathf.Pow(initialPoint.x, 2) + Mathf.Pow(midpoint.x, 2);
        float B1 = -initialPoint.x + midpoint.x;
        float D1 = -initialPoint.y + midpoint.y;

        float A2 = -Mathf.Pow(midpoint.x, 2) + Mathf.Pow(finalPoint.x, 2);
        float B2 = -midpoint.x + finalPoint.x;
        float D2 = -midpoint.y + finalPoint.y;

        float BMultiplier = -(B2 / B1);
        float A3 = (BMultiplier * A1) + A2;
        float D3 = (BMultiplier * D1) + D2;

        parabolaA = D3 / A3;
        parabolaB = (D1 - (A1 * parabolaA)) / B1;
        parabolaC = initialPoint.y - parabolaA * Mathf.Pow(initialPoint.x, 2) - parabolaB * initialPoint.x;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Geyser" && (mDuckState == DuckStates.INAIR || mDuckState == DuckStates.AT_APPLEBEES || mDuckState == DuckStates.RUN))
        {
            collision.gameObject.GetComponent<Geyser>().geyserAwake();
            bool peppered = false;
            float interval = 1; //reduces the range of the launch distance
            if (mDuckState == DuckStates.AT_APPLEBEES)
            {
                peppered = true;
            }

            Vector3 target = Vector3.zero;

            DuckTileMap tileMap = GameManager.Instance.GetTileMap();

            int currentHeight = tileMap.getTileFromPosition(duckTransform.position).mHeight;
            Vector3 direction = targetPos - startingPos;
            for (float count = direction.magnitude; count > 0; count--)
            {
                DuckTile tile = tileMap.getTileFromPosition(duckTransform.position + ((interval * count) * direction.normalized));
                if (tile != null && (tile.mHeight == currentHeight || tile.mType == DuckTile.TileType.PassableBoth || tile.mType == DuckTile.TileType.UnpassableMaster))
                {
                    count = 0;
                    target = tile.mPosition;
                }
            }

            throwDuck(target, peppered);
        }
    }
}
