using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuckAnimationControlScript : MonoBehaviour
{
    Animator animator;
    SoundFile[] Sounds;
	[SerializeField]
    GameObject SoundPlayer;
	AudioSource audioSource;

    public void Start()
    {
        //SoundPlayer = Resources.Load<GameObject>("Sounds/Soundplayer");
        animator = GetComponent<Animator>();
		audioSource = SoundPlayer.GetComponent<AudioSource>();

        Sounds = new SoundFile[] {
            //new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep1"), new string[]{ "Duck", "Walking" }),
            //new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep2"), new string[]{ "Duck", "Walking" }),
            //new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep3"), new string[]{ "Duck", "Walking" }),
            //new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep4"), new string[]{ "Duck", "Walking" }),
            };
    }

    private void OnEnable()
    {
        AnimationEventStuff.onDuckWalkingChange += ChangeWalk;
        AnimationEventStuff.onThrow += StartThrow;
        AnimationEventStuff.onDuckHeldChange += ChangeHeld;
        AnimationEventStuff.onDuckInAirChange += ChangeInAir;
    }

    private void OnDisable()
    {
        AnimationEventStuff.onDuckWalkingChange -= ChangeWalk;
        AnimationEventStuff.onThrow -= StartThrow;
        AnimationEventStuff.onDuckHeldChange -= ChangeHeld;
        AnimationEventStuff.onDuckInAirChange -= ChangeInAir;
    }

    void ChangeWalk(bool newWalk)
    {
        animator.SetBool("Walking", newWalk);
        ChangeHeld(false);
        ChangeInAir(false);
    }

    void StartThrow()
    {
        animator.SetTrigger("Throw");
        ChangeHeld(false);
        ChangeInAir(true);
    }

    void ChangeHeld(bool newHeld)
    {
        animator.SetBool("Held", newHeld);
    }
    void ChangeInAir(bool newInAir)
    {
        animator.SetBool("InAir", newInAir);
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
			// TO DO: Change this so it's playing on a single block
            AudioClip ac = tempSounds[(int)Random.Range(0, tempSounds.Count)].GetClip();
			audioSource.clip = ac;
			audioSource.Play();
		}
    }

}
