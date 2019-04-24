using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EasterEgg : MonoBehaviour
{
    string CurrentBuffer;
    string[] Keywords = { "elvis", "falco" };
    List<string> currentKeywords = new List<string>();
    // Start is called before the first frame update
    private void Start()
    {
        ResetBuffer();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            CurrentBuffer += Input.inputString;
            if (CurrentBuffer.ToCharArray().Length > 1)
                CheckEggs();
        }

    }

    void CheckEggs()
    {
        List<string> tempRemoval = new List<string>();
        int i = CurrentBuffer.Length - 1;
        char newChar = CurrentBuffer.ToCharArray()[i];
        foreach (string keyword in currentKeywords)
        {
            if (CurrentBuffer == keyword)
            {
                EasterEggSelect();
                return;
            }
            if (newChar != keyword.ToCharArray()[i])
                tempRemoval.Add(keyword);
        }
        foreach (string keyword in tempRemoval)
        {
            currentKeywords.Remove(keyword);
        }
        if (currentKeywords.Count < 1)
            ResetBuffer();


    }

    void EasterEggSelect()
    {
        switch (CurrentBuffer)
        {
            case "elvis":
                Suave();
                break;

            case "falco":
                Debug.Log("THATS NOT FALCO");
                break;
        }

        ResetBuffer();
    }

    void ResetBuffer()
    {
        CurrentBuffer = "";
        currentKeywords.AddRange(Keywords);
    }

    public void Suave()
    {
        gameObject.AddComponent<AudioSource>();
        GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Special/SuaveSound");
        GetComponent<AudioSource>().Play();
        gameObject.AddComponent<VideoPlayer>();
        GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("Special/Crespo");
        GetComponent<VideoPlayer>().audioOutputMode = VideoAudioOutputMode.None;
        GetComponent<VideoPlayer>().targetTexture = Resources.Load<RenderTexture>("Special/Suave");
        GetComponent<VideoPlayer>().Play();
        RenderSettings.skybox = Resources.Load<Material>("Special/SuaveMat");
    }
}
