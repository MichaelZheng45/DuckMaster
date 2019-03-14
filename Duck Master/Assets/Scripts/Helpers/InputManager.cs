using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
#if (UNITY_EDITOR || UNITY_STANDALONE)
    Vector3 lastLeftClick, lastRightClick;
    Vector3 upLeftClick;
//#elif (UNITY_IOS || UNITY_ANDROID)
    Touch[] taps;
    int tapCount;
    Vector3 lastPos;

    public enum SwipeDirection
    {
        NONE = -1,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }
#endif
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // in editor or on pc
#if (UNITY_EDITOR || UNITY_STANDALONE)
        if (Input.GetMouseButtonDown(0))
            lastLeftClick = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0))
            upLeftClick = Input.mousePosition;
        if (Input.GetMouseButtonDown(1))
            lastRightClick = Input.mousePosition;
        
//#elif (UNITY_IOS || UNITY_ANDROID)
        if(tapCount > 0)
        {
            lastPos = taps[0].position;
        }
        taps = Input.touches;
        tapCount = Input.touchCount;
        
#endif
    }

    public bool getInput(int dist = 100, int layer = 1)
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return getLeftMouseClickHit(dist, layer);
#elif (UNITY_ANDROID || UNITY_IOS)
        return getSingleTapHit(dist, layer);
#endif
    }

    public bool getLeftMouseClickHit(int dist = 100, int layer = 1)
    {
        Ray ray = Camera.main.ScreenPointToRay(lastLeftClick);
        return Physics.Raycast(ray, dist, 1 << layer);
    }

    public bool getRightMouseClickHit(int dist = 100, int layer = 1)
    {
        Ray ray = Camera.main.ScreenPointToRay(lastRightClick);
        return Physics.Raycast(ray, dist, 1 << layer);
    }

    public bool getSingleTapHit(int dist = 100, int layer = 1)
    {
        Ray ray = Camera.main.ScreenPointToRay(taps[0].position);
        return Physics.Raycast(ray, dist, 1 << layer);
    }

    public bool[] getMultipleTapHit(int dist = 100, int layer = 1)
    {
        bool[] ret = new bool[tapCount];
        Ray ray;
        Camera main = Camera.main;
        for(int i = 0; i < tapCount; ++i)
        {
            ray = main.ScreenPointToRay(taps[i].position);
            ret[i] = Physics.Raycast(ray, dist, 1 << layer);
        }
        return ret;
    }

    // returns direction
    //  -1  : left swipe
    //  0   : no swipe
    //  1   : right swipe
    public SwipeDirection getSwipeDistance()
    {
        return SwipeDirection.NONE;
    }
}
