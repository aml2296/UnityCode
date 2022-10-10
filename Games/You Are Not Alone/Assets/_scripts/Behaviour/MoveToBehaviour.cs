using System;
using UnityEngine;

[Serializable]
public class MoveToBehaviour : BehaviourState
{
    [SerializeField]
    Vector3 target;
    [SerializeField]
    Vector3 position;
    float withIn = 1f;

    public MoveToBehaviour(Vector3 target, Vector3 position, float withInValue)
    {
        Start(target, position, withInValue);
    }
    public void Start(bool value = true)
    {
        if (!value)
            setRunning(false);
        else
            Start();
    }
    void Start(Vector3 target, Vector3 position, float withInValue)
    {
        name = "MoveTo";

        this.target = target;
        this.position = position;
        this.withIn = withInValue;
    }
    public override Vector2 Reaction()
    {
        if (this.IsRunning())
        {
            Vector2 direction = (target.x - position.x) * Vector2.right + (target.z - position.z) * Vector2.up;
            if (direction.magnitude < withIn)
            {
                direction = Vector2.zero;
                this.Stop();
            }
            else
            {
                direction = direction.normalized;
            }
            return direction;
        }
        return Vector2.zero;
    }
    public void setTargetPos(Vector3 pos)
    {
        target = pos;
    }
    public Vector3 getTarget()
    {
        return target;
    }
    public void setPosition(Vector3 pos)
    {
        position = pos;
    }
    public new void Stop()
    {
        base.Stop();
    }
}
