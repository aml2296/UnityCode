using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    [Range(0,100f)]
    public float speed;
    
    public void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        float yAxis = Input.GetAxis("Float");

        if(xAxis != 0)
            transform.position += transform.right * xAxis * speed * Time.deltaTime;
        if (zAxis != 0)
            transform.position += transform.forward * zAxis * speed * Time.deltaTime;
        if (yAxis != 0)
            transform.position += transform.up * yAxis * speed * Time.deltaTime;
    }
}
