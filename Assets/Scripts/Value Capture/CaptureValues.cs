using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;
using Vuforia;

public class CaptureValues : MonoBehaviour
{
    public GameObject[] Markers;
    public Text Timer;
    public int NumSamples = 60;
    public int SampleFrames = 1;
    public int id = 1;
    public int SecondsBeforeCapture = 3;
    public string fileName = "values.csv";
    public InputField idField;
    public InputField numSamplesField;
    public InputField sampleFramesField;

    private List<FrameInfoIntermediate>[] frames;
    private bool capturing = false;
    private bool preparing = false;
    private bool hasCaptured = false;
    private float curSeconds = 0f;

    // Start is called before the first frame update
    void Start()
    {
        frames = new List<FrameInfoIntermediate>[Markers.Length];
        for (int i = 0; i < Markers.Length; i++)
        {
            frames[i] = new List<FrameInfoIntermediate>();
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
                frames[i].Add(new FrameInfoIntermediate(obj.position, obj.forward,
                    marker.CurrentStatus == TrackableBehaviour.Status.TRACKED ? 1 : 0));
            }
            if (frames[0].Count > NumSamples * SampleFrames)
            {
                List<FrameInfo>[] avgFrames = new List<FrameInfo>[Markers.Length];
                for (int i = 0; i < Markers.Length; i++)
                {
                    avgFrames[i] = new List<FrameInfo>();
                    for (int j = NumSamples * SampleFrames; j >= SampleFrames; j -= SampleFrames)
                    {
                        var before = frames[i][j - SampleFrames];
                        var after = frames[i][j];
                        var avg = new FrameInfo(after.position - before.position, Quaternion.FromToRotation(before.forward, after.forward), 0);
                        for (int k = j - SampleFrames + 1; k <= j; k++)
                        {
                            avg.visible += frames[i][k].visible;
                        }
                        avg.visible /= SampleFrames;
                        avgFrames[i].Insert(0, avg);
                    }
                }
                capturing = false;
                Timer.color = Color.black;
                WriteToSheet(avgFrames);
                for (int i = 0; i < Markers.Length; i++)
                {
                    frames[i] = new List<FrameInfoIntermediate>();
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

    public void Overwrite()
    {
        Settings settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        settings.NumSamples = NumSamples;
        settings.SampleFrames = SampleFrames;
        System.IO.File.WriteAllText(Application.persistentDataPath + "/settings.json", JsonConvert.SerializeObject(settings));
        using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(Application.persistentDataPath + "/" + fileName, false))
        {
            for (int i = 0; i < NumSamples; i++)
            {
                file.Write(Header(i) + (i == NumSamples - 1 ? ",id" : ","));
            }
        }
    }

    public void PrepareCapture()
    {
        preparing = true;
        curSeconds = SecondsBeforeCapture;
    }

    void WriteToSheet(List<FrameInfo>[] frames)
    {
        using (System.IO.StreamWriter file =
         new System.IO.StreamWriter(Application.persistentDataPath + "/" + fileName, true))
        {
            file.Write("\n");
            for (int i = 0; i < NumSamples; i++)
            {
                for (int j = 0; j < Markers.Length; j++)
                {
                    var frame = frames[j][i];
                    var pos = frame.position;
                    var rot = frame.rotation;
                    file.Write(frame.visible + "," + pos.x + "," + pos.y + "," + pos.z + "," + rot.x + "," + rot.y + "," + rot.z + "," + rot.w +
                        (j == Markers.Length - 1 && i == NumSamples - 1 ? "," + id : ","));
                }
            }
        }
    }

    public void setId()
    {
        id = int.Parse(idField.text);
    }

    public void setNumSamples()
    {
        NumSamples = int.Parse(numSamplesField.text);
    }

    public void setSampleFrames()
    {
        SampleFrames = int.Parse(sampleFramesField.text);
    }

    public void GoToTest()
    {
        SceneManager.LoadScene("Test");
    }
}
