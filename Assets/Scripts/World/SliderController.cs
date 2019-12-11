using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderController : MonoBehaviour
{
    SliderGrabbable slider;
    public void RegisterCallBack(IFloatCallBack callBack)
    {
        slider.RegisterCallBack(callBack);
    }

    public void RemoveCallBack(IFloatCallBack callBack)
    {
        slider.RemoveCallBack(callBack);
    }
    // Start is called before the first frame update
    void Awake()
    {
        slider = transform.Find("Slider Handle").GetComponent<SliderGrabbable>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
