using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject gameManager;
    InputManager inputManager;

    [SerializeField]
    Vector2 lowerBounds;
    [SerializeField]
    Vector2 upperBounds;

    [SerializeField]
    float cameraSpeed = 1.0f;
    [SerializeField]
    float cameraRotationSpeed = 1f;

    InputManager.SwipeData[] swipeData;
    int swipeCount;
    Vector2 moveDirection;
    Vector3 rotateAroundPos;
    bool movable = true;

    int halfScreenWidth, halfScreenHeight;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = gameManager.GetComponent<InputManager>();
        swipeData = InputManager.DefaultSwipeDataArray;
        halfScreenWidth = Screen.width / 2;
        halfScreenHeight = Screen.height / 2;
        rotateAroundPos = GameManager.Instance.GetTileMap().GetCenterPos();
    }

    public void SetMovable(bool newMovable)
    {
        movable = newMovable;
    }

    // Update is called once per frame
    void Update()
    {
        swipeCount = inputManager.GetSwipeCount();
        swipeData = inputManager.GetSwipeData();
        moveDirection = swipeData[0].deltaPos;

        if (movable)
        {

            if (swipeCount == 1)
            {
                // all I'm doing is treating the map and screen as a circle and going to the next point
                Vector2 touchZeroPrevPos = swipeData[0].currentPos - swipeData[0].deltaPos;
                float anglePrev = Mathf.Atan2(touchZeroPrevPos.y - halfScreenHeight, touchZeroPrevPos.x - halfScreenWidth) * Mathf.Rad2Deg;
                float angleCurr = Mathf.Atan2(swipeData[0].currentPos.y - halfScreenHeight, swipeData[0].currentPos.x - halfScreenWidth) * Mathf.Rad2Deg;

                transform.RotateAround(rotateAroundPos, Vector3.up, (anglePrev - angleCurr) * -cameraRotationSpeed);
            }
            else if (swipeCount >= 2)
            {
                Vector2 touchZeroPrevPos = swipeData[0].currentPos - swipeData[0].deltaPos;
                Vector2 touchOnePrevPos = swipeData[1].currentPos - swipeData[1].deltaPos;
                Vector2 deltaPos = swipeData[0].currentPos - swipeData[1].currentPos;
                Vector2 prevDeltaPos = touchZeroPrevPos - touchOnePrevPos;

                float prevTouchDeltaMag = (prevDeltaPos).magnitude;
                float touchDeltaMag = (deltaPos).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                if (Mathf.Abs(deltaMagnitudeDiff) > 4)
                {
                    transform.GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * .01f;
                    transform.GetComponent<Camera>().orthographicSize = Mathf.Clamp(transform.GetComponent<Camera>().orthographicSize, .1f, 5f);
                }
                else
                {
                    moveDirection = Quaternion.Euler(0, 0, -(transform.rotation.eulerAngles.y)) * moveDirection * Time.deltaTime * cameraSpeed;
                    Vector3 tempPos = transform.position + new Vector3(-moveDirection.x, 0, -moveDirection.y);

                    // this has to change somehow? To a bounding box? Something for later on.
                    // TO DO: Center based on the level
                    //if (tempPos.x >= lowerBounds.x && tempPos.x <= upperBounds.x && tempPos.z >= lowerBounds.y && tempPos.z <= upperBounds.y)
                    //{
                    transform.position = tempPos;
                    //}
                }
            }
        }
    }
}

