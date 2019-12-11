using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderGrabbable : IGrabbable
{
    private float xRange;
    public float Value;
    List<IFloatCallBack> callBacks = new List<IFloatCallBack>();
    public override void GetUpdate(GestureResolver parent)
    {
        var relHandPos = transform.parent.InverseTransformPoint(parent.GetTransform().position);
        var newPos = transform.localPosition;
        newPos.x = Mathf.Clamp(relHandPos.x, -xRange, xRange);
        transform.localPosition = newPos;
        Value = 1 - ((newPos.x + xRange) / (xRange * 2));
        CallBacks();
        Debug.Log("Slider Val: " + Value);
        if (Vector3.Distance(transform.position, parent.GetTransform().position) > parent.GrabRange)
        {
            parent.ForceRelease();
        }
    }

    void CallBacks()
    {
        foreach (var callBack in callBacks)
        {
            callBack.ValueChanged(Value);
        }
    }

    public void RegisterCallBack(IFloatCallBack callBack)
    {
        callBacks.Add(callBack);
    }

    public void RemoveCallBack(IFloatCallBack callBack)
    {
        callBacks.Remove(callBack);
    }

    public override void Grab(GestureResolver parent)
    {
    }

    public override void Release()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        xRange = Mathf.Abs(transform.localPosition.x);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
