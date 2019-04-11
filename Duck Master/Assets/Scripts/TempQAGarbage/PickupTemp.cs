using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTemp : MonoBehaviour
{
    public string NarrativeToAdd;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FindObjectOfType<TableOfContents>().AddNewJournalEntry(NarrativeToAdd);
            Destroy(gameObject);
        }
    }
}
