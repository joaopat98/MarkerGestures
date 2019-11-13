using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circle : MonoBehaviour
{
    public float speed = 0.1f, radius = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x, y;
        x = radius * Mathf.Cos(Time.time);
        y = radius * Mathf.Sin(Time.time);
        Vector3 pos = transform.position;
        pos.x = x;
        pos.y = y;
        transform.position = pos;
        Debug.Log(1 / Time.deltaTime);
    }
}
