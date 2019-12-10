using RandomForest.Lib.Numerical;
using RandomForest.Lib.Numerical.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;
using Newtonsoft.Json;

public class ResolveGestures : MonoBehaviour
{
    public GameObject[] Markers;
    public Transform camera;
    public Text Result;
    public Text Action;
    public int NumSamples = 60;
    public int SampleFrames = 1;
    public string fileName = "values.csv";
    public GameObject HoldCube;
    public Material HeldMaterial;
    public Material ReleaseMaterial;
    public TrackableBehaviour WorldCenter;
    private bool cubeHeld = false;
    private Vector3 offset;
    private Quaternion offsetQ;

    private List<FrameInfoIntermediate>[] frames;

    private Forest forest;

    public float alpha = 0;
    public float disappearSpeed = 0.01f;


    private int capturedAction = -1;
    private float captureDuration = 0;
    public float gestureThreshold = 0.75f;

    public float GrabRange = 5f;

    // Start is called before the first frame update
    void Start()
    {
        var settings = JsonConvert.DeserializeObject<Settings>(System.IO.File.ReadAllText(Application.persistentDataPath + "/settings.json"));
        NumSamples = settings.NumSamples;
        SampleFrames = settings.SampleFrames;
        frames = new List<FrameInfoIntermediate>[Markers.Length];
        for (int i = 0; i < Markers.Length; i++)
        {
            frames[i] = new List<FrameInfoIntermediate>();
        }
        string dataPath = Application.persistentDataPath;
        ForestGrowParameters p = new ForestGrowParameters
        {
            ExportDirectoryPath = dataPath + "/trees",
            ExportToJson = true,
            ResolutionFeatureName = "id",
            ItemSubsetCountRatio = 0.6f,
            TrainingDataPath = dataPath + "/" + fileName,
            MaxItemCountInCategory = 7,
            TreeCount = 50,
            SplitMode = SplitMode.GINI
        };
        // Method to "grow" the forest
        forest = (Forest)ForestFactory.Create();
        if (System.IO.File.GetLastWriteTime(dataPath + "/" + fileName) != settings.dateTime)
        {
            Debug.Log("Growing forest...");
            forest.Grow(p);
            forest.ImportFromJson(dataPath + "/trees");
            settings.dateTime = System.IO.File.GetLastWriteTime(dataPath + "/" + fileName);
            System.IO.File.WriteAllText(dataPath + "/settings.json", JsonConvert.SerializeObject(settings));
        }
        else
        {
            Debug.Log("Importing forest from file...");
            forest.InitializeItemSet(p.TrainingDataPath);
            forest.ImportFromJson(dataPath + "/trees");
        }
    }

    void HalveFrameBuffers()
    {
        for (int i = 0; i < Markers.Length; i++)
        {
            frames[i].RemoveRange(0, SampleFrames / 2);
        }
    }

    int RoundIfClose(float val)
    {
        var round = Mathf.RoundToInt(val);
        if (Mathf.Abs(round - val) <= 0.3f)
        {
            return round;
        }
        else
        {
            return 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (alpha > 0)
        {
            var color = Action.color;
            color.a = alpha;
            Action.color = color;
            alpha -= disappearSpeed * Time.deltaTime;
        }
        for (int i = 0; i < Markers.Length; i++)
        {
            var obj = Markers[i].transform;
            var marker = Markers[i].GetComponent<TrackableBehaviour>();
            frames[i].Add(new FrameInfoIntermediate(camera.InverseTransformPoint(obj.position), camera.InverseTransformDirection(obj.forward),
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
            IItemNumerical item = forest.CreateItem();
            for (int i = 0; i < NumSamples; i++)
            {
                for (int j = 0; j < Markers.Length; j++)
                {
                    var frame = avgFrames[j][i];
                    var obj = Markers[j];
                    item.SetValue(obj.name + "_visible_" + i, frame.visible);
                    item.SetValue(obj.name + "_px_" + i, frame.position.x);
                    item.SetValue(obj.name + "_py_" + i, frame.position.y);
                    item.SetValue(obj.name + "_pz_" + i, frame.position.z);
                    item.SetValue(obj.name + "_rx_" + i, frame.rotation.x);
                    item.SetValue(obj.name + "_ry_" + i, frame.rotation.y);
                    item.SetValue(obj.name + "_rz_" + i, frame.rotation.z);
                    item.SetValue(obj.name + "_rw_" + i, frame.rotation.w);
                }
            }
            double gesture = forest.Resolve(item);
            int gestureRound = RoundIfClose((float)gesture);
            if (gestureRound == capturedAction)
            {
                captureDuration += Time.deltaTime;
                if (captureDuration >= gestureThreshold)
                {
                    captureDuration = 0;
                    bool visible = true;
                    foreach (var marker in Markers)
                    {
                        if (marker.GetComponent<TrackableBehaviour>().CurrentStatus != TrackableBehaviour.Status.TRACKED)
                        {
                            visible = false;
                            break;
                        }
                    }
                    if (visible)
                    {
                        switch (capturedAction)
                        {
                            case 1:
                                Message("Hand close");
                                OnHandClose();
                                HalveFrameBuffers();
                                break;
                            case 2:
                                Message("Hand Open");
                                OnHandOpen();
                                HalveFrameBuffers();
                                break;
                            case 3:
                                Message("Move Right");
                                HalveFrameBuffers();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                capturedAction = gestureRound;
                captureDuration = 0;
            }
            Result.text = gesture.ToString();
            for (int i = 0; i < Markers.Length; i++)
            {
                frames[i].RemoveAt(0);
            }
        }
        if (cubeHeld)
        {
            HoldCube.transform.position = Markers[0].transform.position + offset;
            HoldCube.transform.rotation = offsetQ * Markers[0].transform.rotation;

        }
        if (WorldCenter.CurrentStatus != TrackableBehaviour.Status.TRACKED)
        {
            HoldCube.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            HoldCube.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void GoToTrain()
    {
        SceneManager.LoadScene("Train");
    }

    void Message(string msg)
    {
        alpha = 1;
        Action.text = msg;
    }

    public void TestMessage()
    {
        Message("oof");
    }

    void OnHandClose()
    {
        offset = HoldCube.transform.position - Markers[0].transform.position;
        if (offset.magnitude < GrabRange)
        {
            cubeHeld = true;
            HoldCube.GetComponent<Rigidbody>().isKinematic = true;
            HoldCube.GetComponent<Renderer>().material = HeldMaterial;
            offsetQ = Quaternion.FromToRotation(Markers[0].transform.forward, HoldCube.transform.forward);
        }

    }

    void OnHandOpen()
    {
        HoldCube.GetComponent<Rigidbody>().velocity = Vector3.zero;
        HoldCube.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        HoldCube.GetComponent<Rigidbody>().isKinematic = false;
        cubeHeld = false;
        HoldCube.GetComponent<Renderer>().material = ReleaseMaterial;
    }

    void OnSwipeRight()
    {

    }
}
