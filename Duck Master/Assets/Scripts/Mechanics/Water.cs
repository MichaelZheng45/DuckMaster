using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaterDirections
{
    NONE,
    UP,
    RIGHT,
    DOWN,
    LEFT
}

public class Water : MonoBehaviour
{
    //PLEASE NOTE, DUCK SHOULD BE AT LEAST 1.25 scale in order for colliders to actually hit
    [SerializeField] WaterDirections direction = WaterDirections.NONE;
    [SerializeField] float moveSpeed = 2.0f;

    const float norm = 0.707f;

    //These directions will be determined based on the camera's orientation at start up
    Vector3 baseUp;
    Vector3 baseRight;
    Vector3 baseDown;
    Vector3 baseLeft;

    duckBehaviour duckbehavior;
    GameObject duck;

    // Start is called before the first frame update
    void Start()
    {
        baseUp = new Vector3(0, 0, 1);
        baseRight = new Vector3(1, 0, 0);
        baseDown = new Vector3(0, 0, -1);
        baseLeft = new Vector3(-1, 0, 0);

        GameObject camera = GameManager.Instance.GetMainCamera();

        //Old stuff that may come in useful if things change
        //Vector3 init;

        //init = camera.transform.TransformDirection(baseUpRight);
        //upRight = new Vector3(init.x, 0.0f, init.z);
        //
        //init = camera.transform.TransformDirection(baseUpLeft);
        //upLeft = new Vector3(init.x, 0.0f, init.z);
        //
        //init = camera.transform.TransformDirection(baseDownRight);
        //downRight = new Vector3(init.x, 0.0f, init.z);
        //
        //init = camera.transform.TransformDirection(baseDownLeft);
        //downLeft = new Vector3(init.x, 0.0f, init.z);

        //When accounting for how the camera is facing initially, the directions are as follows
    }

    // Update is called once per frame
    void Update()
    {
        if (duckbehavior != null)
        {
            if (duckbehavior.mDuckState == DuckStates.STILL)
            {
                Vector3 position = duck.transform.position;
                if (direction == WaterDirections.UP)
                    position += (baseUp * moveSpeed * Time.deltaTime);
                    
                if (direction == WaterDirections.RIGHT)
                    position += (baseRight * moveSpeed * Time.deltaTime);

                if (direction == WaterDirections.DOWN)
                    position += (baseDown * moveSpeed * Time.deltaTime);

                if (direction == WaterDirections.LEFT)
                    position += (baseLeft * moveSpeed * Time.deltaTime);

                if(GameManager.Instance.GetTileMap().getTileFromPosition(position) != null)
                {
                    duck.transform.position = position;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Duck")
        {
            print("Water collision with Duck!");
            duck = other.gameObject;
            duckbehavior = other.gameObject.GetComponent<duckBehaviour>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Duck")
        {
            print("Duck has left water tile");
            duck = null;
            duckbehavior = null;
        }
    }

    public WaterDirections GetWaterDirection()
    {
        return direction;
    }
}
