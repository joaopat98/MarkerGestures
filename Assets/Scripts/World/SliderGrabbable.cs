using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderGrabbable : IGrabbable
{
    private float xRange;
    public float Value;
    List<IFloatCallBack> callBacks = new List<IFloatCallBack>();
    private GameObject halo;

    protected void Awake()
    {
        halo = transform.Find("Halo").gameObject;
    }

    public override void GetUpdate(GestureResolverExample parent)
    {
        var relHandPos = transform.parent.InverseTransformPoint(parent.GetPosition());
        var newPos = transform.localPosition;
        newPos.x = Mathf.Clamp(relHandPos.x, -xRange, xRange);
        transform.localPosition = newPos;
        Value = 1 - ((newPos.x + xRange) / (xRange * 2));
        CallBacks();
        if (Vector3.Distance(transform.position, parent.GetPosition()) > parent.GrabRange)
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

    public override void Grab(GestureResolverExample parent)
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

    public override void ProximityUpdate(GestureResolverExample parent)
    {
        halo.SetActive(Vector3.Distance(parent.GetPosition(), transform.position) < parent.GrabRange);
    }
}
