using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFlow : MonoBehaviour
{
    Animator anim;
    int setToLoad = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void GoToScene(int SceneToLoad)
    {
        anim.SetBool("Block", true);
        setToLoad = SceneToLoad;
    }

    public void ActualSceneLoad()
    {
        SceneManager.LoadSceneAsync(setToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
