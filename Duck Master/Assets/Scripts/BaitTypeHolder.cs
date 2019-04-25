using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitTypeHolder : MonoBehaviour
{
    BaitTypes bait;


    public void SetBaitType(BaitTypes _bait)
    {
        bait = _bait;
        switch (bait)
        {
            case BaitTypes.ATTRACT:
                GetComponent<Renderer>().material.color = Color.green;
                break;
            case BaitTypes.REPEL:
                GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case BaitTypes.PEPPER:
                GetComponent<Renderer>().material.color = Color.red;
                break;

        }

    }
    public BaitTypes GetBaitType()
    {
        return bait;
    }
}
