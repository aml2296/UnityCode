using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;


//10.22.22
public class Player3rdPerson : AnimatableEntity
{
    [SerializeField]
    Transform cameraTransform = null;
    [SerializeField]
    GameObject playerModel = null;
    [SerializeField]
    Behaviour[] behaviours;

    public override void Start()
    {
        base.Start();
        this.StateChange += SetRunningAnimation;
        this.StateChange += SetIdleAnimation;
        this.StateChange += SetFireAnimation;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        this.StateChange -= SetRunningAnimation;
        this.StateChange -= SetIdleAnimation;
        this.StateChange -= SetFireAnimation;
    }
    public override Vector2 getMoveInput()
    {
        Vector3 move = Input.GetAxis("Horizontal") * cameraTransform.right + Input.GetAxis("Vertical") * cameraTransform.forward;
        return new Vector2(move.x, move.z);
    }
    public override Vector2 HandleAirInput()
    {
        return getMoveInput()*0.35f;
    }
    public override Vector2 HandleGroundedInput()
    {
        return getMoveInput();
    }
    public override void OnDie()
    {
        throw new System.NotImplementedException();
    }
    public override void OnGrounded()
    {
        return;
    }
    public override void rotateTowards(Vector3 direction)
    {
        float singleStep = this.turnSpeed * Time.deltaTime;
        Transform rotator = playerModel != null ? playerModel.transform : transform;

        Vector3 newDirection = Vector3.RotateTowards(rotator.forward, direction, singleStep, 0f);
        if (rotator == transform && rbody != null)
        {
            ConsoleHandler.Log(newDirection.ToString());
            rbody.MoveRotation(Quaternion.LookRotation(newDirection));
        }
        else
            rotator.rotation = Quaternion.LookRotation(newDirection);
    }
    public override void Move()
    {
        base.Move();
    }
    public override void Update()
    {
        base.Update();
    }
    public void SetRunningAnimation(EntityState s)
    {
        if (s != EntityState.grounded_moving || !hasAnimator)
            return;

        animator.Play("Run");
    }
    public void SetIdleAnimation(EntityState s)
    {
        if (s != EntityState.grounded || !hasAnimator)
            return;

        animator.Play("Idle");
    }
    public void SetFireAnimation(EntityState s)
    {
        if (s != EntityState.attacking || !hasAnimator)
            return;

        animator.Play("Fire");
    }
    public void Hide()
    {
        foreach (var behaviour in behaviours)
            behaviour.enabled = false;
        playerModel.SetActive(false);
    }

}
