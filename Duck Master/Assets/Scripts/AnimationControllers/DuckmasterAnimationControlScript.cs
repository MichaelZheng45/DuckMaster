using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DuckmasterAnimationControlScript : MonoBehaviour
{
    Animator animator;
    SoundFile[] Sounds;
	[SerializeField]
    GameObject SoundPlayer;

	AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
		audioSource = SoundPlayer.GetComponent<AudioSource>();
        //SoundPlayer = Resources.Load<GameObject>("Sounds/Soundplayer");

        Sounds = new SoundFile[] {
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep1"), new string[]{ "Duckmaster", "Walking" }),
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep2"), new string[]{ "Duckmaster", "Walking" }),
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep3"), new string[]{ "Duckmaster", "Walking" }),
            new SoundFile(Resources.Load<AudioClip>("Sounds/Duckmaster/GrassStep4"), new string[]{ "Duckmaster", "Walking" }),
            };
    }

    private void OnEnable()
    {
        AnimationEventStuff.onDuckmasterWalkingChange += ChangeWalk;
        AnimationEventStuff.onThrow += StartThrow;
        AnimationEventStuff.onWhistle += Whistle;
    }

    void Unload()
    {
        AnimationEventStuff.onDuckmasterWalkingChange -= ChangeWalk;
        AnimationEventStuff.onThrow -= StartThrow;
        AnimationEventStuff.onWhistle -= Whistle;
    }

    private void OnDisable()
    {
        Unload();
    }

    void ChangeWalk(bool newWalk)
    {
        animator.SetBool("Walking", newWalk);
    }
    void StartThrow()
    {
        animator.SetTrigger("Throw");
    }
    void Whistle()
    {
        animator.SetTrigger("Whistle");
    }

    public void ContinueThrow()
    {
        GameManager.Instance.masterThrow();
    }
    public void ContinueRecall()
    {
        GameManager.Instance.masterRecall();
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
			// TO DO: Change so it's playing on a single object
            AudioClip ac = tempSounds[(int)Random.Range(0, tempSounds.Count)].GetClip();
			audioSource.clip = ac;
			audioSource.Play();
        }
    }

}
