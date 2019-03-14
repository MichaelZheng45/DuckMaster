#if (UNITY_EDITOR || UNITY_STANDALONE)
#define DESKTOP
#elif (UNITY_IOS || UNITY_ANDROID)
#define MOBILE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
#if DESKTOP
    bool leftClick, rightClick;
    Vector3 lastLeftClick, lastRightClick;
    Vector3 upLeftClick;
#elif MOBILE
    Touch[] taps;
    int tapCount = 0;
    Vector2 deltaPos;
    bool swiping = false;
#endif
    public enum SwipeDirection
    {
        NONE = -1,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }
    // Start is called before the first frame update
    void Start()
    {
#if MOBILE
        deltaPos = new Vector2(0.0f, 0.0f);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        // in editor or on pc
#if DESKTOP
        leftClick = Input.GetMouseButtonDown(0);
        rightClick = Input.GetMouseButtonDown(1);
        if (leftClick)
            lastLeftClick = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0))
            upLeftClick = Input.mousePosition;
        if (rightClick)
            lastRightClick = Input.mousePosition;
        
#elif MOBILE
        taps = Input.touches;
        tapCount = Input.touchCount;
        if (tapCount > 0)
        {
            if (!swiping && taps[0].phase == TouchPhase.Moved)
            {
                deltaPos = new Vector2(0.0f, 0.0f);
                swiping = true;
            }
            else if(swiping && taps[0].phase == TouchPhase.Ended)
            {
                deltaPos += taps[0].deltaPosition;
                swiping = false;
            }
            if (swiping)
            {
                deltaPos += taps[0].deltaPosition;
            }
        }
        Debug.Log(deltaPos);
        Debug.Log(swiping);
#endif
    }

    public bool getInput(int dist = 100, int layer = 1)
    {
#if DESKTOP
        return getLeftMouseClickHit(dist, layer);
#elif MOBILE
        return getSingleTapHit(dist, layer);
#endif
    }
#if DESKTOP
    public bool getLeftMouseClickHit(int dist = 100, int layer = 1)
    {
        if (!leftClick)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(lastLeftClick);
        return Physics.Raycast(ray, dist, 1 << layer);
    }

    public bool getRightMouseClickHit(int dist = 100, int layer = 1)
    {
        if (!rightClick)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(lastRightClick);
        return Physics.Raycast(ray, dist, 1 << layer);
    }
#elif MOBILE
    public bool getSingleTapHit(int dist = 100, int layer = 1)
    {
        if(tapCount == 0)
            return false;

        Ray ray = Camera.main.ScreenPointToRay(taps[0].position);
        return Physics.Raycast(ray, dist, 1 << layer);
    }

    public bool[] getMultipleTapHit(int dist = 100, int layer = 1)
    {
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
    }
#endif

    // returns direction
    //  -1  : left swipe
    //  0   : no swipe
    //  1   : right swipe
    public SwipeDirection getSwipeDistance()
    {
        return SwipeDirection.NONE;
    }
}
