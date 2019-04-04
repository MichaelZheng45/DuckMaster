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
        rend = gameObject.GetComponent<MeshRenderer>();
        rend.material = unpressedMat;
        pressed = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        //Initial checks for plate
        string tag = other.gameObject.tag;
        //If Duck enters and no player 
        if (pressed == false && tag == "Duck" && playerCollider == null)
        {
            pressed = true;
            rend.material = pressedMat;
            duckCollider = other;
            //GameManager.Instance.buttonActivated();
        }

        //If Player enters and no duck
        if (pressed == false && tag == "Player" && duckCollider == null)
        {
            pressed = true;
            playerCollider = other;
            rend.material = pressedMat;
            //GameManager.Instance.buttonActivated();
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
            pressed = false;
            rend.material = unpressedMat;
        }
    }

    public override bool IsActive()
    {
        return pressed;
    }
}
