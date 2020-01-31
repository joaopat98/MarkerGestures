using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleGrabbable : IGrabbable
{
    public Transform HandleCenter;
    public Transform Handle;

    public float Value;
    public override void GetUpdate(IGrabbing parent)
    {
        var relHandPos = HandleCenter.InverseTransformPoint(parent.GetGrabPosition());
        var rot = new Vector3(Mathf.Clamp(-Mathf.Rad2Deg * Mathf.Atan2(relHandPos.y, relHandPos.z), -90, 90), 0, 0);
        Handle.transform.localEulerAngles = rot;
        Value = (-rot.x + 90) / 180;
        if (Vector3.Distance(transform.position, parent.GetGrabPosition()) > parent.GetGrabRange())
        {
            parent.ForceRelease();
        }
    }

    public override void Grab(IGrabbing parent)
    {
    }

    public override void Release()
    {
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
