using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletSwipable : ISwipable
{
    Renderer rend;
    public float SwipeDuration = 1f;
    public float SwipeOffset = 1f;
    public bool testSwipe;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    public override void SwipeLeft(GestureResolver parent)
    {
        StartCoroutine(Swipe(1));
    }

    public override void SwipeRight(GestureResolver parent)
    {
        StartCoroutine(Swipe(-1));
    }

    IEnumerator Swipe(float direction)
    {
        var curOffset = rend.material.mainTextureOffset;
        var finalOffset = curOffset + new Vector2(SwipeOffset * direction, 0);
        float t = 0;
        while (t < SwipeDuration)
        {
            rend.material.mainTextureOffset = Vector2.Lerp(curOffset, finalOffset, EasingFunction.EaseOutCirc(0, 1, t / SwipeDuration));
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
        }
    }

    void Update()
    {
        if (testSwipe)
        {
            testSwipe = false;
            StartCoroutine(Swipe(1));
        }
    }
}
