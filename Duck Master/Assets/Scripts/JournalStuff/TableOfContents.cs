using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableOfContents : MonoBehaviour
{

    Transform topMid;
    GameObject journalContainer;
    GameObject journalEntryPage;
    public GameObject jornalEntryButton;
    public Text JournalText;

    Image JournalBackground;

    public List<TableOfContentsButton> goToJournalButtons = new List<TableOfContentsButton>();
    public List<JournalEntryObject> pagesCollected = new List<JournalEntryObject>();

    // Start is called before the first frame update
    void Start()
    {
        JournalBackground = GetComponent<Image>();
        journalContainer = transform.GetChild(0).gameObject;
        topMid = journalContainer.transform.Find("TopMid");
        journalEntryPage = journalContainer.transform.Find("JournalPage").gameObject;
        AddNewJournalEntry("Duck Alter");
        AddNewJournalEntry("Duck Bath");
        CloseJournalEntry();
        CloseJournal();
        
    }

    public void AddNewJournalEntry(string JournalToLoad)
    {
        pagesCollected.Add(Resources.Load<JournalEntryObject>("scriptableObjects/Journal_Entries/" + JournalToLoad));
    }

    public void GoToJournalEntry(string _JournalEntryName)
    {
        foreach(JournalEntryObject jeo in pagesCollected)
        {
            if (jeo.JournalEntryName == _JournalEntryName)
            {
                journalEntryPage.SetActive(true);
                JournalText.text = jeo.JournalEntryText;
            }
        }
    }
    public void CloseJournalEntry()
    {
        journalEntryPage.SetActive(false);
    }

    public void UpdateJournalEntries()
    {
        foreach(JournalEntryObject jeo in pagesCollected)
        {
            bool exists = false;

            foreach(TableOfContentsButton tocb in goToJournalButtons)
            {
                if(tocb.JournalEntryName == jeo.JournalEntryName)
                {
                    exists = true;
                }
            }

            if (!exists)
            {
                GameObject g = Instantiate(jornalEntryButton, journalContainer.transform);
                if (goToJournalButtons.Count > 0)
                    g.transform.position = goToJournalButtons[goToJournalButtons.Count - 1].transform.position + new Vector3(0, -75, 0);
                else
                    g.transform.position = topMid.position + new Vector3(0, -75, 0);
                goToJournalButtons.Add(g.GetComponent<TableOfContentsButton>());
                Debug.Log(jeo.JournalEntryName);
                g.GetComponent<TableOfContentsButton>().UpdateText(this,jeo.JournalEntryName);
                g.transform.SetAsFirstSibling();
            }
        }
    }

    public void OpenJournal()
    {
        JournalBackground.enabled = true;
        journalContainer.SetActive(true);

        UpdateJournalEntries();
    }

    public void CloseJournal()
    {
        JournalBackground.enabled = false;
        journalContainer.SetActive(false);
    }
}
