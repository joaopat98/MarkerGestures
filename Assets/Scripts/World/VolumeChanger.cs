using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChanger : MonoBehaviour, IFloatCallBack
{
    AudioSource source;
    public SliderController volumeSlider;
    public void ValueChanged(float value)
    {
        source.volume = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        volumeSlider.RegisterCallBack(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
