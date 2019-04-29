using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{

    int PositionIndex = 0;
    [SerializeField]
    Transform[] CameraPositions;

    public float timer = 10;

    // Start is called before the first frame update
    void Start()
    {

    }

    void CallTimer()
    {
        int i = Random.Range(0, 2);
        switch (i)
        {
            case 0:
                AnimationEventStuff.Whistle();
                break;
            case 1:
                AnimationEventStuff.Pickup();
                break;

        }

        timer = Random.Range(10, 30);
    }

    public void SetCameraPosition(int i)
    {
        PositionIndex = i;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            CallTimer();

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
