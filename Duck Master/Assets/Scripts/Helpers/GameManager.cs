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
    [SerializeField] GameObject tileMap;
    [SerializeField] GameObject altimeter;

    //playable objects
    [SerializeField] GameObject player;
    [SerializeField] GameObject duck;

    //materials
    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;
    //script systems
    obstacleTilingSystem tilingSys;
    PlayerAction playerActionSys;
    duckBehaviour duckBehaviourSys;
    Altimeter altimeterSys;
    //transforms
    Transform playerTransform;
    Transform duckTransform;

    //renderComponents
    MeshRenderer highlighterRenderer;

    //other Data
    [SerializeField] float throwDistanceMax;
    //lists
    List<unfreindlyScript> unFriendlyList; // <<------ GOOOSE?!?!?!?

    //bool checks
    bool holdingDuck = false;

    // Start is called before the first frame update
    void Start()
    {
        unFriendlyList = new List<unfreindlyScript>();
        tilingSys = tileMap.GetComponent<obstacleTilingSystem>();
        playerActionSys = player.GetComponent<PlayerAction>();
        duckBehaviourSys = duck.GetComponent<duckBehaviour>();
        altimeterSys = altimeter.GetComponent<Altimeter>();
        playerTransform = player.transform;
        duckTransform = duck.transform;
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
        tilingSys.checkToTile(hit, highlighter);

        //temporary clicking movement
        if (rightClick)
        {
            List<Vector3> tilePath = tilingSys.getTilePathPlayer(playerTransform.position, hit.transform.position,
                playerActionSys.getTraverseData().traversePossibilities);
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
        if (!Physics.Raycast(duckTransform.position, dir.normalized, out athit, dir.magnitude, unthrowMask) && dir.magnitude < throwDistanceMax)
        {
            tile atTile = tilingSys.getToTile(hit);

            //check if throwable
            if (duckBehaviourSys.traverseData.traversePossibilities[(int)atTile.tType] && atTile.walkable)
            {
                //throw duck
                playerActionSys.isHoldingDuck = false;
                duckBehaviourSys.throwDuck(atTile.pos);
            }
        }
    }

    public void highLightOnOff(bool on)
    {

    }

    //create pathfinding to return duck back to the player
    void duckRecall()
    {
        //do pathfinding
        if (duckBehaviourSys.isRecallable())
        {
            List<Vector3> tilePath = tilingSys.getTilePathDuck(duckTransform.position, playerTransform.position,
                         duckBehaviourSys.traverseData.traversePossibilities);
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

    //Will: Used for altimeter
    public obstacleTilingSystem GetTilingSystem()
    {
        return tilingSys;
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
        tile atTile = tilingSys.getToTileByPosition(atPos);
        float startingRange = .8f;
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

        //is not a geyser
        return Vector3.zero;
    }

    public void buttonActivated()
    {
        tilingSys.changeAllFromButtons();
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
                tile atTile = tilingSys.getToTileByPosition(duckTransform.position);

                for (int row = -1; row < 2; row++)
                {
                    for (int col = -1; col < 2; col++)
                    {
                        tile adjTile = tilingSys.getTilebyIndex((int)atTile.index2.x + col, (int)atTile.index2.y + row);
                        if (duckBehaviourSys.traverseData.traversePossibilities[(int)adjTile.tType] && adjTile.walkable && !(col == 0 && row == 0))
                        {
                            return adjTile.pos + new Vector3(0, 1, 0);
                        }
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

    public void markUnfreindlies(ref List<tile> unTouchable, Vector3 pos, int range)
    {
        unTouchable = tilingSys.turnWalkable(unTouchable, pos, range);
    }
}
