using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

//10.31.22
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CharacterController))]
public abstract class Entity : GravityEntity
{
    public event Action<EntityState> StateChange;

    protected float xInput;
    protected float zInput;
    [Range(0, 9999)]
    [SerializeField] float entityForce;
    protected CharacterController controller;
    protected Health hp;
    protected EntityState state = default(EntityState);

    [SerializeField]
    float runSpeed = 5f;
    [SerializeField]
    protected float turnSpeed = 15f;

    public override void Start()
    {
        base.Start();
        controller = GetComponent<CharacterController>();
        hp = GetComponent<Health>();

        hp.death += OnDie;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        hp.death -= OnDie;
    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;
        rb.AddForceAtPosition(entityForce * controller.velocity.normalized, hit.point);
    }
    public override void Update()
    {
        base.Update();

        EntityState finalizedState = state;
        Vector2 moveInput = Vector2.zero;

        rotateTowards(new Vector3(moveInput.x, 0f, moveInput.y));
        
        if (gravity.getGrounded())
        {
            finalizedState = EntityState.grounded;
            moveInput = HandleGroundedInput();
            if (moveInput != Vector2.zero)
            {
                finalizedState = EntityState.grounded_moving;
            }
        }
        else
        {
            finalizedState = EntityState.airborn;
            moveInput = HandleAirInput();
        }

        setState(finalizedState);
        SetInput(moveInput);
        Move();
    }
    public virtual void Move()
    {
        controller.Move(new Vector3(this.xInput, 0f, this.zInput) * Time.deltaTime * runSpeed);
        controller.Move(velocity * Time.deltaTime);
    }
    public virtual void rotateTowards(Vector3 direction)
    {
        float singleStep = turnSpeed * Time.deltaTime;

        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction , singleStep, 0f);
        rbody.MoveRotation(Quaternion.LookRotation(newDirection));
    }
    abstract public Vector2 getMoveInput();
    abstract public Vector2 HandleGroundedInput();
    abstract public Vector2 HandleAirInput();
    abstract public void OnDie();


    public void AddVelocity(Vector3 vel) {
        velocity += vel;
    }
    public void SetInput(Vector2 _input)
    {
        xInput = _input.x;
        zInput = _input.y;
    }
    public EntityState getState()
    {
        return state;
    }
    public void setState(EntityState s)
    {
        if(s == state)
            return;
        StateChange?.Invoke(s);
        this.state = s;
    }
    public static string getStateString(EntityState s)
    {
        switch (s)
        {
            case EntityState.climbing:
                return "Climbing";
            case EntityState.airborn:
                return "Airborn";
            case EntityState.attacking:
                return "Attacking";
            case EntityState.grounded_moving:
                return "Grounded_Moving";
            case EntityState.grounded:
            default:
                return "Grounded";
        }
    }
}
