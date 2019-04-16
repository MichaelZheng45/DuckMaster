using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    //systems objects
    [SerializeField] 
    private GameObject cameraMain;
    [SerializeField] 
    private GameObject highlighter;
    [SerializeField]
    private GameObject altimeter;

    //playable objects
    [SerializeField] 
    private GameObject player;
    [SerializeField]
    private GameObject duck;

    //script systems
    private PlayerAction playerActionSys;
    private duckBehaviour duckBehaviourSys;
    private Altimeter altimeterSys;

    //transforms
    private Transform playerTransform;
    private Transform duckTransform;

    //throw check data
    [SerializeField]
    //should be in another script to be honest
    private float throwDistanceMax;
    bool isThrowing = false;
    float timerThrowWait = .5f; //<--Lag Time to throw duck
    float currentTimer;
    Vector3 throwTilePosition;

    //lists
    //TO DO: find a way to populate this list with unfriendlies for each level
    private List<unfreindlyScript> unFriendlyList; 

    //bool checks
    private bool holdingDuck = false;

    [SerializeField]
    private TileMapScriptableObject mTileMapScriptableObject = null;
    private DuckTileMap mTileMap;

    // Start is called before the first frame update
    void Start()
    {
        unFriendlyList = new List<unfreindlyScript>();
        playerActionSys = player.GetComponent<PlayerAction>();
        duckBehaviourSys = duck.GetComponent<duckBehaviour>();
        altimeterSys = null;
        playerTransform = player.transform;
        duckTransform = duck.transform;

        //tileMap = tileMapScriptableObject.tileMap;
        GenerateTileMap(mTileMapScriptableObject.verticalLevels,
                        mTileMapScriptableObject.listGridSelStrings,
                        mTileMapScriptableObject.blockTypes,
                        mTileMapScriptableObject.levelHeights,
                        mTileMapScriptableObject.levelWidths);

	//	movePlayerTo(new Vector3(0, 0, 4));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        throwDuckPrep();
    }

    public bool checkIsHoldingDuck()
    {
        return playerActionSys.isHoldingDuck;
    }

    public void movePlayerTo(Vector3 targetPosition)
    {
        List<Vector3> tilePath = Pathfinder.getTilePathPlayer(playerTransform.position, targetPosition, mTileMap);
        if (tilePath.Count > 0)
        {
            playerActionSys.applyNewPath(tilePath);
        }
    }

    public void pickUpDuck()
    {
        //check if the duck is nearby, if not nothing happens
        if ((duckTransform.position - playerTransform.position).magnitude < 1.5 && duckBehaviourSys.mDuckState != DuckStates.HELD)
        {
            playerActionSys.isHoldingDuck = true;
            duckBehaviourSys.pickUpDuck();
        }
    }

    void throwDuckPrep()
    {
        if(isThrowing)
        {
            currentTimer += Time.deltaTime;
            if(currentTimer > timerThrowWait)
            {
                isThrowing = false;
                currentTimer = 0;
                duckBehaviourSys.throwDuck(throwTilePosition);
            }
        }
    }

    public void enableThrowDuck(RaycastHit hit)
    {
        //layer mask
        int unthrowMask = 1 << 11;
        RaycastHit athit;
        Vector3 dir = hit.point - duckTransform.position;
        Vector3 pos = hit.collider.gameObject.transform.position;
        //check if anything is in the way (need to be changed)
        if (!Physics.Raycast(duckTransform.position, dir.normalized, out athit, dir.magnitude, unthrowMask) && dir.magnitude < throwDistanceMax)
        {
			DuckTile atTile = mTileMap.getTileFromPosition(pos);

            //check if throwable
            if (atTile.mType == DuckTile.TileType.PassableBoth || atTile.mType == DuckTile.TileType.UnpassableMaster)
            {
                //throw duck
                playerActionSys.isHoldingDuck = false;
                Vector3 Difference = atTile.mPosition - playerTransform.position;
                playerTransform.transform.forward = new Vector3(Difference.x, 0, Difference.z).normalized;
                isThrowing = true;
                throwTilePosition = atTile.mPosition; 
            }
        }
    }

    //create pathfinding to return duck back to the player
    public void duckRecall()
    {
        //do pathfinding
        if (duckBehaviourSys.isRecallable())
        {
            List<Vector3> tilePath = Pathfinder.getTilePathDuck(duckTransform.position, playerTransform.position, mTileMap,
                                                                 (int)duck.GetComponent<DuckRotation>().currentRotation);
            if (tilePath.Count > 0)
            {
                //give to duck
                duckBehaviourSys.applyNewPath(tilePath);
            }
        }
    }

    public Transform getPlayerTrans()
    {
        return playerTransform;
    }

    public Transform getduckTrans()
    {
        return duckTransform;
    }

    public Altimeter GetAltimeter()
    {
        return altimeterSys;
    }

    //Will: For Running Water
    public GameObject GetMainCamera()
    {
        return cameraMain;
    }

    public DuckTileMap GetTileMap()
    {
        return mTileMap;
    }

    public Vector3 checkGeyser(Vector3 atPos, Vector3 fromPos)
    {
        DuckTile atTile = mTileMap.getTileFromPosition(atPos);
        //float startingRange = .8f;
        /*
        if (atTile.tType == tileType.Geyser)
        {
            //calculate a new trajectory
            Vector3 trajectory = (atPos - fromPos);
            List<bool> travables = duckBehaviourSys.traverseData.traversePossibilities;
            bool tileFound = false;
            while (!tileFound)
            {
                Vector3 distance = trajectory.normalized * (trajectory.magnitude * startingRange) + atPos;
                //check if trajectory is within bounds and tile
                if (!tilingSys.isInBoundsByAxis(ref distance, travables))
                {
                    startingRange -= .05f; //how specific? 
                }
                else
                {
                    //found a spot
                    return distance;
                }

                //found no tile to move
                if (startingRange < .5f)
                {
                    return Vector3.zero;
                }
            }
			
        }
		*/
        //is not a geyser
        return Vector3.zero;
    }

    public Vector3 checkToRun(float range)
    {
        foreach (unfreindlyScript enemy in unFriendlyList)
        {
            Vector3 dist = new Vector3(duckTransform.position.x - enemy.unitTransform.position.x, 0, duckTransform.position.z - enemy.unitTransform.position.z);
            if (dist.magnitude < range)
            {
                playerActionSys.isHoldingDuck = false;
                //find a tile to move to
                DuckTile atTile = mTileMap.getTileFromPosition(duckTransform.position);

                for (int row = -1; row < 2; row++)
                {
                    for (int col = -1; col < 2; col++)
                    {
                        // tile adjTile = tilingSys.getTilebyIndex((int)atTile.index2.x + col, (int)atTile.index2.y + row);
                        //   if (duckBehaviourSys.traverseData.traversePossibilities[(int)adjTile.tType] && adjTile.walkable && !(col == 0 && row == 0))
                        //   {
                        //       return adjTile.pos + new Vector3(0, 1, 0);
                        //   }
                    }
                }
                return playerTransform.position;
            }
        }

        return Vector3.zero;
    }

    public void addUnFriendly(unfreindlyScript newUnfreindly)
    {
        if (unFriendlyList != null)
        {
            unFriendlyList.Add(newUnfreindly);
        }
    }


    public void markUnfreindlies(ref List<DuckTile> previousAOE, Vector3 pos, int range)
    {
        //turn all of those back to the
    }

    private void GenerateTileMap(int verticalLevels, string[] listGridSelStrings, string[] blockTypes, int[] levelHeights, int[] levelWidths)
    {
        List<List<DuckTile.TileType>> typeGrid;
        List<DuckTile.TileType> typeList;
        List<List<bool>> baitableGrid;
        List<bool> baitableList;
        List<List<bool>> heightChangeGrid;
        List<bool> heightChangeList;
        List<Vector3> positionsList;
        List<List<Vector3>> positionGrid;
        int index = 0;

        List<DuckTileGrid> tileGrids = new List<DuckTileGrid>();

        for (int i = 0; i < verticalLevels; ++i)
        {
            // current height is i
            int height = levelHeights[i];
            int width = levelWidths[i];
            typeGrid = new List<List<DuckTile.TileType>>();
            baitableGrid = new List<List<bool>>();
            heightChangeGrid = new List<List<bool>>();
            positionGrid = new List<List<Vector3>>();

            for (int j = 0; j < height; ++j)
            {
                typeList = new List<DuckTile.TileType>();
                baitableList = new List<bool>();
                heightChangeList = new List<bool>();
                positionsList = new List<Vector3>();
                for (int k = 0; k < width; ++k)
                {
                    // TO DO: center positions, fix instantiation ways
                    string currentBlock = listGridSelStrings[index];
                    Vector3 pos = new Vector3(j, i, k);

                    // string[] blockTypes = { "Ground", "Water", "None" };
                    if (currentBlock == blockTypes[0])
                    {
                        // passable both
                        typeList.Add(DuckTile.TileType.PassableBoth);
                    }
                    else if (currentBlock == blockTypes[1])
                    {
                        // unpassable master
                        typeList.Add(DuckTile.TileType.UnpassableMaster);
                    }
                    else
                    {
                        // it's none/null aka null
                        typeList.Add(DuckTile.TileType.INVALID_TYPE);
                    }
                    baitableList.Add(false);
                    heightChangeList.Add(false);
                    positionsList.Add(pos);
                    index++;
                }
                typeGrid.Add(typeList);
                baitableGrid.Add(baitableList);
                heightChangeGrid.Add(heightChangeList);
                positionGrid.Add(positionsList);
            }
            tileGrids.Add(new DuckTileGrid(typeGrid, baitableGrid, heightChangeGrid, positionGrid, i));
        }
        mTileMap = new DuckTileMap(tileGrids);
    }
}
