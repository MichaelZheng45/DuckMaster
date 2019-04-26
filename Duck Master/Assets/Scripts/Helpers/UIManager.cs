using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum UIState { Whistle, Recalling, Pickup, PickingUp, Holding, Throwing }

public class UIManager : MonoBehaviour
{


    [SerializeField]
    private GameObject primaryButton;


    [SerializeField]
    private GameObject BaitButton;


    [SerializeField]
    Animator BaitPanel;
    [SerializeField]
    private GameObject attractButton;
    [SerializeField]
    private GameObject repelButton;
    [SerializeField]
    private GameObject pepperButton;

    //private Text primaryButtonText;
    //private Text throwButtonText;

    [SerializeField]
    Sprite PickUpUnPushTex;
    [SerializeField]
    Sprite PickUpPushTex;
    [SerializeField]
    Sprite throwUnPushTex;
    [SerializeField]
    Sprite throwPushTex;
    [SerializeField]
    Sprite whistleUnPushTex;
    [SerializeField]
    Sprite whistlePushTex;
    [SerializeField]
    Material underGateMat;
    [SerializeField]
    Material waterMat;

    private Text attractText;
    private Text repelText;
    private Text pepperText;

    BaitSystem bait;
    BaitTypes currentType;

    public UIState currentState { get; set; }
    bool baiting;

    [SerializeField]
    private double magnitude = 1.5;
    private bool baitToggle;
    private const string WHISTLE = "WHISTLE";
    private const string PICKUP = "PICK UP";
    private const string NONE = "   ";
    private string currentPrimaryState = "WHISTLE";
    private bool duckRecalled;

    List<GameObject> highlightedThrowTiles;
    static List<GameObject> underGateTiles;
    Image primaryButtonImage;

    Transform duck;
    Transform player;
    Vector3 lastPlayerPos;

    // Start is called before the first frame update
    void Start()
    {
        currentState = UIState.Pickup;
        BaitButton.SetActive(!(GameManager.Instance.GetBait().GetBaitAmount(BaitTypes.ATTRACT) == 0 && GameManager.Instance.GetBait().GetBaitAmount(BaitTypes.REPEL) == 0 && GameManager.Instance.GetBait().GetBaitAmount(BaitTypes.PEPPER) == 0));
        highlightedThrowTiles = new List<GameObject>();
        underGateTiles = new List<GameObject>();
        duckRecalled = false;

        currentType = BaitTypes.INVALID;
        baitToggle = false;


        attractText = attractButton.GetComponentInChildren<Text>();
        repelText = repelButton.GetComponentInChildren<Text>();
        pepperText = pepperButton.GetComponentInChildren<Text>();

        bait = GameManager.Instance.GetBait();
        primaryButtonImage = primaryButton.GetComponent<Image>();

        player = GameManager.Instance.getPlayerTrans();
        duck = GameManager.Instance.getduckTrans();
        ContextButtonGraphics();
        UpdateBaitButtons();
    }

    public void SetNewState(UIState newState)
    {
        primaryButton.GetComponent<Button>().interactable = true;
        currentState = newState;
        ContextButtonGraphics();
    }

    public void ContextButton()
    {
        switch (currentState)
        {
            case UIState.Pickup:
                SetNewState(UIState.PickingUp);
                GameManager.Instance.pickUpDuck();
                break;

            case UIState.Holding:
                SetNewState(UIState.Throwing);
                break;

            case UIState.Whistle:
                AnimationEventStuff.Whistle();
                SetNewState(UIState.Recalling);
                break;
            case UIState.Throwing:
                SetNewState(UIState.Holding);
                break;

            default:
                break;
        }
    }

    void ContextButtonGraphics()
    {
        switch (currentState)
        {
            case UIState.Whistle:
                UnHighlightTiles();
                primaryButtonImage.sprite = whistleUnPushTex;
                break;

            case UIState.Recalling:
                //primaryButton.GetComponent<Button>().interactable = false;
                primaryButtonImage.sprite = whistlePushTex;
                break;

            case UIState.Pickup:
                primaryButtonImage.sprite = PickUpUnPushTex;
                break;

            case UIState.PickingUp:
                //primaryButton.GetComponent<Button>().interactable = false;
                primaryButtonImage.sprite = PickUpPushTex;
                break;
            case UIState.Holding:
                UnHighlightTiles();
                primaryButtonImage.sprite = throwUnPushTex;
                break;

            case UIState.Throwing:
                HighlightThrowTiles();
                primaryButtonImage.sprite = throwPushTex;
                break;


        }
    }

    void UpdateBaitButtons()
    {
        int num;

        num = GameManager.Instance.GetBait().GetBaitAmount(BaitTypes.ATTRACT);
        if (num > 0)
        {
            attractButton.SetActive(true);
            attractText.text = num.ToString();
        }
        else
        {
            attractButton.SetActive(false);
        }

        num = GameManager.Instance.GetBait().GetBaitAmount(BaitTypes.REPEL);
        if (num > 0)
        {
            repelButton.SetActive(true);
            repelText.text = num.ToString();
        }
        else
        {
            repelButton.SetActive(false);
        }

        num = GameManager.Instance.GetBait().GetBaitAmount(BaitTypes.PEPPER);
        if (num > 0)
        {
            pepperButton.SetActive(true);
            pepperText.text = num.ToString();
        }
        else
        {
            pepperButton.SetActive(false);
        }
    }

