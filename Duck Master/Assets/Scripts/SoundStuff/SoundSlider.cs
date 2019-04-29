using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSlider : Slider
{
    SettingsMenu sm;

    protected override void Start()
    {
        transform.Find("Label").GetComponent<Text>().text = name;
        base.Start();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        sm = FindObjectOfType<SettingsMenu>();
        value = (sm.GetVolume(name) == 200) ? value : sm.GetVolume(name);
    }

    public void UpdateSlider()
    {
        if (sm)
            sm.UpdateVolume(name, value);
    }
}
