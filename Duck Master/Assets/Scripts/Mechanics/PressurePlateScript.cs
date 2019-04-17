using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateScript : LogicInput
{
    bool pressed;
    [SerializeField] Material pressedMat;
    [SerializeField] Material unpressedMat;
    MeshRenderer rend;
    Collider playerCollider;
    Collider duckCollider;


    // Start is called before the first frame update
    void Start()
    {
        pressed = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    print("changing pressure plate states");
        //    pressed = true;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //Initial checks for plate
        string tag = other.gameObject.tag;
        //If Duck enters and no player 
        if (pressed == false && tag == "Duck" && playerCollider == null)
        {
            active = true;
            duckCollider = other;
            CallChange();
        }

        //If Player enters and no duck
        if (pressed == false && tag == "Player" && duckCollider == null)
        {
            active = true;
            playerCollider = other;
            CallChange();
        }

        //If duck here, add player
        if (pressed == true && tag == "Player")
            playerCollider = other;
        
        
        //If player here add duck
        if (pressed == true && tag == "Duck")           
            duckCollider = other;
     
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;

        //If Player leaving
        if (pressed == true && tag == "Player")
            playerCollider = null;

        //If duck leaving
        if (pressed == true && tag == "Duck")
            duckCollider = null;
        
        //If both are off turn off
        if (playerCollider == null && duckCollider == null)
        {
            //pressed = false;
            active = false;
            CallChange();
        }
    }
}
