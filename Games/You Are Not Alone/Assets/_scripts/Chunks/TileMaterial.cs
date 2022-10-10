using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaterial : MonoBehaviour
{
    public static Dictionary<TileType, Material> material = new Dictionary<TileType, Material>();
    [SerializeField]
    Material[] mats;

    public void Awake()
    {
        Init();
    }
    public void Start()
    {
        if (material.Count < 0)
            Init();
    }

    private void Init()
    {
        try
        {
            material.Clear();
            material.Add(TileType.Grass, mats[0]);
            material.Add(TileType.Rock, mats[1]);
            material.Add(TileType.Sand, mats[2]);
            material.Add(TileType.Water, mats[3]);
            material.Add(TileType.Indestructable, mats[4]);
            material.Add(TileType.Wood, mats[5]);
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
     }
}
