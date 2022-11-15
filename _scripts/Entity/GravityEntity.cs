using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Gravity))]
public abstract class GravityEntity : MonoBehaviour
{
    protected Rigidbody rbody;
    protected Vector3 velocity = Vector3.zero;
    protected Gravity gravity;

    public virtual void Start()
    {
        rbody = GetComponent<Rigidbody>();
        gravity = GetComponent<Gravity>();
        gravity.grounded += OnGrounded;
    }
    public virtual void Update()
    {
        if (gravity.getGrounded())
        {
            velocity.y = -2f;
        }
        velocity.y += gravity.getValue() * Time.deltaTime;
    }
    public virtual void FixedUpdate()
    {
        if (rbody)
            rbody.AddForce(velocity-rbody.velocity, ForceMode.VelocityChange);
    }
    public virtual void OnDestroy()
    {
        gravity.grounded -= OnGrounded;
    }
    abstract public void OnGrounded();
}
