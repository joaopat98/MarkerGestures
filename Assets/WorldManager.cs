using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    public float DistanceBetweenContainers = 10f;
    public GameObject ContainerPrefab;
    public GameObject MusicCubePrefab;
    public Transform MainContainer;
    private List<MusicCube> musicCubes = new List<MusicCube>();
    private string dataPath;

    // Start is called before the first frame update
    void Start()
    {
        dataPath = Application.persistentDataPath + "/songs/";
        char[] sep = { '/' };
        var songNames = Directory.GetDirectories(dataPath).Select<string,string>(s => s.Split(sep).Last()).ToArray();
        var firstCube = Instantiate(MusicCubePrefab, MainContainer.position + new Vector3(0, 13.5f, 0), Quaternion.identity, transform);
        musicCubes.Add(firstCube.GetComponent<MusicCube>());
        musicCubes[0].songName = songNames[0];
        musicCubes[0].Init();
        MainContainer.GetComponentInChildren<MusicContainer>().ReceiveCube(musicCubes[0]);
        for (int i = 1; i < songNames.Length; i++)
        {
            var container = Instantiate(
                ContainerPrefab,
                MainContainer.position + new Vector3(i * DistanceBetweenContainers, 4.4f, 0),
                Quaternion.identity,
                transform
                );
            var mCube = Instantiate(
                MusicCubePrefab,
                MainContainer.position + new Vector3(i * DistanceBetweenContainers, 3.4f, 0),
                Quaternion.identity,
                transform
                );
            musicCubes.Add(mCube.GetComponent<MusicCube>());
            musicCubes[i].songName = songNames[i];
            musicCubes[i].Init();
            container.GetComponent<MusicContainer>().ReceiveCube(musicCubes[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
