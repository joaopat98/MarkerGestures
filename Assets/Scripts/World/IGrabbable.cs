using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    void Grab(Transform parent);
    void Release();
    void GetUpdate(Transform parent);
}
