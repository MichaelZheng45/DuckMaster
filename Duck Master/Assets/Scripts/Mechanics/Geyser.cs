using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    ParticleSystem gush;
    // Start is called before the first frame update
    void Start()
    {
        gush = transform.Find("Geyser_water").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //do your thing here
    public void geyserAwake()
    {
        gush.Stop();
        gush.Play();
    }
}
