#if (UNITY_EDITOR || UNITY_STANDALONE)
#define DESKTOP
#elif (UNITY_IOS || UNITY_ANDROID)
#define MOBILE
#endif

// Above is a check to see what platform we are currently on
// These are used to not have pointless code run below

using UnityEngine;

public class InputManager : MonoBehaviour
{
#if DESKTOP
	// Keep track of it we have clicked, where we started the click, and if we recently let go
    bool leftClick, rightClick;
    Vector3 lastLeftClick, lastRightClick;
    Vector3 upLeftClick;
#elif MOBILE
	// Keep track of the current touches and amount
    Touch[] taps;
    int tapCount = 0;
#endif
	[Tooltip("Pixel Distance A Tap Becomes A Swipe")]
    [SerializeField]
    float swipeTolerance = 25f;
	// Enums used for generic direction
    public enum SwipeDirection
    {
        NONE = -1,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

	// Used to contain the data needed to process and keep track of swipes
    public struct SwipeData
    {
        public Vector2 startPosition;
        public Vector2 deltaPos;
        public SwipeDirection direction;
        public bool isSwiping;
    }

	const int MAX_TAPS = 5;
	// Keeping track of swipes
	SwipeData[] mSwipeData;

    // Start is called before the first frame update
    void Start()
    {
        mSwipeData = new SwipeData[MAX_TAPS];
    }

    // Update is called once per frame
    void Update()
    {
        // in editor or on pc
#if DESKTOP
        leftClick = Input.GetMouseButtonDown(0);
        rightClick = Input.GetMouseButtonDown(1);
        
		// If our mouse is down we get delta for swiping
        if (Input.GetMouseButton(0))
        {
            mSwipeData[0].deltaPos = Input.mousePosition - lastLeftClick;
        }
		// Reset when we let go
        else if (Input.GetMouseButtonUp(0))
        {
            upLeftClick = Input.mousePosition;
            mSwipeData[0].deltaPos = Vector3.zero;
        }
        if (rightClick)
        {
            lastRightClick = Input.mousePosition;
        }
        lastLeftClick = Input.mousePosition;
#elif MOBILE
		// Get the current touches and make sure we are capped at 5
        taps = Input.touches;
        tapCount = Input.touchCount;
        if(tapCount > MAX_TAPS)
        {
            tapCount = MAX_TAPS;
        }

        //  In order to detect a swipe
        //      If it has moved, how far, if I wasn't previously swiping
        //      If I am currently swiping I do what I need to do with swipe data
        //      I need to reset the data and swipe state
        //  I need to do this with every finger
        //  
        //  In order to detect a tap
        //      I need to know if my touch has ended and is not swiping
        //      
        
        if(tapCount > 0)
        {
            for (int i = 0; i < tapCount; ++i)
            {
                switch (taps[i].phase)
                {
                    case TouchPhase.Began:
                    {
                        mSwipeData[i].startPosition = taps[i].position;
                        break;
                    }
                    case TouchPhase.Moved:
                    {
						// we are currently swiping
						if(mSwipeData[i].isSwiping)
						{
							// we are swiping
							// so we need to continue swiping
							mSwipeData[i].deltaPos += taps[i].deltaPosition;
						}
						// we aren't previously swiping and we are outside the swipe tolerance
						else if(Mathf.Abs(taps[i].position.x - mSwipeData[i].startPosition.x) > swipeTolerance || Mathf.Abs(taps[i].position.y - mSwipeData[i].startPosition.y) > swipeTolerance)
						{
							// we need to start swiping
							mSwipeData[i].isSwiping = true;
							mSwipeData[i].deltaPos = Vector2.zero;
							mSwipeData[i].direction = SwipeDirection.NONE;
						}
                        break;
                    }
                    case TouchPhase.Ended:
                    {
						// we were not swiping
						if(!mSwipeData[i].isSwiping)
						{
							// we have tapped
							// probably need to have an event fire here?
							Debug.Log("We Have Tappdown");
						}
						// we were swiping
						else
						{
							// we have stopped swiping
							// probably need to have an event fire here?
							Debug.Log("We Have Swipedown");
							mSwipeData[i].isSwiping = false;
						}
                        break;
                    }
                }
            }
        }
#endif
	}

	// Generic get function that will work with both platforms
	public bool GetInput(int dist = 100, int layer = 1)
    {
#if DESKTOP
        return GetLeftMouseClickHit(dist, layer);
#elif MOBILE
        return GetSingleTapHit(dist, layer);
#endif
    }

	// This might get wonky with clicks, shouldn't worry about it as PC isn't our target right now
	// Layer is what layermask you want to collide with
	// DO NOT BIT ADJUST IT DOES IT FOR YOU
    public bool GetLeftMouseClickHit(int dist = 100, int layer = 1)
    {
#if DESKTOP
        if (!leftClick)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(lastLeftClick);
        return Physics.Raycast(ray, dist, 1 << layer);
#endif
        return false;
    }

	// This might get wonky with clicks, shouldn't worry about it as PC isn't our target right now
	// Layer is what layermask you want to collide with
	// DO NOT BIT ADJUST IT DOES IT FOR YOU
	public bool GetRightMouseClickHit(int dist = 100, int layer = 1)
    {
#if DESKTOP
        if (!rightClick)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(lastRightClick);
        return Physics.Raycast(ray, dist, 1 << layer);
#endif
        return false;
    }

	// Mobile specific single input
	// Layer is what layermask you want to collide with
	// DO NOT BIT ADJUST IT DOES IT FOR YOU
	public bool GetSingleTapHit(int dist = 100, int layer = 1)
    {
#if MOBILE
        if(tapCount == 0 || mSwipeData[0].isSwiping)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(taps[0].position);
        return Physics.Raycast(ray, dist, 1 << layer);
#endif
        return false;
    }

	// Mobile specific multiple input
	// Layer is what layermask you want to collide with
	// DO NOT BIT ADJUST IT DOES IT FOR YOU
	public bool[] GetMultipleTapHit(int dist = 100, int layer = 1)
    {
#if MOBILE
        bool[] ret = new bool[tapCount];
        if (tapCount == 0)
            return ret;
        Ray ray;
        Camera main = Camera.main;
        for(int i = 0; i < tapCount; ++i)
        {
            ray = main.ScreenPointToRay(taps[i].position);
            ret[i] = Physics.Raycast(ray, dist, 1 << layer);
        }
        return ret;
#endif
        return null;
    }

	// Get the Swipe data
	// If you want to set the deltaPos to zero send TRUE
	public SwipeData GetSwipeData(bool reset = false)
    {
		SwipeData returnData = new SwipeData
		{
			deltaPos = mSwipeData[0].deltaPos
		};
		if (reset)
        {
            mSwipeData[0].deltaPos.x = 0.0f;
            mSwipeData[0].deltaPos.y = 0.0f;
        }

        returnData.direction = FindDirection(returnData.deltaPos);

        return returnData;
    }

	// Helper function that returns the generic direction
	private SwipeDirection FindDirection(Vector2 vector)
    {
        return (vector.x > vector.y) ?
            ((vector.x >= 0) ? SwipeDirection.RIGHT : SwipeDirection.LEFT) :
            ((vector.y >= 0) ? SwipeDirection.UP : SwipeDirection.DOWN);
    }
}
