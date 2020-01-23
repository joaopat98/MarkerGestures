using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGrabbable : IAugmentable
{
    public abstract void Grab(GestureResolverExample parent);
    public abstract void Release();
    public abstract void GetUpdate(GestureResolverExample parent);

    public virtual void ProximityUpdate(GestureResolverExample parent)
    {

    }
}
