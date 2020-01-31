using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrabbable : IGrabbable
{
    private Vector3 offsetPos;
    private Quaternion offsetRot;
    protected Rigidbody rb;
    public bool WasKinematic;
    private GameObject halo;
    public bool Held;

    protected void Awake()
    {
        halo = transform.Find("Halo").gameObject;
        rb = GetComponent<Rigidbody>();
    }

    public override void GetUpdate(IGrabbing parent)
    {
        transform.position = parent.GetTransform().position + parent.GetTransform().TransformVector(offsetPos);
        transform.rotation = parent.GetTransform().rotation * offsetRot;
    }

    public override void Grab(IGrabbing parent)
    {
        rb.isKinematic = true;
        Held = true;
        offsetPos = parent.GetTransform().InverseTransformVector(transform.position - parent.GetTransform().position);
        offsetRot = Quaternion.Inverse(parent.GetTransform().rotation) * transform.rotation;
    }

    public override void Release()
    {
        Held = false;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void WorldUpdate(Transform WorldCenter, bool Tracked, bool PreviouslyTracked)
    {
        if (PreviouslyTracked && !Tracked)
        {
            WasKinematic = rb.isKinematic;
            rb.isKinematic = true;
        }
        if (!PreviouslyTracked && Tracked)
        {
            rb.isKinematic = WasKinematic;
        }
    }

    public override void ProximityUpdate(IGrabbing parent)
    {
        halo.SetActive(Vector3.Distance(parent.GetGrabPosition(), transform.position) < parent.GetGrabRange());
    }
}
