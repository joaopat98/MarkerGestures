using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to make objects swipable
/// </summary>
public abstract class ISwipable : IAugmentable
{
    /// <summary>
    /// Method to invoke when a left swipe is made in front of the object
    /// </summary>
    /// <param name="parent">Parent who is swipping</param>
    public abstract void SwipeLeft(ISwipping parent);
    /// <summary>
    /// Method to invoke when a right swipe is made in front of the object
    /// </summary>
    /// <param name="parent">Parent who is swipping</param>
    public abstract void SwipeRight(ISwipping parent);
}
