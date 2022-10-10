using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[Serializable]
public class RotationEvent : UnityEvent{}

[Serializable]
public class RotationNode
    {
    [SerializeField]
    public Vector3 rotation = Vector3.zero;
    [SerializeField]
    public RotationEvent rotationEvent = null;
    
    public void FireEvent()
    {
        rotationEvent?.Invoke();
        if (rotationEvent == null)
            Debug.Log("NULL EVENT FOR ROT: " + rotation.ToString());
    }
}

public class DayNightEvents : MonoBehaviour
{
    private float withinDegree = 1f;
    [SerializeField]
    List<RotationNode> roationEvents = new List<RotationNode>();
    [SerializeField]
    Color duskColor;
    [SerializeField]
    float duskDensity = 0.05f;
    [SerializeField]
    Color dawnColor;
    [SerializeField]
    float dawnDensity = 0.05f;

    public void Dawn()
    {
        Debug.Log("DAWN");
        RenderSettings.fogColor = dawnColor;
        RenderSettings.fogDensity = dawnDensity;
    }
    public void Dusk()
    {
        Debug.Log("DUSK");
        RenderSettings.fogColor = duskColor;
        RenderSettings.fogDensity = duskDensity;
    }

    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;
        foreach (RotationNode node in roationEvents)
        {
            if (node.rotation.x > 360 || node.rotation.x < 0)
                node.rotation.x = 0;
            if (node.rotation.y > 360 || node.rotation.y < 0)
                node.rotation.y = 0;
            if (node.rotation.z > 360 || node.rotation.x < 0)
                node.rotation.x = 0;
            if (Math.Abs(node.rotation.x - currentRotation.x) <= withinDegree &&
                Math.Abs(node.rotation.y - currentRotation.y) <= withinDegree &&
                    Math.Abs(node.rotation.z - currentRotation.z) <= withinDegree)
            {
                Debug.Log(currentRotation + "|" + node.rotation);
                node.FireEvent();
            }
        }
    }
}
