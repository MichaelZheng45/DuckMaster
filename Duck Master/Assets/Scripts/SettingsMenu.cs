using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    AudioMixer PlayerSounds;

    // Start is called before the first frame update
    void Start()
    {
        Toggle();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
        {
            GetComponentInChildren<Button>().image.enabled = false;
        }
        else
        {
            Camera.main.GetComponent<CameraController>().SetMovable(!gameObject.activeInHierarchy);
        }

    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateVolume(string VolumeGroup, float newVolume)
    {
        if (PlayerSounds)
        {
            switch (VolumeGroup)
            {
                case "Master":
                    PlayerSounds.SetFloat("MasterVol", newVolume);
                    break;
                case "SFX":
                    PlayerSounds.SetFloat("SfxVol", newVolume);
                    PlayerSounds.SetFloat("PlayerVol", newVolume - 20);
                    break;
                case "Music":
                    PlayerSounds.SetFloat("MusicVol", newVolume);
                    break;
                default:
                    break;
            }
        }
    }

    public float GetVolume(string VolumeGroup)
    {
        float valueReturn = 0;
        if (PlayerSounds)
        {
            switch (VolumeGroup)
            {
                case "Master":
                    PlayerSounds.GetFloat("MasterVol", out valueReturn);
                    break;
                case "SFX":
                    PlayerSounds.GetFloat("SfxVol", out valueReturn);
                    break;
                case "Music":
                    PlayerSounds.GetFloat("MusicVol", out valueReturn);
                    break;
                default:
                    valueReturn = 200;
                    break;
            }
        }
        else
            valueReturn = 200;

        return valueReturn;
    }
}
