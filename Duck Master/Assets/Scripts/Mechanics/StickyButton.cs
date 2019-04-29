using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyButton : LogicInput
{
    ParticleSystem pressed;
    ParticleSystem failed;

    List<GameObject> entered = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Renderer>().material = unpressedMaterial;
        active = false;
        pressed = transform.Find("StickyParticle").GetComponent<ParticleSystem>();
        failed = transform.Find("FailParticle").GetComponent<ParticleSystem>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ResetButton(bool passed)
    {
        var m = failed.main;
        m.startColor = passed ? Color.green : Color.red;
        failed.Play();
        GetComponentInChildren<Animator>().SetBool("Active", active);
    }

    public override void CallChange()
    {
        GetComponentInChildren<Animator>().SetBool("Active", active);
        base.CallChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (entered.Count == 0)
        {
            if (other.tag == "Duck" || other.tag == "Player")
            {
                entered.Add(other.gameObject);
            }

            if (!active)
            {
                active = true;
                GetComponentInChildren<AudioSource>().Play();
                pressed.Play();
                CallChange();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Duck" || other.tag == "Player")
        {
            entered.Remove(other.gameObject);
        }
    }
}
