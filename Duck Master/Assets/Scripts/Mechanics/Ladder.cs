using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : LogicOutput
{
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] bool isActive = true;
    GameObject target;
    GameObject player;
    PlayerAction action;
    Ladder otherLadder;
    bool isPlayerUsing;
    bool isChild;


    // Start is called before the first frame update
    void Start()
    {

        if (gameObject.name == "Ladder-Top")
        {
            target = transform.Find("Ladder-End").gameObject;
            otherLadder = target.GetComponent<Ladder>();
            isChild = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerUsing)
        {
            if (!action.CheckMoving())
                player.transform.position = Vector3.MoveTowards(player.transform.position, target.transform.position, moveSpeed);

            if (player.transform.position == target.transform.position)
            {
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
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Duck")
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

                    if (!action.isHoldingDuck)
                    {
                        player = playerObj;
                        isPlayerUsing = true;
                    }

                    else
                        action = null;
                    
                }
            }

            else
                print("Use Ladder Error: Player was not passed in");
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
