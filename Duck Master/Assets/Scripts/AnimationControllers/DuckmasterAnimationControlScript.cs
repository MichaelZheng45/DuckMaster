﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuckmasterAnimationControlScript : MonoBehaviour
{
    Animator animator;
    SoundFile[] Sounds;
    GameObject SoundPlayer;

    public void Awake()
    {
        animator = GetComponent<Animator>();
        SoundPlayer = Resources.Load<GameObject>("Sounds/Soundplayer");

        Sounds = new SoundFile[] {
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep1"), new string[]{ "Duckmaster", "Walking" }),
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep2"), new string[]{ "Duckmaster", "Walking" }),
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep3"), new string[]{ "Duckmaster", "Walking" }),
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep4"), new string[]{ "Duckmaster", "Walking" }),
            };
    }

    private void OnEnable()
    {
        AnimationEventStuff.onWalkingChange += ChangeWalk;
    }

    private void OnDisable()
    {
        AnimationEventStuff.onWalkingChange -= ChangeWalk;
    }

    void ChangeWalk(bool newWalk)
    {
        animator.SetBool("Walking", newWalk);
        Debug.Log(newWalk);
    }

    public void PlaySound(AnimationEvent soundsToPlay)
    {
        string[] tempTags = soundsToPlay.stringParameter.Split(',');

        List<SoundFile> tempSounds = new List<SoundFile>();

        foreach (SoundFile sf in Sounds)
        {
            if (sf.HasTag(tempTags[0]))
                tempSounds.Add(sf);
        }

        for (int i = 1; i < tempTags.Length; i++)
        {
            foreach (SoundFile sf in tempSounds)
            {
                if (!sf.HasTag(tempTags[i]))
                    tempSounds.Remove(sf);
            }
        }
        if (tempSounds.Count > 0)
        {
            AudioClip ac = tempSounds[(int)Random.Range(0, tempSounds.Count)].GetClip();
            GameObject g = Instantiate(SoundPlayer);
            g.transform.position = transform.position;
            g.GetComponent<AudioSource>().clip = ac;
            g.GetComponent<AudioSource>().Play();
            Destroy(g, ac.length);
        }
    }

}