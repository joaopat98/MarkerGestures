using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicCube : CubeGrabbable
{
    public string songName;
    private string dataPath;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public MobileImporter importer;
    public event Action<MusicCube> Released;

    public void Init()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        dataPath = Application.persistentDataPath + "/songs/" + songName + "/";
        var imagePath = dataPath + "/image.png";
        if (System.IO.File.Exists(imagePath))
        {
            var bytes = System.IO.File.ReadAllBytes(imagePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            GetComponent<Renderer>().material.mainTexture = tex;
        }
        importer = GetComponent<MobileImporter>();
        importer.Loaded += OnLoaded;
        importer.Import((Application.platform == RuntimePlatform.WindowsEditor ? "file:///" : "file://") + dataPath + "song.mp3");
    }

    public override void Release()
    {
        base.Release();
        if (Released != null)
            Released(this);
    }

    void OnLoaded(AudioClip clip)
    {
        audioClip = clip;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
