using UnityEngine;

public class FrameInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public bool visible;

    public FrameInfo(Vector3 pos, Quaternion rot, bool v)
    {
        position = pos;
        rotation = rot;
        visible = v;
    }
}
