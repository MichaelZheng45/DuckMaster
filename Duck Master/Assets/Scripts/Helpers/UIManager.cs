using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pickUpButton;
    [SerializeField] GameObject whistleButton;
    [SerializeField] GameObject throwButton;
    [SerializeField] double magnitude = 1.5;
    bool throwToggle = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform duck = GameManager.Instance.getduckTrans();
        Transform player = GameManager.Instance.getPlayerTrans();

        //Determine whether to whistle or hold - Do the calculation here to avoid merge conflict
        if ((duck.position - player.position).magnitude < magnitude && !GameManager.Instance.checkIsHoldingDuck())
        {
            whistleButton.SetActive(false);
            pickUpButton.SetActive(true);
        }

        else
        {
            whistleButton.SetActive(true);
            pickUpButton.SetActive(false);
        }

        //Check for throw
        if (GameManager.Instance.checkIsHoldingDuck())
            throwButton.SetActive(true);
        else
            throwButton.SetActive(false);

        InputManager manager = GameManager.Instance.GetComponent<InputManager>();

        if (throwToggle)
        {
            //if tap on tile throw
            if (manager != null)
            {
                print("Input manager is not null (UI)");
                List<RaycastHit> hitList = manager.GetInput();

                foreach (RaycastHit hit in hitList)
                {
                    if (hit.collider != null)
                    {
                        print("hit gameObject name: " + hit.collider.gameObject.name);

                        if (hit.collider.gameObject.name == "ground(Clone)" || hit.collider.gameObject.name == "water(Clone)")
                            GameManager.Instance.throwDuck(hit);
                    }

                }
            }
            else
            {
                //path find call
            }
        }

    }

    public void ToggleThrow(bool toggle)
    {
        throwToggle = toggle;
    }
}
