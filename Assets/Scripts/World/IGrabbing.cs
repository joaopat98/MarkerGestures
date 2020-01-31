using UnityEngine;

/// <summary>
/// Implement this interface to allow for the object to interact with grabbable objects
/// </summary>
public interface IGrabbing
{
    /// <summary>
    /// Get position of the object grabbing center
    /// </summary>
    /// <returns></returns>
    Vector3 GetGrabPosition();
    /// <summary>
    /// Get Transform of objects anchor
    /// </summary>
    /// <returns></returns>
    Transform GetTransform();
    /// <summary>
    /// Get distance within which object can grab other objects
    /// </summary>
    /// <returns></returns>
    float GetGrabRange();
    /// <summary>
    /// Force the release of any grabbed objects held by this object
    /// </summary>
    void ForceRelease();
}