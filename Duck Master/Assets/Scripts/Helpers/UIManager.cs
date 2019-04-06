using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject primaryButton;
    Text primaryButtonText;
    [SerializeField] GameObject throwButton;
    [SerializeField] double magnitude = 1.5;
    bool throwToggle;
    const string WHISTLE = "WHISTLE";
    const string PICKUP = "PICK UP";
    const string NONE = "";

    // Start is called before the first frame update
    void Start()
    {
        primaryButtonText = primaryButton.GetComponentInChildren<Text>();
        throwToggle = false;
    }

    // Update is called once per frame
    void Update()
    {
        //print("In UIManager update");
        Transform duck = GameManager.Instance.getduckTrans();
        Transform player = GameManager.Instance.getPlayerTrans();
        //print("Past getting transforms UI");

        //Determine whether to whistle or hold - Do the calculation here to avoid merge conflict
        if ((duck.position - player.position).magnitude < magnitude && !GameManager.Instance.checkIsHoldingDuck())
            primaryButtonText.text = PICKUP;
        
        else
            primaryButtonText.text = WHISTLE;

        //print("Past checking for pick up or throw UI");

        //Check for throw
        if (GameManager.Instance.checkIsHoldingDuck())
            throwButton.SetActive(true);
        else
            throwButton.SetActive(false);

        //print("Past checking for throw toggle");

        InputManager manager = GameManager.Instance.GetComponent<InputManager>();

        if (manager == null)
            print("UI input manager is NULL!");

        //print("manager shouldn't be null UI");

        //If throw enabled tap on tile to throw
        if (throwToggle)
        {
            print("Inside throw input UI");
            //Debug.Log("Inside throw input UI debug log");
            if (manager != null)
            {
                print("Input manager is not null (UI)");
                List<RaycastHit> hitList = manager.GetTapHits();

                foreach (RaycastHit hit in hitList)
                {
                    if (hit.collider != null)
                    {
                       
                        print("hit gameObject name: " + hit.collider.gameObject.name);

                        if (hit.collider.gameObject.name == "ground(Clone)" || hit.collider.gameObject.name == "water(Clone)")
                        {
                            //hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                            print("valid throw target");
                            GameManager.Instance.throwDuck(hit);
                            throwToggle = false;
                            primaryButtonText.text = WHISTLE;
                        }
                    }

                }
            }
        }
        else
        {
            //print("Inside pathfinding check UI");
            //Debug.Log("Inside pathfinding check UI debug log");
            //path find call
            if (manager != null)
            {
                //print("Input manager is not null (UI)");
                List<RaycastHit> hitList = manager.GetTapHits();

                foreach (RaycastHit hit in hitList)
                {
                    if (hit.collider != null)
                    {
                        //print("hit gameObject name: " + hit.collider.gameObject.name);

                        if (hit.collider.gameObject.name == "ground(Clone)")
                        {
                            //print("Raycast hit point: " + hit.point.ToString());
                            Vector3 pos = hit.collider.gameObject.transform.position;
                            //print("pos is " + pos.ToString());

                            DuckTile tile = GameManager.Instance.tileMap.mHeightMap.GetTile(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
                            //hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.blue;

                            //if (tile == null)
                            //    print("tile to path is null");
                            //else
                            if (tile != null)
                            {
                                print("Received tile pos is " + tile.mPosition.ToString());
                                GameManager.Instance.movePlayerTo(tile.mPosition, true);
                            }
                                
                        }
                    }

                }
            }
        }
    }

    public void ToggleThrow()
    {
        throwToggle = !throwToggle;
    }

    //To Pick up or Whistle for Duck, this will deduce that
    public void PrimaryButtonCall()
    {
        if (primaryButtonText.text == PICKUP)
        {
            GameManager.Instance.pickUpDuck();
            primaryButtonText.text = NONE;
        }

        if (primaryButtonText.text == WHISTLE)
            GameManager.Instance.recallDuck();

    }
}
