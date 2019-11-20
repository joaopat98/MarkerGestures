using UnityEngine;

public class FrameInfoIntermediate
{
    public Vector3 position;
    public Vector3 forward;
    public float visible;

    public FrameInfoIntermediate(Vector3 pos, Vector3 f, float v)
    {
        position = pos;
        forward = f;
        visible = v;
    }
}

public class FrameInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public float visible;

    public FrameInfo(Vector3 pos, Quaternion rot, float v)
    {
        position = pos;
        rotation = rot;
        visible = v;
    }
}
