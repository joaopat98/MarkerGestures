using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for objects that can be augmented into the scene
/// </summary>
public abstract class IAugmentable : MonoBehaviour
{
    /// <summary>
    /// Update the augmentable object with the state of the world tracking (to deactivate/stop physics when world is not tracked for example)
    /// </summary>
    /// <param name="WorldCenter">Transform by which the world is tracked</param>
    /// <param name="Tracked">true if the world is being tracked in the current frame</param>
    /// <param name="PreviouslyTracked">true if the world was being tracked in the previous frame</param>
    public virtual void WorldUpdate(Transform WorldCenter, bool Tracked, bool PreviouslyTracked)
    {

    }
}