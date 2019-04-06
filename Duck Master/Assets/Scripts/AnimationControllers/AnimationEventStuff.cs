using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventStuff : MonoBehaviour
{
    public delegate void CharacterEventHandler(bool newVal);

    public static event CharacterEventHandler onWalkingChange;

    public static void WalkingChange(bool newVal)
    {
        if (onWalkingChange != null)
            onWalkingChange(newVal);
    }


}
