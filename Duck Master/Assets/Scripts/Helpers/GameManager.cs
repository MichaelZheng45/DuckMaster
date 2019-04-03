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
        altimeterSys = altimeter.GetComponent<Altimeter>();
        playerTransform = player.transform;
        duckTransform = duck.transform;

		tileMap = tileMapScriptableObject.tileMap;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public bool checkIsHoldingDuck()
    {
        return playerActionSys.isHoldingDuck;
    }

    public void mouseHitOnTile(RaycastHit hit, bool rightClick)
    {
        //temporary clicking movement
        if (rightClick)
        {
            List<Vector3> tilePath = Pathfinder.getTilePathPlayer(playerTransform.position, hit.transform.position,tileMap);
            if (tilePath.Count > 0)
            {
                playerActionSys.applyNewPath(tilePath);
            }
        }

    }

    public void clickOnDuck()
    {
        //check if the duck is nearby, if not nothing happens
        if ((duckTransform.position - playerTransform.position).magnitude < 1.5 && duckBehaviourSys.mDuckState != DuckStates.HELD)
        {
            playerActionSys.isHoldingDuck = true;
            duckBehaviourSys.pickUpDuck();
        }
        else
        {
            duckRecall(); //if the duck is too far away recall it
        }
    }

    public void throwDuck(RaycastHit hit)
    {
        //layer mask
        int unthrowMask = 1 << 11;
        RaycastHit athit;
        Vector3 dir = hit.point - duckTransform.position;
		//check if anything is in the way (need to be changed)
        if (!Physics.Raycast(duckTransform.position, dir.normalized, out athit, dir.magnitude, unthrowMask) && dir.magnitude < throwDistanceMax)
        {
            DuckTile atTile = tileMap.mHeightMap.GetTile(Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.z));

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
		DuckTile atTile = tileMap.mHeightMap.GetTile(Mathf.FloorToInt(atPos.x), Mathf.FloorToInt(atPos.z));
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
				DuckTile atTile = tileMap.mHeightMap.GetTile(Mathf.FloorToInt(duckTransform.position.x), Mathf.FloorToInt(duckTransform.position.z));

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
		unFriendlyList.Add(newUnfreindly);
	}

	
	public void markUnfreindlies(ref List<DuckTile> previousAOE, Vector3 pos, int range)
	{
		//turn all of those back to the
	}
}
