using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorMoveTo : MonoBehaviour
{
    Vector3 JournalTo;
    // Start is called before the first frame update
    void Start()
    {
        JournalTo = Camera.main.ScreenToWorldPoint(GameObject.Find("OpenJournal").transform.position);
        Debug.Log("HI");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, JournalTo, 0.05f);
        if (Vector3.Distance(transform.position, JournalTo) < .01f)
            Destroy(gameObject);
    }
}
