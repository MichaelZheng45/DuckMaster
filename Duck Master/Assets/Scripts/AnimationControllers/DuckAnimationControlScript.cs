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

    ParticleSystem Ground;
    GameObject Trail;

    public void Start()
    {
        //SoundPlayer = Resources.Load<GameObject>("Sounds/Soundplayer");
        Trail = transform.Find("Trail").gameObject;
        Ground = transform.Find("Landing").GetComponent<ParticleSystem>();
        Trail.SetActive(false);
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
        AnimationEventStuff.onWhistle += Startle;
    }

    private void OnDisable()
    {
        AnimationEventStuff.onDuckWalkingChange -= ChangeWalk;
        AnimationEventStuff.onThrow -= StartThrow;
        AnimationEventStuff.onDuckHeldChange -= ChangeHeld;
        AnimationEventStuff.onDuckInAirChange -= ChangeInAir;
        AnimationEventStuff.onWhistle -= Startle;
    }

    void ChangeWalk(bool newWalk)
    {
        animator.SetBool("Walking", newWalk);
        ChangeHeld(false);
    }

    void StartThrow()
    {
        animator.SetTrigger("Throw");
    }

    void ChangeHeld(bool newHeld)
    {
        animator.SetBool("Held", newHeld);
    }

    void Startle()
    {
        animator.SetTrigger("Startle");
    }

    void ChangeInAir(bool newInAir)
    {
        animator.SetBool("InAir", newInAir);
        Trail.SetActive(newInAir);

        if (!newInAir)
        {
            Ground.Play();
        }
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
