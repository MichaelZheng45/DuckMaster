using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{

    int PositionIndex = 0;
    [SerializeField]
    Transform[] CameraPositions;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetCameraPosition(int i)
    {
        PositionIndex = i;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, CameraPositions[PositionIndex].position) > 1)
        {
            transform.position = Vector3.Lerp(transform.position, CameraPositions[PositionIndex].position, 0.05f);
            transform.rotation = Quaternion.Lerp(transform.rotation, CameraPositions[PositionIndex].rotation, 0.05f);
        }
        else
        {
            transform.position = CameraPositions[PositionIndex].position;
            transform.rotation = CameraPositions[PositionIndex].rotation;
        }
    }
}
