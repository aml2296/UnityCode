using System;
using UnityEngine;


public abstract class ClimbableEntity : AnimatableEntity
{ 
    [SerializeField]
    private GameObject ClimbBox;
    protected Climable climb;
    public override void Start()
    {
        try
        {
            if (ClimbBox == null)
                throw new Exception("No Climb Box Ref!!!: " + gameObject.name);
            if (ClimbBox.GetComponent<Climable>() == null)
                throw new Exception("No Climable Component: " + gameObject.name);
            if (ClimbBox.GetComponent<Collider>() == null)
                throw new Exception("No Collider Component: " + gameObject.name);

            climb = ClimbBox.GetComponent<Climable>();
            climb.setParent(this);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        base.Start();
    }
    public override void Update()
    {
        Vector2 moveInput = Vector2.zero;
        EntityState finalizedState = state;

        if (finalizedState != EntityState.climbing)
        {
            if (gravity.getGrounded())
            {
                finalizedState = EntityState.grounded;
                velocity.y = -2f;
                moveInput = HandleGroundedInput();
                if(moveInput != Vector2.zero)
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
        }
        else
        {
            moveInput = HandleClimbInput();
        }    

        velocity.y += gravity.getValue() * Time.deltaTime;
        setState(finalizedState);
        setInput(moveInput);
        Move();
    }
    public override void Move()
    {
        if (state == EntityState.climbing)
        {
            controller.Move((transform.up *  zInput + transform.right * xInput) * climb.getSpeed() * Time.deltaTime);
            velocity = controller.velocity;
        }
        else
            base.Move();
    }
    public override void OnGrounded()
    {
        velocity.x = 0f;
        velocity.z = 0f;
    }
    public virtual Vector2 HandleClimbInput()
    {
        return this.HandleGroundedInput();
    }
}
