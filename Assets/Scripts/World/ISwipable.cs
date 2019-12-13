using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISwipable : IAugmentable
{
    public abstract void SwipeLeft(GestureResolver parent);
    public abstract void SwipeRight(GestureResolver parent);
}
