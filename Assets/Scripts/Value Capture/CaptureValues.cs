using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class CaptureValues : MonoBehaviour
{
    public GameObject[] Markers;
    public Text Timer;
    public int SampleFrames = 60;
    public int id = 1;
    public bool Overwrite = true;
    public int SecondsBeforeCapture = 3;
    public string fileName = "values.csv";
    public InputField idField;

    private List<FrameInfo>[] frames;
    private Quaternion[] iRot;
    private Vector3[] iPos;
    private bool capturing = false;
    private bool preparing = false;
    private bool hasCaptured = false;
    private float curSeconds = 0f;

    // Start is called before the first frame update
    void Start()
    {
        frames = new List<FrameInfo>[Markers.Length];
        iRot = new Quaternion[Markers.Length];
        iPos = new Vector3[Markers.Length];
        for (int i = 0; i < Markers.Length; i++)
        {
            frames[i] = new List<FrameInfo>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (preparing)
        {
            curSeconds -= Time.fixedDeltaTime;
            if (curSeconds <= 0)
            {
                preparing = false;
                capturing = true;
                Timer.text = 0.ToString();
                Timer.color = Color.red;
                for (int i = 0; i < Markers.Length; i++)
                {
                    var obj = Markers[i].transform;
                    iRot[i] = obj.rotation;
                    iPos[i] = obj.position;
                }
            }
            else
            {
                Timer.text = ((int)Mathf.Ceil(curSeconds)).ToString();
            }
        }
        if (capturing)
        {
            for (int i = 0; i < Markers.Length; i++)
            {
                var obj = Markers[i].transform;
                var marker = Markers[i].GetComponent<TrackableBehaviour>();
                frames[i].Add(new FrameInfo(obj.position - iPos[i], obj.rotation * Quaternion.Inverse(iRot[i]),
                    marker.CurrentStatus == TrackableBehaviour.Status.TRACKED));
            }
            if (frames[0].Count == SampleFrames)
            {
                capturing = false;
                Timer.color = Color.black;
                WriteToSheet();
                for (int i = 0; i < Markers.Length; i++)
                {
                    frames[i] = new List<FrameInfo>();
                }
            }
        }
    }

    string Header(int offset)
    {
        string str = "";
        for (int i = 0; i < Markers.Length; i++)
        {
            var obj = Markers[i];
            str += obj.name + "_visible_" + offset + ",";
            str += obj.name + "_px_" + offset + ",";
            str += obj.name + "_py_" + offset + ",";
            str += obj.name + "_pz_" + offset + ",";
            str += obj.name + "_rx_" + offset + ",";
            str += obj.name + "_ry_" + offset + ",";
            str += obj.name + "_rz_" + offset + ",";
            str += obj.name + "_rw_" + offset + (i == Markers.Length - 1 ? "" : ",");
        }
        return str;
    }

    public void PrepareCapture()
    {
        if (Overwrite && !hasCaptured)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(Application.persistentDataPath + "/" + fileName, !Overwrite))
            {
                for (int i = 0; i < SampleFrames; i++)
                {
                    file.Write(Header(i) + (i == SampleFrames - 1 ? ",id" : ","));
                }
            }
            hasCaptured = true;
        }
        preparing = true;
        curSeconds = SecondsBeforeCapture;
    }

    void WriteToSheet()
    {
        using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(Application.persistentDataPath + "/" + fileName, true))
        {
            file.Write("\n");
            for (int i = 0; i < SampleFrames; i++)
            {
                for (int j = 0; j < Markers.Length; j++)
                {
                    var frame = frames[j][i];
                    var pos = frame.position;
                    var rot = frame.rotation;
                    file.Write(pos.x + "," + pos.y + "," + pos.z + "," + rot.x + "," + rot.y + "," + rot.z + "," + rot.w + "," + (frame.visible ? 1 : 0) +
                        (j == Markers.Length - 1 && i == SampleFrames - 1 ? "," + id : ","));
                }
            }
        }
    }

    public void setOverwrite(bool value)
    {
        Overwrite = value;
    }

    public void setId()
    {
        id = int.Parse(idField.text);
    }


    public void GoToTest()
    {
        SceneManager.LoadScene("Test");
    }
}
