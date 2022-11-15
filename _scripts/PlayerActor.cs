using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ServerEntity))]
public class PlayerActor : PlayerController
{
    ServerEntity serverEntity; 
    [SerializeField]
    GameObject playerModel = null;
    [SerializeField]
    Behaviour[] behaviours;

    protected bool listenToInput = true;
    public bool ListenToInput { get { return listenToInput; } set { listenToInput = value; } }

    public override void Start()
    {
        base.Start();
        serverEntity = GetComponent<ServerEntity>();
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
        if (listenToInput)
            return base.getMoveInput();
        return new Vector2(xInput, zInput);
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

    public void setInputActions(bool[] inputActionData)
    {
        throw new NotImplementedException();
    }
}
