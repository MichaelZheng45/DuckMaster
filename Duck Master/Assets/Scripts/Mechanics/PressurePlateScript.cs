using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : LogicInput
{
    //These do not need to be public, they are handled in collision functions
    Collider playerCollider;
    Collider duckCollider;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
    }

    public override void CallChange()
    {
        GetComponent<Animator>().SetBool("Active", active);
        Debug.Log(active);
        if (active)
        {
            //GetComponentInChildren<AudioSource>().Play();
            GetComponentInChildren<ParticleSystem>().Play();
        }

        base.CallChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Initial checks for plate
        string tag = other.gameObject.tag;
        //If Duck enters and no player 
        if (active == false && tag == "Duck" && playerCollider == null)
        {
            active = true;
            duckCollider = other;
            CallChange();
        }

        //If Player enters and no duck
        if (active == false && tag == "Player" && duckCollider == null)
        {
            active = true;
            playerCollider = other;
            CallChange();
        }

        //If duck here, add player
        if (active == true && tag == "Player")
            playerCollider = other;


        //If player here add duck
        if (active == true && tag == "Duck")
            duckCollider = other;

    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        //If Player leaving
        if (active == true && tag == "Player")
            playerCollider = null;

        //If duck leaving
        if (active == true && tag == "Duck")
        {
            duckCollider = null;
        }
        //If both are off turn off
        if (playerCollider == null && duckCollider == null)
        {
            active = false;
            CallChange();
        }
    }
}
