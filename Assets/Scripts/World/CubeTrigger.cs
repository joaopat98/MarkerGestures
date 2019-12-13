using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTrigger : MonoBehaviour
{
    public Material DefaultMaterial, ActivatedMaterial;
    private MusicContainer musicContainer;
    private MusicCube competingCube;
    private void OnTriggerEnter(Collider other)
    {
        if (!musicContainer.mCube)
        {
            GetComponent<Renderer>().material = ActivatedMaterial;

            var mCube = other.GetComponent<MusicCube>();
            if (mCube)
            {
                if (!mCube.Held)
                    musicContainer.ReceiveCube(mCube);
                else
                    mCube.Released += OnCubeRelease;
            }
        }
        else
        {
            var mCube = other.GetComponent<MusicCube>();
            if (mCube)
            {
                Debug.Log("found competing");
                if (mCube != musicContainer.mCube)
                    competingCube = mCube;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<Renderer>().material = DefaultMaterial;
        var mCube = other.GetComponent<MusicCube>();
        if (mCube)
        {
            if (mCube == musicContainer.mCube)
            {
                musicContainer.RemoveCube();
                mCube.Released -= OnCubeRelease;
                Debug.Log("Removed Listener");
                if (competingCube)
                {
                    Debug.Log("receiving competing");
                    if (!competingCube.Held)
                        musicContainer.ReceiveCube(mCube);
                    else
                    {
                        GetComponent<Renderer>().material = ActivatedMaterial;
                        competingCube.Released += OnCubeRelease;
                    }
                }
            }
            else if (mCube == competingCube)
            {
                competingCube.Released -= OnCubeRelease;
                competingCube = null;
            }
        }
    }

    private void OnCubeRelease(MusicCube cube)
    {
        Debug.Log("get cube");
        musicContainer.ReceiveCube(cube);
    }

    // Start is called before the first frame update
    void Awake()
    {
        musicContainer = GetComponentInParent<MusicContainer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
