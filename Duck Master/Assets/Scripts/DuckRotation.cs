using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DuckRotationState
{
    LEFT,
    TOP,
    RIGHT,
    DOWN
};

public class DuckRotation : MonoBehaviour
{
    public DuckRotationState currentRotation { get; set; }

    [Tooltip("A number to fudge the rotation to the base rotation (top)")]
    [SerializeField] int rotationFactor;

    void Start()
    {
        //set new rotation
        updateDuckRotation();
    }

    public void rotateDuckToDirection(DuckRotationState direction)
    {
        currentRotation = direction;
        updateDuckRotation();
    }

    public void rotateDuck(Vector3 dir)
    {
        float angle = (Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg);

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, angle + rotationFactor, 0));

        //shifts the graph quadrant from a (+) to (x) and reduce range from 0 to 360. Finally divides it by 90 which will be a range from 0 to 3 when floored
        angle = nfmod(angle + 45, 360);

        angle = Mathf.FloorToInt((angle) / 90);

        //to compensate for the fact that left in connections starts at 0.
        currentRotation = (DuckRotationState)(nfmod(angle + 1, 4));
    }

    void updateDuckRotation()
    {
        switch (currentRotation)
        {
            case DuckRotationState.TOP:
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90 + rotationFactor, 0));
                break;
            case DuckRotationState.RIGHT:
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0 + rotationFactor, 0));
                break;
            case DuckRotationState.DOWN:
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 270 + rotationFactor, 0));
                break;
            case DuckRotationState.LEFT:
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180 + rotationFactor, 0));
                break;
            default:
                break;
        }
    }

    float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    public Vector3 findDirection()
    {
        switch (currentRotation)
        {
            case DuckRotationState.TOP:
                return new Vector3(0,0,1);
            case DuckRotationState.RIGHT:
                return new Vector3(1, 0, 0);
            case DuckRotationState.DOWN:
                return new Vector3(0, 0, -1);
            case DuckRotationState.LEFT:
                return new Vector3(-1, 0, 0);
            default:
                break;
        }
        return Vector3.zero;
    }
}
