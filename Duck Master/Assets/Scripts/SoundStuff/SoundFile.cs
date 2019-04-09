using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFile
{
    AudioClip audioClip;
    string[] tags;

    public SoundFile(AudioClip _audioClip, string[] _tags)
    {
        audioClip = _audioClip;
        tags = _tags;
    }

    public bool HasTag(string tagCheck)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == tagCheck)
                return true;
        }
        return false;
    }
    public AudioClip GetClip()
    {
        return audioClip;
    }
}
