using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicContainer : MonoBehaviour
{
    public bool IsMain = false;
    private AudioSource audioSource;
    private MusicCube mCube;

    // Start is called before the first frame update

    public void ReceiveCube(MusicCube cube)
    {
        mCube = cube;
        if(IsMain)
        {
            if (cube.audioClip)
                PlaySong(cube.audioClip);
            else {
                cube.importer.Loaded += PlaySong;
            }
        }
    }

    void PlaySong(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        mCube.importer.Loaded -= PlaySong;
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
