using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTemp : MonoBehaviour
{
    public string NarrativeToAdd;
    bool destroying;

    ParticleSystem normalParticle;
    ParticleSystem destroyParticle;

    [SerializeField]
    GameObject journalParticle;

    // Start is called before the first frame update
    void Start()
    {
        normalParticle = transform.Find("QuillParticle").GetComponent<ParticleSystem>();
        destroyParticle = transform.Find("QuillDisappear").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!destroying && (other.tag == "Player" || other.tag == "Duck"))
        {
            FindObjectOfType<TableOfContents>().AddNewJournalEntry(NarrativeToAdd);
            destroying = true;
            normalParticle.Stop();
            destroyParticle.Play();
            GameObject g = Instantiate(journalParticle);
            g.transform.position = transform.position;
            Destroy(gameObject, 1);

        }
    }
}
