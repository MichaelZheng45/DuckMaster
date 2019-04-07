using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject primaryButton;
    Text primaryButtonText;
    Text throwButtonText;
    [SerializeField] GameObject throwButton;
    [SerializeField] double magnitude = 1.5;
    bool throwToggle;
    const string WHISTLE = "WHISTLE";
    const string PICKUP = "PICK UP";
    const string NONE = "   ";

    // Start is called before the first frame update
    void Start()
    {
        primaryButtonText = primaryButton.GetComponentInChildren<Text>();
        throwButtonText = throwButton.GetComponentInChildren<Text>();
        throwToggle = false;
    }

    // Update is called once per frame
    void Update()
    { 
        Transform duck = GameManager.Instance.getduckTrans();
        Transform player = GameManager.Instance.getPlayerTrans();

        //Graphical stuff
        if (throwToggle)
        {
            throwButton.GetComponent<RawImage>().color = Color.white;
            throwButtonText.color = Color.black;   
            //ColorBlock block = throwButton.GetComponent<Button>().colors;
            //block.normalColor = Color.green;
            //block.highlightedColor = new Color(100, 255, 100, 255);
            //throwButton.GetComponent<Button>().colors = block;
        }

        else
        {
            throwButton.GetComponent<RawImage>().color = Color.black;
            throwButtonText.color = Color.white;
            //ColorBlock block = throwButton.GetComponent<Button>().colors;
            //block.normalColor = Color.white;
            //block.highlightedColor = Color.white;
            //throwButton.GetComponent<Button>().colors = block;
        }

        //Determine whether to whistle or hold - Do the calculation here to avoid merge conflict
        //Also check for throw
        if (!GameManager.Instance.checkIsHoldingDuck())
        {
            if ((duck.position - player.position).magnitude < magnitude && !GameManager.Instance.checkIsHoldingDuck())
                primaryButtonText.text = PICKUP;

            else
                primaryButtonText.text = WHISTLE;

            throwButton.SetActive(false);
        }

        else
            throwButton.SetActive(true);
       

        InputManager manager = GameManager.Instance.GetComponent<InputManager>();

        //If throw enabled tap on tile to throw
        if (throwToggle)
        {
            if (manager != null)
            {
                List<RaycastHit> hitList = manager.GetTapHits();

                foreach (RaycastHit hit in hitList)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.name == "ground(Clone)" || hit.collider.gameObject.name == "water(Clone)")
                        {
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
            //path find call
            if (manager != null)
            {
                List<RaycastHit> hitList = manager.GetTapHits();

                foreach (RaycastHit hit in hitList)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.name == "ground(Clone)")
                        {
                            print("hit ground");
                            Vector3 pos = hit.collider.gameObject.transform.position;
                            //Debug.Log(pos);
                            GameManager.Instance.movePlayerTo(pos);
                            /*
                            DuckTile tile = GameManager.Instance.getTileFromPosition(pos);
                            
                            if (tile != null)
                            {
                                print("Received tile pos is " + tile.mPosition.ToString());
                                GameManager.Instance.movePlayerTo(tile.mPosition);
                            }
                            */
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
            GameManager.Instance.duckRecall();

    }
}
