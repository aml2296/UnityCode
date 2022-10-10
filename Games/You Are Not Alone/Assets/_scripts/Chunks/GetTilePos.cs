using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTilePos : MonoBehaviour
{
    Point tile;



    public Point GetTile() { return tile; }
    public void OnCollisionEnter(Collision collision)
    {
    }
}
