using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableOfContentsButton : Button
{
    public string JournalEntryName;

    public Text JournalEntryLabel;
    TableOfContents toc;

    protected override void Awake()
    {
        JournalEntryLabel = transform.Find("Text").GetComponent<Text>();
        onClick.AddListener(() => toc.GoToJournalEntry(JournalEntryName));
        base.Awake();
    }

    public void UpdateText(TableOfContents _toc,string _JournalEntryName)
    {
        toc = _toc;
        JournalEntryName = _JournalEntryName;
        JournalEntryLabel.text = JournalEntryName;
    }
}
