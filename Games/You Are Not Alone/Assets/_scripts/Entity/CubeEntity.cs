using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEntity : BehaviourEntity
{
    [SerializeField]
    [Range(0,1000)]
    int size = 1;
    
    
    public new void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }

    public override void OnDie()
    {
    }

    public override void OnGrounded()
    {
    }
    public override void setTarget(Transform target)
    {
        base.setTarget(target);
    }
    public void changeSize(int s)
    {
        size = s;
    }
}
