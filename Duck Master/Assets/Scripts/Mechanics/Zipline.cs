using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zipline : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.5f;
    GameObject target;
    GameObject player;
    PlayerAction action;
    bool isPlayerUsing;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerUsing = false;
        target = transform.Find("Zipline-End").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerUsing)
        {
            //Wait to make sure player has stopped pathing
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
        //if (other.gameObject.tag == "Player")
        //{
        //    action = other.gameObject.GetComponent<PlayerAction>();
        //
        //    if (!action.isHoldingDuck)
        //    {
        //        isPlayerUsing = true;
        //        player = other.gameObject;
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        //Will: If the player passes over a zipline, make sure to not use zipline if not stopping on it 
        //if (other.gameObject.tag == "Player")
        //{
        //    if (action.CheckMoving())
        //    {
        //        isPlayerUsing = false;
        //        player = null;
        //        action = null;
        //    }
        //}
    }

    public void UseZipline(GameObject playerObj)
    {
        if (playerObj.tag == "Player")
        {
            PlayerAction action = playerObj.GetComponent<PlayerAction>();

            if (!action.isHoldingDuck)
            {
                isPlayerUsing = true;
                player = playerObj;
            }
        }

        else
        {
            print("Use Zipline Error: Player not passed");
        }
    }
}
