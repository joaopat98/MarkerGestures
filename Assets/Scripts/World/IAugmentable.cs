using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAugmentable : MonoBehaviour
{
    public virtual void WorldUpdate(Transform WorldCenter, bool Tracked, bool PreviouslyTracked)
    {

    }
}