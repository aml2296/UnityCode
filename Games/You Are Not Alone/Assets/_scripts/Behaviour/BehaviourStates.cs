using System;
using UnityEngine;

[SerializeField]
public abstract class BehaviourState
{
    [SerializeField]
    protected string name;
    [SerializeField]
    private bool isRunning;

    public BehaviourState()
    {
        Start();
    }
    public virtual string getName() { return name; }
    public virtual bool IsRunning() { return isRunning; }
    public void setRunning(bool isRunning) { this.isRunning = isRunning; }
    public virtual Vector2 Reaction() { return Vector2.zero; }
    public virtual void Start() { isRunning = true; }
    public virtual void Stop() { isRunning = false; }
    public override string ToString()
    { return name; }
}
[Serializable]
public class BehaviourReadStates
{
    [SerializeField]
    private BehaviourState state;
    [SerializeField]
    private string name;
    [SerializeField]
    private bool isActive;
    [SerializeField]
    private Vector3 targetPos = Vector3.zero;


    public BehaviourReadStates(BehaviourState state)
    {
        this.state = state;
        this.isActive = state.IsRunning();
        this.name = state.getName();
        if (state is MoveToBehaviour)
        {
            MoveToBehaviour mtState = (MoveToBehaviour)state;

            this.targetPos = mtState.getTarget();
        }
    }
}