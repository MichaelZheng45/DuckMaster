using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventStuff : MonoBehaviour
{
    public delegate void DuckmasterEventHandlerBool(bool newVal);
    public delegate void DuckEventHandlerBool(bool newVal);
    public delegate void GenericEventHandler();



    public static event DuckmasterEventHandlerBool onDuckmasterWalkingChange;
    public static event DuckmasterEventHandlerBool onDuckWalkingChange;
    public static event GenericEventHandler onThrow;
    public static event GenericEventHandler unloadScene;


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


    public static void DuckmasterThrowing()
    {
        if (onThrow != null)
            onThrow();
    }

}
