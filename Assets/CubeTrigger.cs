using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var mCube = other.GetComponent<MusicCube>();
        if (mCube)
        {
            mCube.Released += OnCubeRelease;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var mCube = other.GetComponent<MusicCube>();
        if (mCube)
        {
            mCube.Released -= OnCubeRelease;
        }
    }

    private void OnCubeRelease(MusicCube cube)
    {
        transform.parent.GetComponent<MusicContainer>().ReceiveCube(cube);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
