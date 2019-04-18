using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject primaryButton;
    [SerializeField]
    private GameObject throwButton;
    [SerializeField]
    private GameObject primaryBaitButton;
    [SerializeField]
    private GameObject attractButton;
    [SerializeField]
    private GameObject repelButton;
    [SerializeField]
    private GameObject pepperButton;

    private Text primaryButtonText;
    private Text throwButtonText;

    private Text attractText;
    private Text repelText;
    private Text pepperText;

    BaitSystem bait;
    BaitTypes currentType;

    [SerializeField]
    private double magnitude = 1.5;
    private bool throwToggle;
    private bool baitToggle;
    private const string WHISTLE = "WHISTLE";
    private const string PICKUP = "PICK UP";
    private const string NONE = "   ";


    // Start is called before the first frame update
    void Start()
    {
        currentType = BaitTypes.INVALID;
        primaryButtonText = primaryButton.GetComponentInChildren<Text>();
        throwButtonText = throwButton.GetComponentInChildren<Text>();
        attractText = attractButton.GetComponentInChildren<Text>();
        repelText = repelButton.GetComponentInChildren<Text>();
        pepperText = pepperButton.GetComponentInChildren<Text>();

        throwToggle = false;
        baitToggle = false;
        bait = GameManager.Instance.getPlayerTrans().GetComponentInChildren<BaitSystem>();
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
        }

        else
        {
            throwButton.GetComponent<RawImage>().color = Color.black;
            throwButtonText.color = Color.white;
        }

        if (baitToggle)
        {
            primaryBaitButton.GetComponent<RawImage>().color = Color.white;
            attractButton.SetActive(true);

            int num = bait.GetBaitAmount(BaitTypes.ATTRACT);

            attractText.text = "Attract: " + num;

            num = bait.GetBaitAmount(BaitTypes.REPEL);

            repelButton.SetActive(true);
            repelText.text = "Repel: " + num;

            num = bait.GetBaitAmount(BaitTypes.PEPPER);

            pepperButton.SetActive(true);
            pepperText.text = "Pepper: " + num;
        }

        else
        {
            primaryBaitButton.GetComponent<RawImage>().color = Color.black;
            attractButton.SetActive(false);
            repelButton.SetActive(false);
            pepperButton.SetActive(false);
        }

        if (currentType == BaitTypes.ATTRACT)
        {
            attractButton.GetComponent<RawImage>().color = Color.white;
            attractText.color = Color.black;

            repelButton.GetComponent<RawImage>().color = Color.black;
            repelText.color = Color.white;

            pepperButton.GetComponent<RawImage>().color = Color.black;
            pepperText.color = Color.white;
        }

        if (currentType == BaitTypes.REPEL)
        {
            attractButton.GetComponent<RawImage>().color = Color.black;
            attractText.color = Color.white;

            repelButton.GetComponent<RawImage>().color = Color.white;
            repelText.color = Color.black;

            pepperButton.GetComponent<RawImage>().color = Color.black;
            pepperText.color = Color.white;
        }

        if (currentType == BaitTypes.PEPPER)
        {
            attractButton.GetComponent<RawImage>().color = Color.black;
            attractText.color = Color.white;

            repelButton.GetComponent<RawImage>().color = Color.black;
            repelText.color = Color.white;

            pepperButton.GetComponent<RawImage>().color = Color.white;
            pepperText.color = Color.black;
        }

        if (currentType == BaitTypes.INVALID)
        {
            attractButton.GetComponent<RawImage>().color = Color.black;
            attractText.color = Color.white;

            repelButton.GetComponent<RawImage>().color = Color.black;
            repelText.color = Color.white;

            pepperButton.GetComponent<RawImage>().color = Color.black;
            pepperText.color = Color.white;
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
                            //print("valid throw target");
                            GameManager.Instance.enableThrowDuck(hit);
                            throwToggle = false;
                            primaryButtonText.text = WHISTLE;
                        }
                    }

                }
            }
            return;
        }
        //Whistle Control
        if (!throwToggle && !baitToggle)
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
                            Vector3 pos = hit.collider.gameObject.transform.position;
                            GameManager.Instance.movePlayerTo(pos);
                        }
                    }
                }
            }
            return;
        }

        //Bait Logic Input
        if (baitToggle)
        {
            if (manager != null)
            {
                List<RaycastHit> hitList = manager.GetTapHits();

                foreach (RaycastHit hit in hitList)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.name == "ground(Clone)" || hit.collider.gameObject.name == "water(clone)")
                        {
                            Vector3 pos = hit.collider.gameObject.transform.position;
                            
                            if (currentType == BaitTypes.ATTRACT)
                            {
                                bait.spawnBait(pos, BaitTypes.ATTRACT);
                            }

                            if (currentType == BaitTypes.REPEL)
                            {
                                bait.spawnBait(pos, BaitTypes.REPEL);
                            }

                            if (currentType == BaitTypes.PEPPER)
                            {
                                bait.spawnBait(pos, BaitTypes.PEPPER);
                            }

                        }
                    }

                }
            }
            return;
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

    public void ToggleBait()
    {
        baitToggle = !baitToggle;
        currentType = BaitTypes.INVALID;
    }

    public void SetBaitType(string type)
    {
        if (type == "Attract")
            currentType = BaitTypes.ATTRACT;

        if (type == "Repel")
            currentType = BaitTypes.REPEL;

        if (type == "Pepper")
            currentType = BaitTypes.PEPPER;

        if (type == "Invalid")
            currentType = BaitTypes.INVALID;
    }

    public void BaitCall()
    {
        if (currentType == BaitTypes.ATTRACT)
        {

        }

        if (currentType == BaitTypes.REPEL)
        {

        }

        if (currentType == BaitTypes.PEPPER)
        {

        }
    }
}
