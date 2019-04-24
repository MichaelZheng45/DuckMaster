using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : LogicOutput
{
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] bool isActive = true;
    [SerializeField] bool duckActivate = true;
    GameObject target;
    GameObject player;
    PlayerAction action;
    Ladder otherLadder;
    bool isPlayerUsing;
    bool isChild;
    bool reachedIntermediateTarget;
    Vector3 intermediateTarget;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        if (gameObject.name == "Ladder-Top")
        {
            target = transform.Find("Ladder-End").gameObject;
            otherLadder = target.GetComponent<Ladder>();
            isChild = false;
            intermediateTarget = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            otherLadder.intermediateTarget = intermediateTarget;
        }

        if (gameObject.name == "Ladder-End")
        {
            target = transform.parent.gameObject;
            otherLadder = target.GetComponent<Ladder>();
            isChild = true;
        }

        //For starting rolled up
        if (!isChild && !isActive)
            target.SetActive(false);
        
        isPlayerUsing = false;
        reachedIntermediateTarget = false;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

        if (isPlayerUsing)
        {
            if (!action.CheckMoving())
            {
                if (!reachedIntermediateTarget)
                    player.transform.position = Vector3.MoveTowards(player.transform.position, intermediateTarget, moveSpeed);
                else
                    player.transform.position = Vector3.MoveTowards(player.transform.position, target.transform.position, moveSpeed);   
            }

            if (player.transform.position == intermediateTarget)
                reachedIntermediateTarget = true;

            if (player.transform.position == target.transform.position)
            {
                reachedIntermediateTarget = false;
                isPlayerUsing = false;
                player = null;
                action = null;
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Test Logic
        if (isActive)
            UseLadder(other.gameObject);
        else
        {
            //if (other.gameObject.tag == "Player" || other.gameObject.tag == "Duck")
            if (other.gameObject.tag == "Duck" && duckActivate)
            {
                SetLadder(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Test Logic
        if (other.gameObject.tag == "Player")
        {
            if (action != null)
            {
                if (action.CheckMoving())
                {
                    StopUse();
                }
            }
        }
    }

    public bool GetUsing()
    {
        return isPlayerUsing;
    }

    public override void Activate(bool active)
    {
        SetLadder(active);
    }

    public void SetLadder(bool active)
    {
        if (!isChild)
        {
            isActive = active;
            target.SetActive(active);
        }
    }
    //Make sure to check Player has stopped pathing when using this function specifically. Use PlayerAction.CheckMoving()  
    public void UseLadder(GameObject playerObj)
    {
        if (isActive)
        {
            if (playerObj.tag == "Player")
            {
                if (!otherLadder.GetUsing())
                {
                    action = playerObj.GetComponent<PlayerAction>();
                    duckBehaviour duck = GameManager.Instance.getduckTrans().gameObject.GetComponent<duckBehaviour>();

                    if (!action.isHoldingDuck && duck.mDuckState == DuckStates.STILL)
                    {
                        //Maybe we can use it but for now - since this screws with duck animations
                        //if (duck.mDuckState == DuckStates.FOLLOW)
                        //    duck.mDuckState = DuckStates.STILL;

                        player = playerObj;
                        isPlayerUsing = true;
                    }

                    else
                        action = null;
                    
                }
            }
        }
    }

    //This is a general safety measure depending on where the logic will be
    public void StopUse()
    {
        isPlayerUsing = false;
        player = null;
        action = null;
    }
}
