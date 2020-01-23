using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISwipable : IAugmentable
{
    public abstract void SwipeLeft(GestureResolverExample parent);
    public abstract void SwipeRight(GestureResolverExample parent);
}
