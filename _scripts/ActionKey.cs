using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActionKey
{
    [SerializeField]
    string actionName = "...";
    [SerializeField]
    List<KeyCode> keyList = new List<KeyCode>();
    [SerializeField]
    bool[] isKeysDown = null;
    [SerializeField]
    bool attemptAction = false;


    [SerializeField]
    int Size { get { return keyList.Count; } }
    [SerializeField]
    public KeyCode[] Keys => keyList.ToArray();
    public string Name => actionName;
    public bool[] KeysDown => isKeysDown;
    public bool AttemptingAction => attemptAction;

    public int AddKey()
    { 
        keyList.Add(default(KeyCode));
        return keyList.Count;
    }
    public int RemoveKey()
    {
        if(keyList.Count > 0)
            keyList.RemoveAt(keyList.Count - 1);
        return keyList.Count;
    }

    internal void SetKeysDown(bool[] _keysDown)
    {
        if (_keysDown.Length != isKeysDown.Length)
            throw new Exception("Wrong sized array!");

        isKeysDown = _keysDown;
    }

    public ActionKey(string name = "...", IEnumerable<KeyCode> keys = null, bool[] keysDown = null, bool _attemptAction = false)
    {
        actionName = name;
        keyList = new List<KeyCode>(keys);
        isKeysDown = keysDown;
        attemptAction = _attemptAction;
    }
    public ActionKey() { }
}
