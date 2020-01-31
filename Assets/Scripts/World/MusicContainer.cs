using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicContainer : MonoBehaviour
{
    public bool IsMain = false;
    public AudioSource audioSource;
    public MusicCube mCube;

    // Start is called before the first frame update

    public void ReceiveCube(MusicCube cube)
    {
        mCube = cube;
        mCube.transform.position = transform.position + new Vector3(0, -1f, 0);
        mCube.transform.rotation = Quaternion.identity;
        if (IsMain)
        {
            if (cube.audioClip)
                PlaySong(cube.audioClip);
            else
            {
                cube.importer.Loaded += PlaySong;
            }
        }
    }

    public void RemoveCube()
    {
        if (IsMain)
        {
            audioSource.Stop();
        }
        mCube = null;
    }

    void PlaySong(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
        mCube.importer.Loaded -= PlaySong;
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
