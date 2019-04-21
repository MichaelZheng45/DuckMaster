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
	float cameraRotationSpeed = 1.5f;

	InputManager.SwipeData[] swipeData;
	int swipeCount;
	Vector2 moveDirection;
    Vector3 rotateAroundPos;

	int halfScreenWidth, halfScreenHeight;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = gameManager.GetComponent<InputManager>();
		swipeData = InputManager.DefaultSwipeDataArray;
		halfScreenWidth = Screen.width / 2;
		halfScreenWidth = Screen.height / 2;
        //rotateAroundPos = gameManager.GetComponent<GameManager>().tileMap.GetCenterPos();
    }

    // Update is called once per frame
    void Update()
    {
		swipeCount = inputManager.GetSwipeCount();
		swipeData = inputManager.GetSwipeData();
		moveDirection = swipeData[0].deltaPos;
        rotateAroundPos = gameManager.GetComponent<GameManager>().GetTileMap().GetCenterPos();

        // Change this to first and none of last
        //if (swipeCount == 1)
		//{
		//	moveDirection = Quaternion.Euler(0, 0, -(transform.rotation.eulerAngles.y)) * moveDirection * Time.deltaTime * cameraSpeed;
		//	Vector3 tempPos = transform.position + new Vector3(-moveDirection.x, 0, -moveDirection.y);
        //    
		//	// this has to change somehow? To a bounding box? Something for later on.
		//	// TO DO: Center based on the level
		//	//if (tempPos.x >= lowerBounds.x && tempPos.x <= upperBounds.x && tempPos.z >= lowerBounds.y && tempPos.z <= upperBounds.y)
		//	//{
		//		transform.position = tempPos;
		//	//}
		//}
		//else if(swipeCount >= 2)
		//{
        //    Vector2 touchZeroPrevPos = swipeData[0].currentPos - swipeData[0].deltaPos;
        //    Vector2 touchOnePrevPos = swipeData[1].currentPos - swipeData[1].deltaPos;
		//	Vector2 deltaPos = swipeData[0].currentPos - swipeData[1].currentPos;
		//	Vector2 prevDeltaPos = touchZeroPrevPos - touchOnePrevPos;
		//
		//	float prevTouchDeltaMag = (prevDeltaPos).magnitude;
        //    float touchDeltaMag = (deltaPos).magnitude;
		//
        //    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
		//	
		//	// difference between frames is going to be small
		//	// but the actual distance is going to be big
		//	// do rotation instead
        //    if((Mathf.Abs((deltaPos - prevDeltaPos).x) > 15 || Mathf.Abs(deltaPos.x) < quarterScreenWidth) && InputManager.AreOppositeDirections(swipeData[0].direction, swipeData[1].direction) && Mathf.Abs(deltaMagnitudeDiff) > .1f)
        //    {
        //        transform.GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * .05f;
        //        transform.GetComponent<Camera>().orthographicSize = Mathf.Clamp(transform.GetComponent<Camera>().orthographicSize, .1f, 5f);
        //    }
		//	// TO DO: fix opposite finger gesture
        //    else
        //    {
        //        float rotation = (swipeData[0].direction == InputManager.SwipeDirection.RIGHT || swipeData[0].direction == InputManager.SwipeDirection.LEFT) ?
        //        cameraRotationSpeed * moveDirection.x * Time.deltaTime : cameraRotationSpeed * moveDirection.y * Time.deltaTime;
        //        transform.RotateAround(rotateAroundPos, Vector3.up, rotation);
        //    }
		//}

		if(swipeCount == 1)
		{
			float rotation = (swipeData[0].direction == InputManager.SwipeDirection.RIGHT || swipeData[0].direction == InputManager.SwipeDirection.LEFT) ?
			cameraRotationSpeed * moveDirection.x * Time.deltaTime : cameraRotationSpeed * moveDirection.y * Time.deltaTime;
			transform.RotateAround(rotateAroundPos, Vector3.up, rotation);
		}
		else if(swipeCount >= 2)
		{
			Vector2 touchZeroPrevPos = swipeData[0].currentPos - swipeData[0].deltaPos;
			Vector2 touchOnePrevPos = swipeData[1].currentPos - swipeData[1].deltaPos;
			Vector2 deltaPos = swipeData[0].currentPos - swipeData[1].currentPos;
			Vector2 prevDeltaPos = touchZeroPrevPos - touchOnePrevPos;
			
			float prevTouchDeltaMag = (prevDeltaPos).magnitude;
			float touchDeltaMag = (deltaPos).magnitude;
			
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			if (/*InputManager.AreOppositeDirections(swipeData[0].direction, swipeData[1].direction)/* && */Mathf.Abs(deltaMagnitudeDiff) > 4/**/)
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
