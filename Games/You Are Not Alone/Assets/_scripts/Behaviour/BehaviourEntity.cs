using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Behaviour))]
public abstract class BehaviourEntity : ClimbableEntity
{
    [SerializeField]
    float objAcknowledgeDistance = 1f;
    [SerializeField]
    protected GameObject target;
    protected Behaviour behaviour;

    public void Awake()
    {
        behaviour = GetComponent<Behaviour>();
    }
    public override void Update()
    {
        base.Update();
        BehaviourState behavState = behaviour.getState();
        if (behavState != null && behavState is MoveToBehaviour)
        {
            MoveToBehaviour moveToState = (MoveToBehaviour)behavState;
            moveToState.setTargetPos(target.transform.position);
            moveToState.setPosition(transform.position);
        }

    }
    public override Vector2 HandleGroundedInput()
    {
        BehaviourState bs = behaviour.getState();
        if (bs != null)
            return bs.Reaction();
        return Vector2.zero;
    }
    public virtual void setTarget(Transform target)
    {
        if (target != null && this.target == target)
            return;
        this.target = target.gameObject;
        behaviour.Clear();
        behaviour.AddState(new MoveToBehaviour(target.transform.position, transform.position, objAcknowledgeDistance));
    }
    public override Vector2 getMoveInput()
    {
        return Vector2.zero;
    }
    public override Vector2 HandleAirInput()
    {
        return Vector2.zero;
    }
}
