using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaterDirections
{
    UPRIGHT,
    UPLEFT,
    DOWNRIGHT,
    DOWNLEFT
}

public class Water : MonoBehaviour
{
    //PLEASE NOTE, DUCK SHOULD BE AT LEAST 1.25 scale in order for colliders to actually hit
    [SerializeField] WaterDirections direction = WaterDirections.DOWNRIGHT;
    [SerializeField] float moveSpeed = 2.0f;

    const float norm = 0.707f;

    Vector3 baseUpRight; 
    Vector3 baseUpLeft;
    Vector3 baseDownRight;
    Vector3 baseDownLeft;

    //These directions will be determined based on the camera's orientation at start up
    Vector3 upRight;
    Vector3 upLeft;
    Vector3 downRight;
    Vector3 downLeft;

    duckBehaviour duckbehavior;
    GameObject duck;

    // Start is called before the first frame update
    void Start()
    {
        baseUpRight = new Vector3(1, 0, 1);
        baseUpLeft = new Vector3(-1, 0, 1);
        baseDownRight = new Vector3(1, 0, -1);
        baseDownLeft = new Vector3(-1, 0, -1);

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
        upRight = Vector3.forward;
        upLeft = Vector3.back;
        downRight = Vector3.right;
        downLeft = Vector3.left;

    }

    // Update is called once per frame
    void Update()
    {
        if (duckbehavior != null)
        {
            if (duckbehavior.mDuckState == DuckStates.STILL)
            {
                if (direction == WaterDirections.UPRIGHT)
                    duck.transform.Translate(upRight * moveSpeed * Time.deltaTime);

                if (direction == WaterDirections.UPLEFT)
                    duck.transform.Translate(upLeft * moveSpeed * Time.deltaTime);

                if (direction == WaterDirections.DOWNRIGHT)
                    duck.transform.Translate(downRight * moveSpeed * Time.deltaTime);

                if (direction == WaterDirections.DOWNLEFT)
                    duck.transform.Translate(downLeft * moveSpeed * Time.deltaTime);
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
}
