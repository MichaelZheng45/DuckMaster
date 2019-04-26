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

        GenerateTileMap(mTileMapScriptableObject.verticalLevels,
                mTileMapScriptableObject.listGridSelStrings,
                mTileMapScriptableObject.blockTypes,
                mTileMapScriptableObject.levelHeights,
                mTileMapScriptableObject.levelWidths);

        bait = player.GetComponentInChildren<BaitSystem>();
        bait.setBaitAmounts(mTileMapScriptableObject.attractQuantity, mTileMapScriptableObject.repelQuantity, mTileMapScriptableObject.pepperQuantity);

        if (unFriendlyList == null)
        {
            unFriendlyList = new List<unfreindlyScript>();
        }

        playerActionSys = player.GetComponent<PlayerAction>();
        throwDistanceMax = playerActionSys.getThrowDistance();
        duckBehaviourSys = duck.GetComponent<duckBehaviour>();
        playerTransform = player.transform;
        duckTransform = duck.transform;
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

    //transforms
    private Transform playerTransform;
    private Transform duckTransform;

    //throw and recall check data
    private float throwDistanceMax;
    bool isThrowing = false;
    bool isRecalling = false;
    bool isPickingUp = false;
    float currentTimer;
    Vector3 throwTilePosition;
    List<Vector3> duckTilePath;

    //lists
    //TO DO: find a way to populate this list with unfriendlies for each level
    private List<unfreindlyScript> unFriendlyList = null;

    [SerializeField]
    private TileMapScriptableObject mTileMapScriptableObject = null;
    private DuckTileMap mTileMap;
    BaitSystem bait;

    //GameObject uiManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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

    //float timerThrowWait = .5f; //<--Lag Time to throw duck
    //float timerRecallWait = .2f; //<---Lag Time to recall duck

    public void masterThrow()
    {
        isThrowing = false;
        duckBehaviourSys.throwDuck(throwTilePosition);
        FindObjectOfType<UIManager>().SetNewState(UIState.Whistle);
    }

    public void masterRecall()
    {
        isRecalling = false;
        duckRecall();
        duckBehaviourSys.applyNewPath(duckTilePath);
    }

    public void duckPickupEnd()
    {
        duckBehaviourSys.endPickup();
        FindObjectOfType<UIManager>().SetNewState(UIState.Holding);
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
            if (atTile.GetDuckPassable())
            {
                //throw duck
                playerActionSys.isHoldingDuck = false;
                isThrowing = true;
                duckBehaviourSys.prepThrow();
                Vector3 Difference = atTile.mPosition - playerTransform.position;
                playerTransform.transform.forward = new Vector3(Difference.x, 0, Difference.z).normalized;

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
                duckTilePath = tilePath;
                isRecalling = true;
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

    //Will: For Running Water
    public GameObject GetMainCamera()
    {
        return cameraMain;
    }

    public List<unfreindlyScript> getScareDuckList()
    {
        return unFriendlyList;
    }
    public DuckTileMap GetTileMap()
    {
        return mTileMap;
    }

    public Vector3 checkToRun(float range)
    {
        foreach (unfreindlyScript enemy in unFriendlyList)
        {

            Vector3 dist = new Vector3(duckTransform.position.x - enemy.getUnitTransform().position.x, 0, duckTransform.position.z - enemy.getUnitTransform().position.z);
            int currentHeight = mTileMap.getTileFromPosition(duckTransform.position).mHeight;
            if (dist.magnitude < range)
            {
                playerActionSys.isHoldingDuck = false;
                //find a tile to move to
                //DuckTile atTile = mTileMap.getTileFromPosition(duckTransform.position);
                float runRange = 10;
                float interval = .33f;
                for (float count = 2; count < runRange; count += interval)
                {
                    DuckTile atTile = mTileMap.getTileFromPosition(duckTransform.position + (dist.normalized * count));
                    if (atTile.GetDuckPassable() && atTile.mHeight <= currentHeight)
                    {
                        return atTile.mPosition;
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
        else
        {
            unFriendlyList = new List<unfreindlyScript>();
            unFriendlyList.Add(newUnfreindly);
        }
    }

    public void markUnfreindlies(ref List<DuckTile> previousAOE, Vector3 pos, int range)
    {
        //turn all of those back to the
    }

    public BaitSystem GetBait()
    {
        return bait;
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

                    // Add specific tile outputs here
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
