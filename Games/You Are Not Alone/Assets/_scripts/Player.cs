using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Claims;

public class Player : ClimbableEntity
{
    Inventory inventory;
    [SerializeField]
    TextMeshProUGUI stateText;
    [SerializeField]
    GameObject playerModel;
    [SerializeField]
    float maxFOOD = 10;
    [SerializeField]
    float maxThurst = 10;
    [SerializeField]
    Transform cmv;

    Point chunkPos = new Point();
    Vector3 playerPosition = Vector3.zero;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (other.transform.position.y < transform.position.y &&
                other.transform.position.y >= transform.position.y - Tile.getGridLength())
                AddVelocity(Vector3.up * 5);
        }
    }
    public override void Start()
    {
        base.Start();
        inventory = GetComponentInChildren<Inventory>();
        this.StateChange += updateStateText;
        this.StateChange += setRunningAnimation;
        this.StateChange += setIdleAnimation;
        this.StateChange += setUseAnimation;
        this.StateChange += setClimbAnimation;
    }
    public override void rotateTowards(Vector3 direction)
    {
        float singleStep = this.turnSpeed * Time.deltaTime;
        Transform rotator = playerModel != null? playerModel.transform : transform;

        Vector3 newDirection = Vector3.RotateTowards(rotator.forward, direction, singleStep, 0f);
        if (rotator==transform && rbody != null)
        {
            Debug.Log(newDirection);
            rbody.MoveRotation(Quaternion.LookRotation(newDirection));
        }
        else
            rotator.rotation = Quaternion.LookRotation(newDirection);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void OnDestroy()
    {
        this.StateChange -= updateStateText;
        this.StateChange -= setRunningAnimation;
        this.StateChange -= setIdleAnimation;
        this.StateChange -= setUseAnimation;
        this.StateChange -= setClimbAnimation;
        base.OnDestroy();
    }
    public override void Move()
    {
        if (state == EntityState.climbing)
        {
            controller.Move((playerModel.transform.up * zInput + playerModel.transform.right * xInput) * climb.getSpeed() * Time.deltaTime);
            velocity = controller.velocity;
        }
        else
            base.Move();
    }
    public override Vector2 getMoveInput()
    {
        Vector3 move = Input.GetAxis("Horizontal") * cmv.right + Input.GetAxis("Vertical") * cmv.forward;
        return new Vector2(move.x, move.z);
    }

    public override Vector2 HandleAirInput()
    {
        return getMoveInput();
    }

    public override Vector2 HandleGroundedInput()
    {
        return getMoveInput();
    }
    public override Vector2 HandleClimbInput()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    public override void OnDie()
    {
        Debug.Log("Dead");
    }
    public override void OnGrounded()
    {       
        base.OnGrounded();
    }
    public Inventory getInventory()
    {
        return inventory;
    }

    public void updateStateText(EntityState s)
    {
        if (stateText)
            stateText.text = getStateString(s);
    }
    public void setClimbAnimation(EntityState s)
    {
        if (s != EntityState.climbing || !hasAnimator)
            return;

        this.animator.Play("T", 0, 0);
    }
    public void setRunningAnimation(EntityState s)
    {
        if (s != EntityState.grounded_moving || !hasAnimator)
            return;

       this.animator.Play("Running", 0, 0);
    }
    public void setIdleAnimation(EntityState s)
    {
        if (s != EntityState.grounded || !hasAnimator)
            return;

        this.animator.Play("Idle", 0, 0);
    }
    public void setUseAnimation(EntityState s)
    {
        if (s != EntityState.attacking || !hasAnimator)
            return;

        this.animator.Play("Use", 0, 0);
    }
}
