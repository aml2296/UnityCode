using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isenabled : MonoBehaviour
{
    [SerializeField]
    Collider en;
    [SerializeField]
    bool started = false;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (en && en.enabled && started)
            Gizmos.DrawCube(transform.position, 1.5f * Vector3.one);
    }
}
