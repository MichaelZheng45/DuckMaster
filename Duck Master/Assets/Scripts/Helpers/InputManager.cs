#if (UNITY_EDITOR || UNITY_STANDALONE)
#define DESKTOP
#elif (UNITY_IOS || UNITY_ANDROID)
#define MOBILE
#endif

// Above is a check to see what platform we are currently on
// These are used to not have pointless code run below

using UnityEngine;
using System.Collections.Generic;
using System;

public class InputManager : MonoBehaviour
{
#if DESKTOP
	// Keep track of it we have clicked, where we started the click, and if we recently let go
    bool leftClick, rightClick;
    Vector3 lastMousePos, lastLeftClickPos, lastRightClickPos;
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
        public Vector2 startPos, currentPos, deltaPos;
        public SwipeDirection direction;
        public bool isSwiping;
    }

	public const int MAX_TAPS = 5;
	// Keeping track of swipes
	SwipeData[] mSwipeData;

	public static SwipeData[] DefaultSwipeDataArray = new SwipeData[MAX_TAPS];
	public static SwipeData DefaultSwipeData = new SwipeData();

	// Start is called before the first frame update
	void Start()
    {
		mSwipeData = DefaultSwipeDataArray;
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
			lastLeftClickPos = Input.mousePosition;
			mSwipeData[0].deltaPos = Input.mousePosition - lastMousePos;
			mSwipeData[0].isSwiping = true;
		}
		// Reset when we let go
        else if (Input.GetMouseButtonUp(0))
        {
            upLeftClick = Input.mousePosition;
            mSwipeData[0].deltaPos = Vector3.zero;
			mSwipeData[0].isSwiping = false;
        }
        if (Input.GetMouseButton(1))
        {
			lastRightClickPos = Input.mousePosition;
			mSwipeData[1].deltaPos = Input.mousePosition - lastMousePos;
			mSwipeData[1].isSwiping = true;
        }
		else if(Input.GetMouseButtonUp(1))
		{
			mSwipeData[1].deltaPos = Vector3.zero;
			mSwipeData[1].isSwiping = false;
		}
		lastMousePos = Input.mousePosition;

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
                        mSwipeData[i].startPos = taps[i].position;
                        break;
                    }
                    case TouchPhase.Moved:
                    {
						mSwipeData[i].currentPos = taps[i].position;
						mSwipeData[i].deltaPos = taps[i].deltaPosition;
						// we are currently swiping
						//if(mSwipeData[i].isSwiping)
						//{
						//	// we are swiping
						//	// so we need to continue swiping
						//	
						//	
						//	//mSwipeData[i].deltaPos += taps[i].deltaPosition;
						//}
						// we aren't previously swiping and we are outside the swipe tolerance
						if(!mSwipeData[i].isSwiping && (Mathf.Abs(taps[i].position.x - mSwipeData[i].startPos.x) > swipeTolerance || Mathf.Abs(taps[i].position.y - mSwipeData[i].startPos.y) > swipeTolerance))
						{
							// we need to start swiping
							mSwipeData[i].isSwiping = true;
							mSwipeData[i].deltaPos = mSwipeData[i].currentPos = Vector2.zero;
							mSwipeData[i].direction = SwipeDirection.NONE;
							Debug.Log("START: Swipe : Index - " + i);
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
							Debug.Log("END: Tapped");
						}
						// we were swiping
						else
						{
							// we have stopped swiping
							// probably need to have an event fire here?
							Debug.Log("END: End Swipe : Index - " + i);
							//mSwipeData[i].isSwiping = false;
							//mSwipeData[i].startPos = mSwipeData[i].deltaPos = mSwipeData[i].currentPos = Vector2.zero;
							//mSwipeData[i].direction = SwipeDirection.NONE;
							for(int j = i, k = i + 1; j < MAX_TAPS; ++j, ++k)
							{
								if(k == MAX_TAPS)
								{
									mSwipeData[j] = DefaultSwipeData;
								}
								else
								{
									mSwipeData[j] = mSwipeData[k];
								}
								Debug.Log(j + " : " + k);
								
							}
						}
                        break;
                    }
					case TouchPhase.Canceled:
					{
						// we were not swiping
						if(!mSwipeData[i].isSwiping)
						{
							// we have tapped
							// probably need to have an event fire here?
							Debug.Log("CANCEL: Tapped");
						}
						// we were swiping
						else
						{
							// we have stopped swiping
							// probably need to have an event fire here?
							Debug.Log("CANCEL: End Swipe : Index - " + i);
							mSwipeData[i].isSwiping = false;
							mSwipeData[i].startPos = mSwipeData[i].deltaPos = mSwipeData[i].currentPos = Vector2.zero;
							mSwipeData[i].direction = SwipeDirection.NONE;
						}
                        break;
					}
                }
            }
        }
