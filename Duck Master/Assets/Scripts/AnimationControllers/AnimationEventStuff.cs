using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventStuff : MonoBehaviour
{
    public delegate void EventHandlerBool(bool newVal);
    public delegate void GenericEventHandler();



    public static event EventHandlerBool onDuckmasterWalkingChange;

    public static event EventHandlerBool onDuckWalkingChange;
    public static event EventHandlerBool onDuckHeldChange;
    public static event EventHandlerBool onDuckInAirChange;

    public static event GenericEventHandler onPickup;
    public static event GenericEventHandler onThrow;
    public static event GenericEventHandler onWhistle;
    public static event GenericEventHandler unloadScene;
    public static event GenericEventHandler onScare;


    public static void UnloadScene()
    {
        if (unloadScene != null)
            unloadScene();
    }

    public static void DuckmasterWalkingChange(bool newVal)
    {
        if (onDuckmasterWalkingChange != null)
            onDuckmasterWalkingChange(newVal);
    }

    public static void DuckWalkingChange(bool newVal)
    {
        if (onDuckWalkingChange != null)
            onDuckWalkingChange(newVal);
    }
    public static void DuckHeldChange(bool newVal)
    {
        if (onDuckHeldChange != null)
            onDuckHeldChange(newVal);
    }
    public static void DuckInAirChange(bool newVal)
    {
        if (onDuckInAirChange != null)
            onDuckInAirChange(newVal);
    }

    public static void Scare()
    {
        if (onScare != null)
            onScare();
    }

    public static void Whistle()
    {
        if (onWhistle != null)
            onWhistle();
    }
    public static void Throw()
    {
        if (onThrow != null)
            onThrow();
    }
    public static void Pickup()
    {
        if (onPickup != null)
            onPickup();
    }

}
