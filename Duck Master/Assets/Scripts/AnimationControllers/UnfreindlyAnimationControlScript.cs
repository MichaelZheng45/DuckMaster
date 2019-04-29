using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnfreindlyAnimationControlScript : MonoBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        AnimationEventStuff.onScare += Scare;
    }

    private void OnDisable()
    {
        AnimationEventStuff.onScare -= Scare;
    }

    void Scare()
    {
        animator.SetTrigger("Scare");
    }
}
