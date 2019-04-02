using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyButton : MonoBehaviour, LogicInput
{
    [SerializeField] Material unpressedMaterial;
    [SerializeField] Material pressedMaterial;
    bool active;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = unpressedMaterial;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
        {
            active = true;
            GetComponent<Renderer>().material = pressedMaterial;
        }
    }

    public bool IsActive()
    {
        return active;
    }
}
