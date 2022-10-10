using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableChunk : ISerializationCallbackReceiver
{
    [SerializeField]
    public SerializableDictionary<Point,SerializableColumn> map = new SerializableDictionary<Point, SerializableColumn>();

    public SerializableChunk(SerializableDictionary<Point, TileColumn> chunkMap) 
    {
        foreach(Point key in chunkMap.Keys)
        {
            SerializableColumn col = new SerializableColumn(chunkMap[key]);
            map.Add(key, col);
        }
    }
    public void OnBeforeSerialize()
    {
    }
    public void OnAfterDeserialize()
    {
    }
}

[Serializable]
public class SerializableColumn : ISerializationCallbackReceiver
{
    [SerializeField]
    public SerializableDictionary<int, TileType> columnMap = new SerializableDictionary<int, TileType>();
    public SerializableColumn(TileColumn column)
    {
        List<TileType> tiles = column.getPrint;
        for (int i =0; i < tiles.Count; i++)
        {
            columnMap.Add(i, tiles[i]);
        }
    }

    public void OnBeforeSerialize()
    {
    }
    public void OnAfterDeserialize()
    {
    }
}
