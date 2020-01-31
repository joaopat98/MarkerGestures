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

/// <summary>
/// Component used for gesture recognition (extend for custom behaviour)
/// </summary>
public abstract class GestureResolver : MonoBehaviour
{
    /// <summary>
    /// List of AR Marker objects (input in editor)
    /// </summary>
    public GameObject[] Markers;
    /// <summary>
    /// Number of framegroups each sample will be divided into (loaded from file)
    /// </summary>
    private int NumSamples = 60;
    /// <summary>
    /// Number of frames in a sample (loaded from file)
    /// </summary>
    private int SampleFrames = 1;
    public string csvFileName = "values.csv";
    /// <summary>
    /// Buffer for recorded frames of information about the markers
    /// </summary>
    private List<FrameInfoIntermediate>[] frames;

    private Forest forest;

    /// <summary>
    /// Gesture ID that was captured in the last frame
    /// </summary>
    private int capturedAction = -1;
    private float captureDuration = 0;
    /// <summary>
    /// Amount of a time the captured gesture IDs must be around a specific gesture ID for that gesture to have been made by the user
    /// </summary>
    public float gestureThreshold = 0.75f;
    /// <summary>
    /// Actions bound to each gesture
    /// </summary>
    /// <typeparam name="int">Id of the gesture</typeparam>
    /// <typeparam name="System.Action">Action to be invoked</typeparam>
    /// <returns></returns>
    Dictionary<int, System.Action> actions = new Dictionary<int, System.Action>();
    /// <summary>
    /// Array with visibility information on each AR Marker (same order as in <see cref="Markers"></see>)
    /// </summary>
    protected bool[] visibility;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        visibility = new bool[Markers.Length];
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
            TrainingDataPath = dataPath + "/" + csvFileName,
            MaxItemCountInCategory = 7,
            TreeCount = 50,
            SplitMode = SplitMode.GINI
        };
        // Method to "grow" the forest
        forest = (Forest)ForestFactory.Create();
        if (System.IO.File.GetLastWriteTime(dataPath + "/" + csvFileName) != settings.dateTime)
        {
            Debug.Log("Growing forest...");
            forest.Grow(p);
            forest.ImportFromJson(dataPath + "/trees");
            settings.dateTime = System.IO.File.GetLastWriteTime(dataPath + "/" + csvFileName);
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
    protected virtual void FixedUpdate()
    {
        for (int i = 0; i < Markers.Length; i++)
        {
            visibility[i] = Markers[i].GetComponent<TrackableBehaviour>().CurrentStatus == TrackableBehaviour.Status.TRACKED;
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
                    if (actions.ContainsKey(capturedAction))
                    {
                        actions[capturedAction]();
                        HalveFrameBuffers();
                    }
                    else
                    {
                        throw new ActionNotRegisteredException(capturedAction);
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
    }

    /// <summary>
    /// Register a method to be invoked when the gesture associated with <paramref name="gestureID"/> is performed
    /// </summary>
    /// <param name="gestureID">ID associated to the pretended gesture</param>
    /// <param name="action">Method to be invoked, must not receive any parameters</param>
    public void RegisterAction(int gestureID, System.Action action)
    {
        actions[gestureID] = action;
    }
}
