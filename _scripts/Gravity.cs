using System;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public event Action grounded;

    [SerializeField] 
    GameObject parent;
    [SerializeField]
    [Range(-20, 20)]
    private float gravityMagnitude = -9.81f;
    [SerializeField]
    [Range(0, 2)]
    private float checkRadius = 0.4f;
    [SerializeField]
    Vector3 checkOffset = Vector3.zero;
    [SerializeField]
    LayerMask mask;

    private bool isGrounded = false;

    void Update()
    {
        bool prevGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(transform.position + checkOffset, checkRadius, mask);

        if (!prevGrounded && isGrounded)
            grounded?.Invoke();
    }

    public bool getGrounded() { return isGrounded;}
    public float getValue() { return gravityMagnitude; }
}
