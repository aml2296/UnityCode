using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climable : MonoBehaviour
{
    [SerializeField]
    private ClimbableEntity parent;
    [SerializeField]
    private float climbSpeed = 5f;

    public void setParent(ClimbableEntity p)
    {
        parent = p;
    }
    public float getSpeed()
    { return climbSpeed; }
    public void OnTriggerEnter(Collider other)
    {
        if (parent != null && other.gameObject.layer == LayerMask.NameToLayer("Ground") && !other.gameObject.CompareTag("NonClimbable"))
        {
            parent.setState(EntityState.climbing);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (parent != null && other.gameObject.layer == LayerMask.NameToLayer("Ground") && !other.gameObject.CompareTag("NonClimbable"))
        {
            if (parent.getState() == EntityState.climbing)
                parent.setState(EntityState.airborn);
        }
    }
}
