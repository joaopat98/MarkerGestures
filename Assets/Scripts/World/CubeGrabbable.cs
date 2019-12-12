using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrabbable : IGrabbable
{
    private Vector3 offset;
    protected Rigidbody rb;
    public bool WasKinematic;
    private Transform halo;

    protected void Start()
    {
        halo = transform.Find("Halo");
        rb = GetComponent<Rigidbody>();
    }

    public override void GetUpdate(GestureResolver parent)
    {
        transform.position = parent.GetPosition() + offset;
    }

    public override void Grab(GestureResolver parent)
    {
        rb.isKinematic = true;
        offset = transform.position - parent.GetPosition();
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
            try
            {
                rb.isKinematic = WasKinematic;
            }
            catch (System.NullReferenceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public override void ProximityUpdate(GestureResolver parent)
    {
        halo.gameObject.SetActive(Vector3.Distance(parent.GetPosition(), transform.position) < parent.GrabRange);
    }
}
