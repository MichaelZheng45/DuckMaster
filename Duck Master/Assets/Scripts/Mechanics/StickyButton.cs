using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyButton : LogicInput
{
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Renderer>().material = unpressedMaterial;
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
            GetComponentInChildren<Animation>().Play();
            GetComponentInChildren<AudioSource>().Play();
            GetComponentInChildren<ParticleSystem>().Play();
            CallChange();
        }
    }
}
