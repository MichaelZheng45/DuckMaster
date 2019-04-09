using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyButton : LogicInput
{
    [SerializeField] Material unpressedMaterial;
    [SerializeField] Material pressedMaterial;
    bool active;
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Renderer>().material = unpressedMaterial;
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    print("changing sticky buttons states");
        //    active = !active;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
        {
            transform.Find("ButtonParticle").GetComponent<ParticleSystem>().Play();
            active = true;
            //GetComponent<Renderer>().material = pressedMaterial;
        }
    }

    public override bool IsActive()
    {
        return active;
    }
}
