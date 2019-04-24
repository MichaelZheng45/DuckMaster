using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewSaveGame", menuName = "Journal/SaveGame", order = 1)]
public class JournalSaveObjects : ScriptableObject
{
    public List<JournalEntryObject> CollectedObjects = new List<JournalEntryObject>();
    public int levelsUnlocked = 1;
}