    public void BaitToggle()
    {
        baitToggle = !baitToggle;
        SetBaitType("INVALID");
        UpdateBaitButtons();

        BaitPanel.SetBool("Active", baitToggle);
    }

    // Update is called once per frame
    void Update()
    {
        InputManager manager = GameManager.Instance.GetComponent<InputManager>();

        if (manager == null)
        {
            return;
        }

        //Duck Rotate
        List<RaycastHit> hitList = manager.GetTapHits();

        foreach (RaycastHit hit in hitList)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == "Duck" || hit.collider.gameObject.tag == "Duck")
                {
                    DuckRotation duckRot = GameManager.Instance.getduckTrans().gameObject.GetComponent<DuckRotation>();

                    if (duckRot.currentRotation < DuckRotationState.DOWN)
                        duckRot.rotateDuckToDirection(duckRot.currentRotation + 1);
                    else
                    {
                        duckRot.currentRotation = 0;
                        duckRot.rotateDuckToDirection(duckRot.currentRotation);
                    }

                    break;
                }
            }
        }

        //If throw enabled tap on tile to throw
        if (currentState == UIState.Throwing)
        {
            if(player.position != lastPlayerPos)
            {
                UnHighlightTiles();
                HighlightThrowTiles();
            }
            foreach (RaycastHit hit in hitList)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.name == "ground(Clone)" || hit.collider.gameObject.name == "water(Clone)")
                    {
                        if (highlightedThrowTiles.Contains(hit.collider.gameObject))
                        {
                            GameManager.Instance.enableThrowDuck(hit);
                            SetNewState(UIState.Whistle);
                        }
                        else
                        {
                            SetNewState(UIState.Holding);
                        }

                    }
                }
            }
            lastPlayerPos = player.position;
            return;
        }
        //Bait Logic Input
        else if (baitToggle)
        {
            foreach (RaycastHit hit in hitList)
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.name == "ground(Clone)" || hit.collider.gameObject.name == "water(clone)")
                    {
                        Vector3 pos = hit.collider.gameObject.transform.position;

                        if (currentType == BaitTypes.ATTRACT)
                        {
                            //bait.spawnBait(pos, BaitTypes.ATTRACT);
                            GameManager.Instance.GetBait().spawnBait(pos, BaitTypes.ATTRACT);
                        }

                        if (currentType == BaitTypes.REPEL)
                        {
                            //bait.spawnBait(pos, BaitTypes.REPEL);
                            GameManager.Instance.GetBait().spawnBait(pos, BaitTypes.REPEL);
                        }

                        if (currentType == BaitTypes.PEPPER)
                        {
                            //bait.spawnBait(pos, BaitTypes.PEPPER);
                            GameManager.Instance.GetBait().spawnBait(pos, BaitTypes.PEPPER);
                        }
                        UpdateBaitButtons();

                    }
                    else if (hit.collider.gameObject.tag == "Bait")
                    {
                        GameManager.Instance.GetBait().removeBait(hit.collider.gameObject.GetComponent<BaitTypeHolder>());
                        UpdateBaitButtons();
                    }
                }

            }
            return;
        }
        //Whistle Control
        else
        {
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

            return;
        }
    }


    public void SetBaitType(string type)
    {
        System.Enum.TryParse(type, out currentType);
        attractButton.GetComponent<Image>().color = (currentType == BaitTypes.ATTRACT) ? Color.green : Color.white;
        repelButton.GetComponent<Image>().color = (currentType == BaitTypes.REPEL) ? Color.green : Color.white;
        pepperButton.GetComponent<Image>().color = (currentType == BaitTypes.PEPPER) ? Color.green : Color.white;
    }

    public void HighlightThrowTiles()
    {
        DuckTileMap tileMap = GameManager.Instance.GetTileMap();
        float MaxThrow = GameManager.Instance.getPlayerTrans().GetComponent<PlayerAction>().getThrowDistance();

        RaycastHit[] hitList = Physics.SphereCastAll(GameManager.Instance.getPlayerTrans().position, MaxThrow - 1, Vector3.down, LayerMask.NameToLayer("TileMask"));

        foreach (RaycastHit hit in hitList)
        {
            if (hit.collider != null && hit.collider.gameObject.name == "ground(Clone)")
            {
                Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
                rend.material.color = Color.green;
                highlightedThrowTiles.Add(hit.collider.gameObject);
            }

            if (hit.collider != null && hit.collider.gameObject.name == "water(Clone)")
            {
                Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
                rend.material.color = Color.green;
                highlightedThrowTiles.Add(hit.collider.gameObject);
            }
        }
    }


    public void UnHighlightTiles()
    {
        if (highlightedThrowTiles.Count > 0)
        {
            foreach (GameObject obj in highlightedThrowTiles)
            {
                if (obj.name == "water(Clone)")
                {
                    Renderer rend = obj.GetComponent<Renderer>();
                    rend.material = waterMat;
                }

                else
                    obj.GetComponent<Renderer>().material.color = Color.white;
            }

            highlightedThrowTiles.Clear();
        }

        if (underGateTiles.Count > 0)
        {
            foreach (GameObject obj in underGateTiles)
            {
                obj.GetComponent<Renderer>().material = underGateMat;
            }
        }
    }

    //Need to get a reference to all tile objects to be able to reset material properly
    public static void AddUnderGateTile(GameObject tile)
    {
        underGateTiles.Add(tile);
    }
}