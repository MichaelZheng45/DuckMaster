using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectSetup : MonoBehaviour
{
    [SerializeField]
    GameObject selectButton;
    public JournalSaveObjects SaveGame;

    // Start is called before the first frame update
    void Start()
    {
        SaveGame = new JournalSaveObjects();
        string dataPath = Path.Combine(Application.persistentDataPath, "SaveGame.txt");
        if (File.Exists(dataPath))
        {
            using (StreamReader streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                JsonUtility.FromJsonOverwrite(jsonString, SaveGame);
            }
        }
        else
        {
            string jsonString = JsonUtility.ToJson(SaveGame);
            using (StreamWriter streamWriter = File.CreateText(dataPath))
            {
                streamWriter.Write(jsonString);
            }
        }

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            GameObject g = Instantiate(selectButton, transform);
            g.GetComponent<LevelSelectButton>().SetupButton(i + 1);
            g.GetComponent<Button>().interactable = (SaveGame.levelsUnlocked > i);
            g.GetComponent<Image>().color = g.GetComponent<Button>().interactable ? new Color(1, 1, 1) : new Color(1, 1, 1, .5f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
