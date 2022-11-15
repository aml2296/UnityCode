using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Point : Comparer<Point>
{
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;

    public Point(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }
    public Point(string p)
    {
        char[] delim = { ',', ' ', '[', ']' };
        string[] posStr = p.Split(delim);
        this.x = int.Parse(posStr[0]);
        this.y = int.Parse(posStr[1]);
    }
    public Point(float x , float y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }
    public Point(Vector3 value)
    {
        this.x = (int)(value.x + 0.5);
        this.y = (int)(value.z + 0.5);
    }
    public static Point zero { get { return new Point(0, 0); } }
    public override string ToString()
    {
        return "[" + x + ", " + y + "]";
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as Point);
    }
    public bool Equals(Point obj)
    {
        return obj != null && obj.x == this.x && obj.y == this.y;
    }
    public bool isNeighbor(Point other)
    {
        if (other.x == this.x && other.y == this.y)
            throw new Exception("Other is the Same as 'this'");

        if (this.x + 1 >= other.x && other.x >= this.x - 1
            && this.y + 1 >= other.y && other.y >= this.y - 1
            && Math.Abs(other.y) != Math.Abs(this.x))
            return true;
        return false;
    }
    public override int Compare(Point A, Point B)
    {
        int xCompare = A.x.CompareTo(B.x);
        if (xCompare != 0)
        {
            return xCompare;
        }
        else
        {
            return A.y.CompareTo(B.y);
        }
    }
}
