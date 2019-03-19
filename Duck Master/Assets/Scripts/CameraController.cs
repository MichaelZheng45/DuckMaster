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
			moveDirection = Quaternion.Euler(0, 0, 45) * moveDirection * Time.deltaTime * cameraSpeed;
			Vector3 tempPos = transform.position + new Vector3(-moveDirection.x, 0, -moveDirection.y);

			// this has to change somehow? To a bounding box? Something for later on.
			if (tempPos.x >= lowerBounds.x && tempPos.x <= upperBounds.x && tempPos.z >= lowerBounds.y && tempPos.z <= upperBounds.y)
			{
				transform.position = tempPos;
			}
		}
		else if(swipeData[0].isSwiping && swipeData[1].isSwiping)
		{
			// do rotation instead
			Debug.Log("Other");
			transform.Rotate(0, moveDirection.x, 0);
		}
    }
}
