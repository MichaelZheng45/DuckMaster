using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitTypeHolder : MonoBehaviour
{
    [SerializeField]
    GameObject attractParticle;
    [SerializeField]
    GameObject repelParticle;

    BaitTypes bait;

    public void SetBaitType(BaitTypes _bait)
    {
        bait = _bait;
        GameObject g = null;
        ParticleSystem.MainModule p;
        switch (bait)
        {
            case BaitTypes.ATTRACT:
                GetComponent<Renderer>().material.color = new Color(0, .6f, 0);
                g = Instantiate(attractParticle, transform);
                p = g.GetComponent<ParticleSystem>().main;
                p.startColor = new Color(0, .5f, 0);
                break;

            case BaitTypes.REPEL:
                GetComponent<Renderer>().material.color = new Color(1, .7f, 0);
                g = Instantiate(repelParticle, transform);
                p = g.GetComponent<ParticleSystem>().main;
                p.startColor = new Color(1, .6f, 0);
                break;

            case BaitTypes.PEPPER:
                GetComponent<Renderer>().material.color = new Color(.7f, 0, 0);
                g = Instantiate(attractParticle, transform);
                p = g.GetComponent<ParticleSystem>().main;
                p.startColor = new Color(.7f, 0, 0);
                break;
        }

        g.transform.position = transform.position;

    }
    public BaitTypes GetBaitType()
    {
        return bait;
    }
}
