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
	/////// The taps might not work but will always have to be checked in update because of the fact that they get reset every frame \\\\\\\\

	/// <summary>
	/// Class to abstract the difference between PC and Mobile platform
	/// This makes it easier to iterate and develop code for testing on both platforms
	/// </summary>

	Camera mainCamera;
#if DESKTOP
	// Keep track of it we have clicked, where we started the click, and if we recently let go
    private bool leftClick, rightClick;
    private Vector3 lastMousePos, lastLeftClickPos, lastRightClickPos;
    private Vector3 upLeftClick;
#elif MOBILE
	// Keep track of the current touches and amount
    private Touch[] taps;
    private int tapCount = 0;
#endif
	[Tooltip("Pixel Distance A Tap Becomes A Swipe")]
    [SerializeField]
    private int swipeTolerance = 25;
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
	private SwipeData[] mSwipeData;

	public static SwipeData[] DefaultSwipeDataArray = new SwipeData[MAX_TAPS];
	public static SwipeData DefaultSwipeData = new SwipeData();

	// Data for storing raycasts
	private List<RaycastHit> mRaycastHits;

	public static RaycastHit DefaultRaycastHit = new RaycastHit();

	// Start is called before the first frame update
	void Start()
    {
		mainCamera = Camera.main;
		mSwipeData = DefaultSwipeDataArray;
		RaycastHit temp = new RaycastHit();
		mRaycastHits = new List<RaycastHit>(new RaycastHit[] { temp, temp, temp, temp, temp });
		mRaycastHits.Capacity = MAX_TAPS;
	}

    // Update is called once per frame
    void Update()
    {
		for (int i = 0; i < MAX_TAPS; ++i)
		{
			mRaycastHits[i] = DefaultRaycastHit;
		}
		// in editor or on pc
#if DESKTOP
		// TO DO: needs to be changes for clicks + swipes
		// If our mouse is down we get delta for swiping
		for(int i = 0; i < 2; ++i)
		{
			if (Input.GetMouseButtonDown(i))
			{
				mSwipeData[i].startPos = Input.mousePosition;
			}
			else if (Input.GetMouseButton(i))
			{
				mSwipeData[i].deltaPos = (Vector2)lastMousePos - mSwipeData[i].currentPos;
				mSwipeData[i].currentPos = (Vector2)lastMousePos;
				if(!mSwipeData[i].isSwiping && (Mathf.Abs(lastMousePos.x - mSwipeData[i].startPos.x) > swipeTolerance || Mathf.Abs(lastMousePos.y - mSwipeData[i].startPos.y) > swipeTolerance))
				{
					mSwipeData[i].isSwiping = true;
					mSwipeData[i].direction = SwipeDirection.NONE;
				}
			}
			else if(Input.GetMouseButtonUp(i))
			{
				if(mSwipeData[i].isSwiping)
				{
					mSwipeData[i].isSwiping = false;
					mSwipeData[i].deltaPos = Vector2.zero;
				}
				else
				{
					Ray ray = mainCamera.ScreenPointToRay(lastMousePos);
					RaycastHit rayHit;
					Physics.Raycast(ray, out rayHit, 100);
					mRaycastHits[i] = rayHit;
				}
			}
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
						// we aren't previously swiping and we are outside the swipe tolerance
						if(!mSwipeData[i].isSwiping && (Mathf.Abs(taps[i].position.x - mSwipeData[i].startPos.x) > swipeTolerance || Mathf.Abs(taps[i].position.y - mSwipeData[i].startPos.y) > swipeTolerance))
						{
							// we need to start swiping
							mSwipeData[i].isSwiping = true;
							//mSwipeData[i].deltaPos = mSwipeData[i].currentPos = Vector2.zero;
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
							Ray ray = mainCamera.ScreenPointToRay(taps[i].position);
							RaycastHit rayHit;
							Physics.Raycast(ray, out rayHit, 100);
							mRaycastHits[i] = rayHit;
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

	// For getting the raycast hit object
	public List<RaycastHit> GetTapHits()
	{
		return mRaycastHits;
	}

	// For looking at a specific layer
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

		Ray ray = mainCamera.ScreenPointToRay(lastLeftClickPos);
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

		Ray ray = mainCamera.ScreenPointToRay(lastRightClickPos);
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

        Ray ray = mainCamera.ScreenPointToRay(taps[0].position);
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
        for(int i = 0; i < tapCount; ++i)
        {
			if(!mSwipeData[i].isSwiping)
			{
			    ray = mainCamera.ScreenPointToRay(taps[i].position);
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

	public int GetSwipeCount()
	{
		int num = 0;
		for(int i = 0; i < MAX_TAPS; ++i)
		{
			if(mSwipeData[i].isSwiping)
			{
				num++;
			}
		}
		return num;
	}
}
