using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CharacterController))]
public abstract class Entity : GravityEntity
{
    public event Action<EntityState> StateChange;

    protected float xInput;
    protected float zInput;
    protected CharacterController controller;
    protected Health hp;
    protected EntityState state;

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
    public override void Update()
    {
        base.Update();

        EntityState finalizedState = state;
        Vector2 moveInput = Vector2.zero;

        if (gravity.getGrounded())
        {
            finalizedState = EntityState.grounded;
            moveInput = HandleGroundedInput();
            if (moveInput != Vector2.zero)
            {
                finalizedState = EntityState.grounded_moving;
                rotateTowards(new Vector3(moveInput.x, 0f, moveInput.y));
            }
        }
        else
        {
            finalizedState = EntityState.airborn;
            moveInput = HandleAirInput();
        }

        setState(finalizedState);
        setInput(moveInput);
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
    public void setInput(Vector2 _input)
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
    public string getStateString(EntityState s)
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
