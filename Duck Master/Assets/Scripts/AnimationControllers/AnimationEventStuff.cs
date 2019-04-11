using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventStuff : MonoBehaviour
{
    public delegate void CharacterEventHandlerBool(bool newVal);
    public delegate void CharacterEventHandler();

    public static event CharacterEventHandlerBool onWalkingChange;
    public static event CharacterEventHandlerBool onThrowing;

    public static void WalkingChange(bool newVal)
    {
        if (onWalkingChange != null)
            onWalkingChange(newVal);
    }

    public static void Throwing(bool newVal)
    {
        if (onThrowing != null)
            onThrowing(newVal);
    }

}
