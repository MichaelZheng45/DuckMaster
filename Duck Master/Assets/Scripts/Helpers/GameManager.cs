using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    //systems objects
    [SerializeField] GameObject cameraMain;
    [SerializeField] GameObject highlighter;
    [SerializeField] GameObject altimeter;

    //playable objects
    [SerializeField] GameObject player;
    [SerializeField] GameObject duck;

	//script systems
    PlayerAction playerActionSys;
    duckBehaviour duckBehaviourSys;
    Altimeter altimeterSys;
	
    //transforms
    Transform playerTransform;
    Transform duckTransform;

	//other Data
	[SerializeField] float throwDistanceMax; //should be in another script to be honest

	//lists
	List<unfreindlyScript> unFriendlyList; //TO DO: find a way to populate this list with unfriendlies for each level

    //bool checks
    bool holdingDuck = false;

	[SerializeField]
	TileMapScriptableObject tileMapScriptableObject = null;
	public DuckTileMap tileMap { get; set; }

	// Start is called before the first frame update
	void Start()
    {
        unFriendlyList = new List<unfreindlyScript>();
        playerActionSys = player.GetComponent<PlayerAction>();
        duckBehaviourSys = duck.GetComponent<duckBehaviour>();
        altimeterSys =  altimeter.GetComponent<Altimeter>();
        playerTransform = player.transform;
        duckTransform = duck.transform;

		//tileMap = tileMapScriptableObject.tileMap;
		GenerateTileMap(tileMapScriptableObject.verticalLevels,
						tileMapScriptableObject.listGridSelStrings,
						tileMapScriptableObject.blockTypes,
						tileMapScriptableObject.levelHeights,
						tileMapScriptableObject.levelWidths);

		movePlayerTo(new Vector3(4, 0, 0));
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
        List<Vector3> tilePath = Pathfinder.getTilePathPlayer(playerTransform.position, targetPosition,tileMap);
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

	public void recallDuck()
	{
		duckRecall();
	}

    public void throwDuck(RaycastHit hit)
    {
        //layer mask
        int unthrowMask = 1 << 11;
        RaycastHit athit;
        Vector3 dir = hit.point - duckTransform.position;
        Vector3 pos = hit.collider.gameObject.transform.position;
        //check if anything is in the way (need to be changed)
        if (!Physics.Raycast(duckTransform.position, dir.normalized, out athit, dir.magnitude, unthrowMask) && dir.magnitude < throwDistanceMax)
        {
			DuckTile atTile = getTileFromPosition(hit.point);

			//check if throwable
			if (atTile.mType == DuckTile.TileType.PassableBoth || atTile.mType == DuckTile.TileType.UnpassableMaster)
            {
                //throw duck
                playerActionSys.isHoldingDuck = false;
                duckBehaviourSys.throwDuck(atTile.mPosition);
            }
        }
    }

    //create pathfinding to return duck back to the player
    void duckRecall()
    {
		//do pathfinding
		if(duckBehaviourSys.isRecallable())
		{
			List<Vector3> tilePath = Pathfinder.getTilePathDuck(duckTransform.position, playerTransform.position, tileMap,
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

    public Vector3 checkGeyser(Vector3 atPos, Vector3 fromPos)
    {
		DuckTile atTile = getTileFromPosition(atPos);
		float startingRange = .8f;
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
				DuckTile atTile = getTileFromPosition(duckTransform.position);

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
		if(unFriendlyList != null)
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
					int index = i * height * width + height * j + k;
					if (index < height * width)
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
					}
				}
				typeGrid.Add(typeList);
				baitableGrid.Add(baitableList);
				heightChangeGrid.Add(heightChangeList);
				positionGrid.Add(positionsList);
			}
			tileGrids.Add(new DuckTileGrid(typeGrid, baitableGrid, heightChangeGrid, positionGrid, i));
		}
		tileMap = new DuckTileMap(tileGrids);
	}

	public DuckTile getTileFromPosition(Vector3 position)
	{
		return tileMap.mHeightMap.GetTile(Mathf.FloorToInt(position.x + .5f), Mathf.FloorToInt(position.z + .5f));
	}
}
