using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to make objects grabbable
/// </summary>
public abstract class IGrabbable : IAugmentable
{
    /// <summary>
    /// Invoke to grab object
    /// </summary>
    /// <param name="parent">Parent who is grabbing the object</param>
    public abstract void Grab(IGrabbing parent);
    /// <summary>
    /// Release the object
    /// </summary>
    public abstract void Release();
    /// <summary>
    /// Update object state according to parent;
    /// </summary>
    /// <param name="parent">Parent to get update from</param>
    public abstract void GetUpdate(IGrabbing parent);
    /// <summary>
    /// Get update according to potential parents who are in the vicinity of the object
    /// </summary>
    /// <param name="parent">Parent to get update from</param>
    public virtual void ProximityUpdate(IGrabbing parent)
    {

    }
}
