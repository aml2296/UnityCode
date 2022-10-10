using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Behaviour : MonoBehaviour
{
    [SerializeField]
    List<BehaviourState> actionQueue = new List<BehaviourState>();
    [SerializeField]
    BehaviourReadStates[] readStates;

    [SerializeField]
    int index = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (actionQueue.Count > 0)
        {
            index = 0;
            actionQueue[index].Start();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (actionQueue.Count > 0)
        {
            readStates = new BehaviourReadStates[actionQueue.Count];
            int i = 0;
            foreach(BehaviourState bs in actionQueue.ToArray())
                readStates[i++] = new BehaviourReadStates(bs);
            
            if (!actionQueue[index].IsRunning())
            {
                index = index + 1 >= actionQueue.Count ? 0 : index + 1;
                actionQueue[index].Start();
            }
        }
    }
    public void AddState(BehaviourState state)
    {
        actionQueue.Add(state);
    }
    public BehaviourState getState()
    {
        if (actionQueue.Count <= 0)
            return null;
        if (index >= actionQueue.Count)
            index = 0;
        return actionQueue[index];
    }
    public bool isEmpty { get { return actionQueue.Count == 0; } }
    public void Clear()
    {
        actionQueue.Clear();
        index = -1;
    }
}
