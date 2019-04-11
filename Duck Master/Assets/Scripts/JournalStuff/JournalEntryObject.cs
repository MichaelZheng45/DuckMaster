using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NewJournalEntry", menuName = "Journal/JournalEntry", order = 1)]
public class JournalEntryObject : ScriptableObject
{
    public string JournalEntryName = "default";
    public string JournalEntryText = "SampleText";
}
