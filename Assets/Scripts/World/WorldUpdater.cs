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
        var vuforia = VuforiaARController.Instance;
        vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        vuforia.RegisterOnPauseCallback(OnPaused);
    }

    private void OnVuforiaStarted()
    {
        CameraDevice.Instance.SetFocusMode(
            CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnPaused(bool paused)
    {
        if (!paused) // resumed
        {
            // Set again autofocus mode when app is resumed
            CameraDevice.Instance.SetFocusMode(
               CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
    }
    void Update()
    {
        var augmentables = FindObjectsOfType<IAugmentable>();
        bool tracked = Marker.CurrentStatus == TrackableBehaviour.Status.TRACKED && Marker.CurrentStatus == TrackableBehaviour.Status.EXTENDED_TRACKED;
        foreach (var obj in augmentables)
        {
            obj.WorldUpdate(transform, tracked, PreviouslyTracked);
        }
        PreviouslyTracked = tracked;
    }
}