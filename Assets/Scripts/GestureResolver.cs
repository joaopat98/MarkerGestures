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

public class GestureResolver : MonoBehaviour
{
    public GameObject[] Markers;
    public int NumSamples = 60;
    public int SampleFrames = 1;
    public string fileName = "values.csv";

    public Material BlinkMaterial, BadMaterial, GoodMaterial;
    public GameObject HandOpen, HandClosed;

    private List<FrameInfoIntermediate>[] frames;

    private Forest forest;
    private bool showingGesture = false;


    private int capturedAction = -1;
    private float captureDuration = 0;
    public float gestureThreshold = 0.75f;

    public float GrabRange = 5f;
    public float SwipeRange = 20f;

    private IGrabbable Held;

    public Transform GrabCenter;

    // Start is called before the first frame update
    void Start()
    {
        HandClosed.SetActive(false);
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
    void FixedUpdate()
    {
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
        for (int i = 0; i < Markers.Length; i++)
        {
            var obj = Markers[i].transform;
            var marker = Markers[i].GetComponent<TrackableBehaviour>();
            frames[i].Add(new FrameInfoIntermediate(Camera.main.transform.InverseTransformPoint(obj.position), Camera.main.transform.InverseTransformDirection(obj.forward),
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
                    int visible = 0;
                    visible += Markers[0].GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED ? 1 : 0;
                    visible += Markers[1].GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED ? 1 : 0;
                    switch (capturedAction)
                    {
                        case 1:
                            if (visible > 0)
                            {
                                OnHandClose();
                                HalveFrameBuffers();
                            }
                            break;
                        case 2:
                            if (visible > 1)
                            {
                                OnHandOpen();
                                HalveFrameBuffers();
                            }
                            break;
                        case 3:
                            if (visible > 0)
                            {
                                OnSwipeRight();
                                HalveFrameBuffers();
                            }
                            break;
                        case 4:
                            if (visible > 0)
                            {
                                OnSwipeLeft();
                                HalveFrameBuffers();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                capturedAction = gestureRound;
                captureDuration = 0;
            }
            for (int i = 0; i < Markers.Length; i++)
            {
                frames[i].RemoveAt(0);
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

    void OnHandClose()
    {
        HandOpen.SetActive(false);
        HandClosed.SetActive(true);
        StartCoroutine(BlinkMarkers());
        var candidates = FindObjectsOfType<IGrabbable>();
        IGrabbable closest = null;
        float dist = Mathf.Infinity;
        foreach (var candidate in candidates)
        {
            float curDist = Vector3.Distance(GetPosition(), candidate.transform.position);
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

    void OnHandOpen()
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

    void OnSwipeRight()
    {
        StartCoroutine(BlinkMarkers());
        var candidates = FindObjectsOfType<ISwipable>();
        ISwipable closest = null;
        float dist = Mathf.Infinity;
        foreach (var candidate in candidates)
        {
            float curDist = Vector3.Distance(GetPosition(), candidate.transform.position);
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

    void OnSwipeLeft()
    {
        StartCoroutine(BlinkMarkers());
        var candidates = FindObjectsOfType<ISwipable>();
        ISwipable closest = null;
        float dist = Mathf.Infinity;
        foreach (var candidate in candidates)
        {
            float curDist = Vector3.Distance(GetPosition(), candidate.transform.position);
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

    public Vector3 GetPosition()
    {
        return GrabCenter.position;
    }

    public void ForceRelease()
    {
        if (Held)
        {
            Held.Release();
            Held = null;
        }
    }
}
