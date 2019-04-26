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

    UIState currentState = UIState.Pickup;
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

    // Start is called before the first frame update
    void Start()
    {
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
            return;

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
            return;
        }

        //Bait Logic Input
        else if (baitToggle)
        {
            if (manager != null)
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

    void RotateDuck()
    {

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

/*
Deadliest Catch is a documentary television series produced by Original Productions for the Discovery Channel.It portrays the real life events aboard fishing vessels in the Bering Sea during the Alaskan king crab, opilio crab and bairdi crab fishing seasons.
The Aleutian Islands port of Dutch Harbor, Alaska, is the base of operations for the fishing fleet.The show's title derives from the inherent high risk of injury or death associated with the work.
Deadliest Catch premiered on the Discovery Channel on April 12, 2005, and currently airs worldwide. The first season consisted of ten episodes, with the finale airing on June 14, 2005. Subsequent seasons have aired on the same April to June or July schedule every year since the original 2005 season.The 14th season premiered on April 10, 2018.[1] On March 7, 2019, Discovery Channel announced the series will return for a fifteenth season to premiere on Tuesday, April 9, 2019. 

The series follows life on "the vast Bering Sea" aboard various crab fishing boats during two of the crab fishing seasons, the October king crab season and the January opilio crab (C. opilio; often referred to as "snow crab" or "opies") season. The show emphasizes the dangers on deck to the fishermen (and the Discovery Channel camera crews recording their work) as crews duck heavy crab pots swinging into position, maneuver hundreds of pounds of crab across a deck strewn with hazards (i. e., holding tank hatches, uneven surfaces, maintenance access plates, wet decks), and lean over the rails to position pots for launch or retrieval as galeforce winds and high waves constantly lash the deck. The series also documents the dangers of being on a boat in the Bering Sea, in the midst of some of the coldest and stormiest waters on earth, where even a minor problem may become complex or even catastrophic with the nearest port often hundreds of miles away.
Each episode focuses on a story, situation or theme that occurs on one or more boats, while side stories delve into the backgrounds and activities of one or two crew members, particularly the "greenhorns" (rookie crew members) on several boats. The fleet's captains are featured prominently, highlighting their camaraderie with their fellow captains and relationships with their crews, as well as their competition with other boats in the hunt for crab. Common themes include friendly rivalries among the captains (particularly between Sig Hansen of the Northwestern, and Johnathan and Andy Hillstrand of the Time Bandit), the familial ties throughout the fleet (brothers Sig, Norm, and Edgar Hansen, who own the Northwestern; the Hillstrand brothers and Johnathan's son Scott on the Time Bandit; brothers Keith and Monte Colburn of the Wizard), the stresses of life on the Bering Sea, and the high burnout rate among greenhorns.
Because Alaskan crab fishing is one of the most dangerous jobs in the world, the U.S. Coast Guard rescue squads stationed at Integrated Support Command Kodiak (Kodiak, Alaska) and their outpost on St. Paul Island, near the northern end of the crab fishing grounds, are frequently shown rescuing crab boat crew members who fall victim to the harsh conditions on the Bering Sea. The USCG rescue squad was featured prominently during the episodes surrounding the loss of F/V Big Valley in January 2005, the loss of F/V Ocean Challenger in October 2006, and the loss of F/V Katmai in October 2008. Original Productions keeps a camera crew stationed with the Coast Guard during the filming of the show. 

The show has no on-camera host. A narrator provides commentary connecting the storylines as the show shifts from one boat to another. Discovery Channel voice artist Mike Rowe narrates the action for North American airings; UK voice artist Bill Petrie, reading from a slightly altered script, offers a regionally familiar accent for a British audience, and Nasir Bilal Khan provides the voice for the episodes aired in Malaysia. The show transitions between boats using a mock-up radar screen that shows the positions of the boats relative to one another and to the two ends of the fishing grounds, St. Paul Island to the north and Dutch Harbor to the south.
Rowe was originally supposed to be the on-camera host as well and he appeared in taped footage as himself during the first season of shooting. As filming of the first season was nearing completion, Discovery greenlighted production on another Rowe project, Dirty Jobs, under the condition that Rowe choose only one show on which to appear on camera. As Rowe relates the story, Discovery told him that the two shows would be airing back-to-back on the same night, thus, "We can't have you telling us stories about six dead fishermen on camera and making a fart joke with your arm in a cow's ass."[3] Most of the footage Rowe shot during the first season became part of the first season's "Behind the Scenes" episode. After the third season of Deadliest Catch, Rowe began hosting a post-season behind-the-scenes miniseries entitled After the Catch, a roundtable discussion featuring the captains relating their experiences filming the preceding season's episodes. A season 3 episode of Dirty Jobs (2007-2008) saw Rowe return to Alaska to take part in a job tangentially related to the fishing industry — diesel fuel spill cleanup. Another episode that same season featured Rowe at work on board the F/V Legacy doing trawl fishing and at-sea shellfish and other seafood processing, during which Rowe made numerous references to the crab fishing of Deadliest Catch.  

Because Deadliest Catch is essentially a filmed record of everyday life in a stressful working environment, the producers have to censor gestures and language deemed inappropriate for television audiences. For example, under the U.S. Television Ratings system, Deadliest Catch is rated TV-14 with inappropriate language ("L") as a highlighted concern.[4] For visual disguise of such items as finger gestures, bloody injuries, or non-featured crew member anonymity, the producers use the traditional pixelization or simple blurring. However, due to the sheer volume of profanities used in the course of everyday crew member conversation, the producers occasionally employ alternate methods of censoring profanities, using sound effects such as a ship's horn, the "clang" of a hatch door, or a burst of radio static in place of the traditional "bleep".


*/
