using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class WorldUpdater : MonoBehaviour
{
    private bool PreviouslyTracked = false;
    private TrackableBehaviour Marker;

    void Start()
    {
        Marker = GetComponent<TrackableBehaviour>();
    }
    void Update()
    {
        var augmentables = FindObjectsOfType<IAugmentable>();
        bool tracked = Marker.CurrentStatus == TrackableBehaviour.Status.TRACKED;
        foreach (var obj in augmentables)
        {
            obj.WorldUpdate(transform, tracked, PreviouslyTracked);
        }
        PreviouslyTracked = tracked;
    }
}