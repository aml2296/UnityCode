using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class AnimatableEntity : Entity
{
    [SerializeField]
    protected Animator animator;
    public bool hasAnimator { get { return animator != null; } }
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }
}
