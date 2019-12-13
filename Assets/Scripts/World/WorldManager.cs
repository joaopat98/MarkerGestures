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
    public MusicContainer MainContainer;
    private Vector3 mainContainerPos;
    private List<MusicContainer> musicContainers = new List<MusicContainer>();
    private string dataPath;
    private float t = 0;

    public bool testSwap = false;

    // Start is called before the first frame update
    void Start()
    {
        mainContainerPos = MainContainer.transform.parent.position;
        dataPath = Application.persistentDataPath + "/songs/";
        char[] sep = { '/' };
        musicContainers.Add(MainContainer);
        var songNames = Directory.GetDirectories(dataPath).Select<string,string>(s => s.Split(sep).Last()).ToArray();
        var firstCube = Instantiate(MusicCubePrefab, mainContainerPos + new Vector3(0, 13.5f, 0), Quaternion.identity, transform).GetComponent<MusicCube>();
        firstCube.songName = songNames[0];
        firstCube.Init();
        MainContainer.ReceiveCube(firstCube);
        for (int i = 1; i < songNames.Length; i++)
        {
            var container = Instantiate(
                ContainerPrefab,
                mainContainerPos + new Vector3(i * DistanceBetweenContainers, 4.4f, 0),
                Quaternion.identity,
                transform
                ).GetComponent<MusicContainer>();
            musicContainers.Add(container);
            var mCube = Instantiate(
                MusicCubePrefab,
                container.transform.position + new Vector3(0, -1f, 0),
                Quaternion.identity,
                transform
                ).GetComponent<MusicCube>();
            mCube.songName = songNames[i];
            mCube.Init();
            container.GetComponent<MusicContainer>().ReceiveCube(mCube);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(testSwap)
        {
            testSwap = false;
            MainContainer.audioSource.time = MainContainer.audioSource.clip.length - 5;
        }

        if (MainContainer.mCube && !MainContainer.audioSource.isPlaying && t > 2f)
        {
            var cubeList = new List<MusicCube>();
            for (int i = 1; i < musicContainers.Count; i++)
            {
                if (musicContainers[i].mCube)
                {
                    cubeList.Add(musicContainers[i].mCube);
                }
            }
            cubeList.Add(MainContainer.mCube);
            foreach (var container in musicContainers)
            {
                if (container.mCube)
                {
                    cubeList.Add(container.mCube);
                }
            }
            for (int i = 0; i < musicContainers.Count; i++)
            {
                if (i < cubeList.Count)
                    musicContainers[i].ReceiveCube(cubeList[i]);
                else
                    musicContainers[i].RemoveCube();
            }
            t = 0;
        }
        t += Time.deltaTime;
    }
}
