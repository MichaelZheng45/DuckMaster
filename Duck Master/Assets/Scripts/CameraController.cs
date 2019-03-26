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
	Vector2 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = gameManager.GetComponent<InputManager>();
		swipeData = new InputManager.SwipeData[InputManager.MAX_TAPS];
    }

    // Update is called once per frame
    void Update()
    {
        swipeData = inputManager.GetSwipeData(true);// * Time.deltaTime;
		moveDirection = swipeData[0].deltaPos;
		// Change this to first and none of last
		if(swipeData[0].isSwiping && !swipeData[1].isSwiping)
		{
			moveDirection = Quaternion.Euler(0, 0, -(transform.rotation.eulerAngles.y)) * moveDirection * Time.deltaTime * cameraSpeed;
			Vector3 tempPos = transform.position + new Vector3(-moveDirection.x, 0, -moveDirection.y);

			// this has to change somehow? To a bounding box? Something for later on.
			// TO DO: Center based on the level
			if (tempPos.x >= lowerBounds.x && tempPos.x <= upperBounds.x && tempPos.z >= lowerBounds.y && tempPos.z <= upperBounds.y)
			{
				transform.position = tempPos;
			}
			Debug.Log("Move direction:" + moveDirection);
		}
		else if(swipeData[0].isSwiping && swipeData[1].isSwiping)
		{
			// do rotation instead
			//transform.Rotate(0, moveDirection.x, 0);
			// TO DO: Make it center of map
			transform.RotateAround(Vector3.zero, Vector3.up, cameraRotationSpeed * moveDirection.y * Time.deltaTime);
			Debug.Log("Camera Rotation" + transform.rotation.eulerAngles);
		}
    }
}
