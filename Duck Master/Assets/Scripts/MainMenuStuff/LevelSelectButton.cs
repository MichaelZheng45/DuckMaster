using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    int LevelToLoad;

    public void SetupButton(int setupInt)
    {
        LevelToLoad = setupInt;
        GetComponentInChildren<Text>().text = (setupInt).ToString();
    }

    public void LoadLevel()
    {
        SceneManager.LoadSceneAsync(LevelToLoad);
    }
}
