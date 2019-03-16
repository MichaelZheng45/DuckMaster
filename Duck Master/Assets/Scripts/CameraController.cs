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

    Vector2 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = gameManager.GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = inputManager.GetSwipeData(true).deltaPos;// * Time.deltaTime;
        moveDirection = Quaternion.Euler(0, 0, 45) * moveDirection * Time.deltaTime * cameraSpeed;
        Vector3 tempPos = transform.position + new Vector3(-moveDirection.x, 0, -moveDirection.y);

        // this has to change somehow? To a bounding box? Something for later on.
        if(tempPos.x >= lowerBounds.x && tempPos.x <= upperBounds.x && tempPos.z >= lowerBounds.y && tempPos.z <= upperBounds.y)
        {
            transform.position = tempPos;
        }
    }
}
