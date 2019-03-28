﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyButton : MonoBehaviour
{
    [SerializeField] Material unpressedMaterial;
    [SerializeField] Material pressedMaterial;
    bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = unpressedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            isActive = true;
            GetComponent<Renderer>().material = pressedMaterial;
        }
    }

    public bool isPressed()
    {
        return isActive;
    }
}