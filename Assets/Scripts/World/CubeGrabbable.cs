using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrabbable : IGrabbable
{
    private Vector3 offset;
    protected Rigidbody rb;
    public bool WasKinematic;
    private GameObject halo;
    public bool Held;

    protected void Awake()
    {
        halo = transform.Find("Halo").gameObject;
        rb = GetComponent<Rigidbody>();
    }

    public override void GetUpdate(GestureResolver parent)
    {
        transform.position = parent.GetPosition() + offset;
    }

    public override void Grab(GestureResolver parent)
    {
        rb.isKinematic = true;
        Held = true;
        offset = transform.position - parent.GetPosition();
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

    public override void ProximityUpdate(GestureResolver parent)
    {
        halo.SetActive(Vector3.Distance(parent.GetPosition(), transform.position) < parent.GrabRange);
    }
}
