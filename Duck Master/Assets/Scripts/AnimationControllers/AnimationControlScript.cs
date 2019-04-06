using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationControlScript : MonoBehaviour
{
    Animator animator;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
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


}
