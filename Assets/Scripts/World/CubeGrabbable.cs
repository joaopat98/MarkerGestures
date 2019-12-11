using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrabbable : IGrabbable
{
    private Vector3 offset;
    private Rigidbody rb;
    public bool WasKinematic;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void GetUpdate(GestureResolver parent)
    {
        transform.position = parent.GetTransform().position + offset;
    }

    public override void Grab(GestureResolver parent)
    {
        rb.isKinematic = true;
        offset = transform.position - parent.GetTransform().position;
    }

    public override void Release()
    {
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
}