#endif
	}

	// Generic get function that will work with both platforms
	// Input: Distance to look, Layer number to look for collision on
	// Output: A list of raycasthit if hits happened
	// DO NOT BIT ADJUST LAYERMASK IT DOES IT FOR YOU
	public List<RaycastHit> GetInput(int dist = 100, int layer = 1)
    {
#if DESKTOP
        return GetLeftMouseClickHit(dist, layer);
#elif MOBILE
        return GetSingleTapHit(dist, layer);
#endif
    }

	// This might get wonky with clicks, shouldn't worry about it as PC isn't our target right now
	// Input: Distance to look, Layer number to look for collision on
	// Output: A list of raycasthit if hits happened
	// DO NOT BIT ADJUST LAYERMASK IT DOES IT FOR YOU
	public List<RaycastHit> GetLeftMouseClickHit(int dist = 100, int layer = 1)
    {
#if DESKTOP
		RaycastHit rayHit;
		List<RaycastHit> ret = new List<RaycastHit>();
        if (!leftClick)
            return ret;

        Ray ray = Camera.main.ScreenPointToRay(lastLeftClickPos);
		if(Physics.Raycast(ray, out rayHit, dist, 1 << layer))
		{
			ret.Add(rayHit);
		}
        return ret;
#endif
        return null;
    }

	// This might get wonky with clicks, shouldn't worry about it as PC isn't our target right now
	// Input: Distance to look, Layer number to look for collision on
	// Output: A list of raycasthit if hits happened
	// DO NOT BIT ADJUST LAYERMASK IT DOES IT FOR YOU
	public List<RaycastHit> GetRightMouseClickHit(int dist = 100, int layer = 1)
    {
#if DESKTOP
		RaycastHit rayHit;
		List<RaycastHit> ret = new List<RaycastHit>();
		if (!rightClick)
			return ret;

		Ray ray = Camera.main.ScreenPointToRay(lastRightClickPos);
		if (Physics.Raycast(ray, out rayHit, dist, 1 << layer))
		{
			ret.Add(rayHit);
		}
		return ret;
#endif
		return null;
    }

	// Mobile specific single input
	// Input: Distance to look, Layer number to look for collision on
	// Output: A list of raycasthit if hits happened
	// DO NOT BIT ADJUST LAYERMASK IT DOES IT FOR YOU
	public List<RaycastHit> GetSingleTapHit(int dist = 100, int layer = 1)
    {

#if MOBILE
		RaycastHit rayHit;
		List<RaycastHit> ret = new List<RaycastHit>();
        if(tapCount == 0 || mSwipeData[0].isSwiping)
            return ret;

        Ray ray = Camera.main.ScreenPointToRay(taps[0].position);
		if(Physics.Raycast(ray, out rayHit, dist, 1 << layer))
		{
			ret.Add(rayHit);
		}
        return ret;
#endif
		return null;
    }

	// Mobile specific multiple input
	// Input: Distance to look, Layer number to look for collision on
	// Output: A list of raycasthit if hits happened
	// DO NOT BIT ADJUST LAYERMASK IT DOES IT FOR YOU
	public List<RaycastHit> GetMultipleTapHit(int dist = 100, int layer = 1)
    {
#if MOBILE
		List<RaycastHit> ret = new List<RaycastHit>();
        if (tapCount == 0)
            return ret;
        Ray ray;
		RaycastHit rayHit;
        Camera main = Camera.main;
        for(int i = 0; i < tapCount; ++i)
        {
			if(!mSwipeData[i].isSwiping)
			{
			    ray = main.ScreenPointToRay(taps[i].position);
				if(Physics.Raycast(ray, out rayHit, dist, 1 << layer));
				{
					ret.Add(rayHit);
				}
			}
        }
        return ret;
#endif
        return null;
    }

	// Get the Swipe data at specific index
	public SwipeData GetSwipeDataIndex(int index)
    {
		SwipeData returnData = mSwipeData[index];
        returnData.direction = FindDirection(returnData.deltaPos);

        return returnData;
    }

	// Get all the swipe data
	public SwipeData[] GetSwipeData()
	{
		int iter;
#if DESKTOP
		iter = 2;
#elif MOBILE
		iter = tapCount;
#endif
		for (int i = 0; i < iter; ++i)
		{
			mSwipeData[i].direction = FindDirection(mSwipeData[i].deltaPos);
		}
		return mSwipeData;
	}

	// Helper function that returns the generic direction
	private SwipeDirection FindDirection(Vector2 vector)
	{
		if(vector.sqrMagnitude == 0)
		{
			return SwipeDirection.NONE;
		}
		return (Mathf.Abs(vector.x) > Mathf.Abs(vector.y)) ?
					((vector.x >= 0) ? SwipeDirection.RIGHT : SwipeDirection.LEFT) :
					((vector.y >= 0) ? SwipeDirection.UP : SwipeDirection.DOWN);
    }

	public List<int> GetSwipeCount()
	{
		List<int> ret = new List<int>();
		for(int i = 0; i < MAX_TAPS; ++i)
		{
			if(mSwipeData[i].isSwiping)
			{
				ret.Add(i);
			}
		}
		return ret;
	}
}
