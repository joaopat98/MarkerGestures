using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGrabbable : IAugmentable
{
    public abstract void Grab(GestureResolver parent);
    public abstract void Release();
    public abstract void GetUpdate(GestureResolver parent);
}
