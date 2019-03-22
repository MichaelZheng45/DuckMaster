using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
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
        if (isActive)
        {
            if (other.gameObject.tag == "Player" && !otherLadder.GetUsing())
            {
                action = other.gameObject.GetComponent<PlayerAction>();

                if (!action.isHoldingDuck)
                {
                    isPlayerUsing = true;
                    player = other.gameObject;
                }
            }
        }

        else
        {
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "Duck")
            {
                isActive = true;
                target.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (action != null)
            {
                if (action.CheckMoving())
                {
                    isPlayerUsing = false;
                    player = null;
                    action = null;
                }
            }
        }
    }

    public bool GetUsing()
    {
        return isPlayerUsing;
    }
}
