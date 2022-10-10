using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [SerializeField]
    public int seed;
    [SerializeField]
    public SerializableDictionary<Point, SerializableChunk> Chunks;
    public GameData()
    {
        Chunks = new SerializableDictionary<Point, SerializableChunk>();
    }
}
