using RandomForest.Lib.Numerical;
using RandomForest.Lib.Numerical.Interfaces;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;
using Newtonsoft.Json;
using System.Linq;

public class GestureResolverExample : GestureResolver, IGrabbing, ISwipping
{
    [Header("Example Settings")]
    public Transform GrabCenter;
    public Material BlinkMaterial, BadMaterial, GoodMaterial;
    public GameObject HandOpen, HandClosed;
    private bool showingGesture = false;


    public float GrabRange = 5f;
    public float SwipeRange = 20f;

    private IGrabbable Held;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        HandClosed.SetActive(false);
        RegisterAction(0, Idle);
        RegisterAction(1, OnHandClose);
        RegisterAction(2, OnHandOpen);
        RegisterAction(3, OnSwipeRight);
        RegisterAction(4, OnSwipeLeft);
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!showingGesture)
        {
            if (Markers[1].GetComponent<TrackableBehaviour>().CurrentStatus != TrackableBehaviour.Status.TRACKED)
            {
                SetMaterial(BadMaterial);
            }
            else
            {
                SetMaterial(GoodMaterial);
            }
        }

        if (Held)
        {
            Held.GetUpdate(this);
        }

        foreach (var grabbable in FindObjectsOfType<IGrabbable>())
        {
            grabbable.ProximityUpdate(this);
        }
    }

    public void GoToTrain()
    {
        SceneManager.LoadScene("Train");
    }

    void SetMaterial(Material material)
    {
        HandClosed.GetComponent<Renderer>().material = material;
        HandOpen.GetComponent<Renderer>().material = material;
    }

    public IEnumerator BlinkMarkers()
    {
        showingGesture = true;
        SetMaterial(BlinkMaterial);
        yield return new WaitForSeconds(0.5f);
        showingGesture = false;
    }

    void Idle()
    {

    }

    void OnHandClose()
    {
        if (visibility[0])
        {
            HandOpen.SetActive(false);
            HandClosed.SetActive(true);
            StartCoroutine(BlinkMarkers());
            var candidates = FindObjectsOfType<IGrabbable>();
            IGrabbable closest = null;
            float dist = Mathf.Infinity;
            foreach (var candidate in candidates)
            {
                float curDist = Vector3.Distance(GetGrabPosition(), candidate.transform.position);
                if (curDist < GrabRange && curDist < dist)
                {
                    closest = candidate;
                    dist = curDist;
                }
            }
            if (closest)
            {
                Held = closest;
                closest.Grab(this);
            }
        }
    }

    void OnHandOpen()
    {
        if (visibility[0] && visibility[1])
        {
            HandOpen.SetActive(true);
            HandClosed.SetActive(false);
            StartCoroutine(BlinkMarkers());
            if (Held)
            {
                Held.Release();
                Held = null;
            }
        }
    }

    void OnSwipeRight()
    {
        if (visibility[0])
        {
            StartCoroutine(BlinkMarkers());
            var candidates = FindObjectsOfType<ISwipable>();
            ISwipable closest = null;
            float dist = Mathf.Infinity;
            foreach (var candidate in candidates)
            {
                float curDist = Vector3.Distance(GetGrabPosition(), candidate.transform.position);
                if (curDist < SwipeRange && curDist < dist)
                {
                    closest = candidate;
                    dist = curDist;
                }
            }
            if (closest)
            {
                closest.SwipeRight(this);
            }
        }
    }

    void OnSwipeLeft()
    {
        StartCoroutine(BlinkMarkers());
        var candidates = FindObjectsOfType<ISwipable>();
        ISwipable closest = null;
        float dist = Mathf.Infinity;
        foreach (var candidate in candidates)
        {
            float curDist = Vector3.Distance(GetGrabPosition(), candidate.transform.position);
            if (curDist < SwipeRange && curDist < dist)
            {
                closest = candidate;
                dist = curDist;
            }
        }
        if (closest)
        {
            closest.SwipeLeft(this);
        }

    }

    public Vector3 GetGrabPosition()
    {
        return GrabCenter.position;
    }

    public Transform GetTransform()
    {
        return Markers[0].transform;
    }

    public void ForceRelease()
    {
        if (Held)
        {
            Held.Release();
            Held = null;
        }
    }

    public float GetGrabRange()
    {
        return GrabRange;
    }
}
