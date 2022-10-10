using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum TileType
{
    Grass,
    Rock,
    Water,
    Sand,
    Empty,
    Indestructable,
    Wood,
    Glass,
    Metal
}




[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Tile : MonoBehaviour
{
    MeshRenderer meshRend;
    MeshFilter meshFilter;
    Mesh mesh;

    public TileType type;

    public bool topRender = false;
    public bool bottomRender = false;
    public bool frontRender = false;
    public bool rightRender = false;
    public bool backRender = false;
    public bool leftRender = false;




    public event Action<Vector3> ActivateEvent;
    [SerializeField]
    private List<Vector3> verticies = new List<Vector3>();
    [SerializeField]
    private List<int> triangles = new List<int>();
    [SerializeField]
    private List<Vector3> normals = new List<Vector3>();
    [SerializeField]
    private List<Vector2> uvs = new List<Vector2>();

    int height = 0;
    private static float gridLength = 1.5f;

    public Tile(int height = 0)
    {
        this.height = height;
        ActivateEvent += Init;
    }
    public void OnDestroy()
    {
        ActivateEvent -= Init;
    }
    public void Awake()
    {
        meshRend = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        mesh = meshFilter.mesh;
    }
    public void Update()
    {
    }
    public void OnCollisionEnter(Collision collision)
    {
    }
    public void Init(Vector3 offset)
    {
        if (meshRend == null)
            meshRend = GetComponent<MeshRenderer>();
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if (mesh == null)
            mesh = meshFilter.mesh;
    
        name = "Cube: ["+this.height.ToString()+"]";
        verticies.Clear();
        verticies.AddRange(Verticies());
        for (int i = 0; i < verticies.Count; i++)
            verticies[i] += offset;
        triangles.Clear();
        triangles.AddRange(Triangles());

        normals.Clear();
        normals.AddRange(Normals());

        uvs.Clear();
        uvs.AddRange(UVS());
/*
        string vertStr = verticies.Count.ToString();
        string triStr = triangles.Count.ToString();
        string normStr = normals.Count.ToString();
        foreach (int t in triangles)
        {
            triStr += "[" + t + "]";
        }
        foreach (Vector3 n in normals)
        {
            normStr += "{" + n + "}";
        }
        foreach (Vector3 v in verticies)
        {
            vertStr += "|" + v + "|";
        }
       // Debug.Log("Error: " + triStr + "| Vertices: " + vertStr + "| Normals: " + normStr);*/
        try
        {
            if(mesh == null)
            {
                throw new System.Exception("MESH IS NULL");
            }
            mesh.vertices = verticies.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            if (type != TileType.Empty)
                meshRend.sharedMaterial = TileMaterial.material[type];
            GetComponent<MeshFilter>().mesh = mesh;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerable<Vector2> UVS()
    {
        List<Vector2> returnList = new List<Vector2>();
        int faceCount = 0;
        if (this.topRender)
            faceCount++;
        if (this.frontRender)
            faceCount++;
        if (this.rightRender)
            faceCount++;
        if (this.backRender)
            faceCount++;
        if (this.leftRender)
            faceCount++;
        if (this.bottomRender)
            faceCount++;

        for(int i = 0; i < faceCount; i++)
        {
            returnList.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) });
        }
        return returnList;
    }

    public static float CubeLength { get { return gridLength; } }
    public static float CenterToEdge { get { return gridLength / 2; } }
    public void setHeight(int h)
    {
        height = h;
        transform.localPosition += ((height * getGridLength()) - transform.position.y) * Vector3.up;
    }
    public void Activate(Vector3 offset)
    {
        ActivateEvent?.Invoke(offset);
    }
    public static Vector3[] Top()
    {
        Vector3[] v = {
            new Vector3(gridLength / 2, gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, gridLength / 2, -gridLength / 2),
            new Vector3(gridLength / 2, gridLength / 2, -gridLength / 2)
        };
        return v;
    }
    public static int[] FaceTop(int start)
    {
        return GenerateFace(start + 2, start + 1,start, start + 3);
    }
    public static Vector3[] getNormals(Vector3 direction, int amount)
    {
        Vector3[] v = new Vector3[amount];
        for (int i = 0; i < amount; i++)
            v[i] = direction;
        return v;
    }
    public static Vector3[] Bottom()
    {
        Vector3[] v = {
            new Vector3(gridLength / 2, -gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, -gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, -gridLength / 2, -gridLength / 2),
            new Vector3(gridLength / 2, -gridLength / 2, -gridLength / 2)
        };
        return v;
    }
    public static int[] FaceBottom(int start)
    {
        return GenerateFace(start, start + 1, start + 2, start + 3);
    }
    public static Vector3[] Front()
    {
        Vector3[] v = {
            new Vector3(gridLength / 2, gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, -gridLength / 2, gridLength / 2),
            new Vector3(gridLength / 2, -gridLength / 2, gridLength / 2)
        };
        return v;
    }
    public static int[] FaceFront(int start)
    {
        return GenerateFace(start, start+1, start+2, start+3);
    }
    public static Vector3[] Right()
    {
        Vector3[] v = {
            new Vector3(gridLength / 2, gridLength / 2, gridLength / 2),
            new Vector3(gridLength / 2, gridLength / 2, -gridLength / 2),
            new Vector3(gridLength / 2, -gridLength / 2, -gridLength / 2),
            new Vector3(gridLength / 2, -gridLength / 2, gridLength / 2)
        };
        return v;
    }
    public static int[] FaceRight(int start)
    {
        return GenerateFace(start + 2, start + 1, start, start + 3);
    }
    public static Vector3[] Back()
    {
        Vector3[] v = {
            new Vector3(gridLength / 2, gridLength / 2, -gridLength / 2),
            new Vector3(-gridLength / 2, gridLength / 2, -gridLength / 2),
            new Vector3(-gridLength / 2, -gridLength / 2, -gridLength / 2),
            new Vector3(gridLength / 2, -gridLength / 2, -gridLength / 2)
        };
        return v;
    }
    public static int[] FaceBack(int start)
    {
        return GenerateFace(start + 2, start + 1, start, start +3);
    }
    public static Vector3[] Left()
    {
        Vector3[] v = {
            new Vector3(-gridLength / 2, gridLength / 2, gridLength / 2),
            new Vector3(-gridLength / 2, gridLength / 2, -gridLength / 2),
            new Vector3(-gridLength / 2, -gridLength / 2, -gridLength / 2),
            new Vector3(-gridLength / 2, -gridLength / 2, gridLength / 2)
        };
        return v;
    }
    public static int[] FaceLeft(int start)
    {
        return GenerateFace(start, start + 1, start + 2, start +3);
    }

    public void SetUpFaceRendering(List<TileType> column, TileType forward, TileType right, TileType back, TileType left)
    {
        /*String s = "";
        foreach (TileType t in column)
        {
            s += t.ToString() + ", ";
        }
        Debug.Log(s + "|| " + height + " ? " + (height + 1));
        */
        if (this.type != TileType.Water)
        {
            if (ShouldRenderWithNeighborType(column[height + 1 > TileColumn.maxHeight ? 0 : height + 1]))
                this.topRender = true;

            if (ShouldRenderWithNeighborType(column[height - 1 < 0 ? 0 : height - 1]))
                this.bottomRender = true;

            if (ShouldRenderWithNeighborType(forward))
                this.frontRender = true;

            if (ShouldRenderWithNeighborType(back))
                this.backRender = true;

            if (ShouldRenderWithNeighborType(right))
                this.rightRender = true;

            if (ShouldRenderWithNeighborType(left))
                this.leftRender = true;

            if (this.topRender == false && this.frontRender == false &&
                this.rightRender == false && this.backRender == false &&
                this.leftRender == false && this.bottomRender == false)
                this.gameObject.SetActive(false);
        }
        else
        {
            if (column[height + 1 > TileColumn.maxHeight ? 0 : height + 1] != TileType.Water)
                this.topRender = true;

            if (column[height - 1 < 0 ? 0 : height - 1] != TileType.Water)
                this.bottomRender = true;

            if (forward != TileType.Water)
                this.frontRender = true;

            if (back != TileType.Water)
                this.backRender = true;

            if (right != TileType.Water)
                this.rightRender = true;

            if (left != TileType.Water)
                this.leftRender = true;
        }
       // Debug.Log("|top: " + topRender + "|bottom: " + bottomRender + "|front: " + frontRender + "|right: " + rightRender + "|back: " + backRender + "|left: " + leftRender);
    }
    public bool ShouldRenderWithNeighborType(TileType neighborType)
    {
        return neighborType == TileType.Empty || neighborType == TileType.Water;
    }
    public Vector3[] Verticies()
    {
        List<Vector3> verts = new List<Vector3>();
        if (topRender)
            verts.AddRange(Top());
        if(bottomRender)
            verts.AddRange(Bottom());
        if(frontRender)
            verts.AddRange(Front());
        if(rightRender)
            verts.AddRange(Right());
        if (backRender)
            verts.AddRange(Back());
        if (leftRender)
            verts.AddRange(Left());
        return verts.ToArray();
    }
    public Vector3[] Normals()
    {
        List<Vector3> norms = new List<Vector3>(); 
        if (topRender)
            norms.AddRange(getNormals(Vector3.up, 4));
        if (bottomRender)
            norms.AddRange(getNormals(Vector3.down, 4));
        if (frontRender)
            norms.AddRange(getNormals(Vector3.forward, 4));
        if (rightRender)
            norms.AddRange(getNormals(Vector3.right, 4));
        if (backRender)
            norms.AddRange(getNormals(Vector3.back, 4));
        if (leftRender)
            norms.AddRange(getNormals(Vector3.left, 4));
        return norms.ToArray();
    }
    public int[] Triangles()
    {
        List<int> tris = new List<int>();
        int i = 0;
        if (topRender)
        {
            tris.AddRange(FaceTop(i));
            i += 4;
        }
        if (bottomRender)
        {
            tris.AddRange(FaceBottom(i));
            i += 4;
        }
        if (frontRender)
        {
            tris.AddRange(FaceFront(i));
            i += 4;
        }
        if (rightRender)
        {
            tris.AddRange(FaceRight(i));
            i += 4;
        }
        if (backRender)
        {
            tris.AddRange(FaceBack(i));
            i += 4;
        }
        if (leftRender)
        { 
            tris.AddRange(FaceLeft(i));
            i += 4;
        }
        return tris.ToArray();
    }
    public int getHeight()
    {
        return height;
    }
    public static float getGridLength()
    { return gridLength; }
    public static int[] GenerateFace(int a,int b,int c,int d)
    {
        return new int[] { a, b, c, a, c, d };
    }
}
