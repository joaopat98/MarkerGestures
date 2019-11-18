using RandomForest.Lib.Numerical;
using RandomForest.Lib.Numerical.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class ResolveGestures : MonoBehaviour
{
    public GameObject[] Markers;
    public Text Result;
    public int SampleFrames = 60;
    public string fileName = "values.csv";

    private List<FrameInfo>[] frames;
    private Quaternion[] iRot;
    private Vector3[] iPos;

    private Forest forest;

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
        string dataPath = Application.persistentDataPath;
        ForestGrowParameters p = new ForestGrowParameters
        {
            ExportDirectoryPath = dataPath + "/trees",
            ExportToJson = true,
            ResolutionFeatureName = "id",
            ItemSubsetCountRatio = 0.6f,
            TrainingDataPath = dataPath + "/" + fileName,
            MaxItemCountInCategory = 5,
            TreeCount = 4,
            SplitMode = SplitMode.GINI
        };
        // Method to "grow" the forest
        forest = (Forest)ForestFactory.Create();
        forest.Grow(p);
        forest.ImportFromJson(dataPath + "/trees");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Markers.Length; i++)
        {
            var obj = Markers[i].transform;
            var marker = Markers[i].GetComponent<TrackableBehaviour>();
            frames[i].Add(new FrameInfo(obj.position - iPos[i], obj.rotation * Quaternion.Inverse(iRot[i]),
                marker.CurrentStatus == TrackableBehaviour.Status.TRACKED));
        }
        if (frames[0].Count > SampleFrames)
        {
            for (int i = 0; i < Markers.Length; i++)
            {
                frames[i].RemoveAt(0);
            }
            IItemNumerical item = forest.CreateItem();
            for (int i = 0; i < SampleFrames; i++)
            {
                for (int j = 0; j < Markers.Length; j++)
                {
                    var frame = frames[j][i];
                    var obj = Markers[j];
                    item.SetValue(obj.name + "_visible_" + i, frame.visible ? 1 : 0);
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
            if (gesture != 1)
                Debug.Log("YEET");
            Result.text = gesture.ToString();
        }
    }

    public void GoToTrain()
    {
        SceneManager.LoadScene("Train");
    }
}
