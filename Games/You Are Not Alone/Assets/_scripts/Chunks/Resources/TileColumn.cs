using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileColumn : MonoBehaviour
{
    private bool hasBeenModified = false;
    public bool Modified { get { return hasBeenModified; } }
    public const char spacer = '\u0191';
    public const char endOfTypes = '\u0148';
    public static int maxHeight = 8;

    public GameObject tilePrefab;
    [SerializeField]
    private List<TileType> columnPrint = new List<TileType>();

    List<Tile> renderTiles = new List<Tile>();
    Point position;

    public TileColumn(GameObject tilePrefab, Point pos)
    {
        this.position = pos;
        this.tilePrefab = tilePrefab;
    }
    public TileColumn()
    {
        this.position = Point.zero;
    }
    public void Start()
    {
        this.tilePrefab = Resources.Load<GameObject>("Cube.prefab");
    }
    public void OnDestroy()
    {
    }

    public void Setup(GameObject tilePrefab, Point pos, ICollection<TileType> print = null)
    {
        this.position = pos;
        this.tilePrefab = tilePrefab;
        this.columnPrint.AddRange(print != null ? print : GenerateEmptyColumn());
    }

    private void SetupPosition()
    {
        this.transform.localPosition = new Vector3(this.position.x, 0f, this.position.y) * Tile.getGridLength();
    }


    public void Init(Vector3 offset, List<TileType> front, List<TileType> right, List<TileType> back, List<TileType> left)
    {
        if (renderTiles == null)
            renderTiles = new List<Tile>();
        if (columnPrint == null || columnPrint.Count != maxHeight)
        {
            Print();
        }
        Tile[] objArray = renderTiles.ToArray();
        int count = renderTiles.Count;
        for(int i = 0; i < count; i++)
        {
            Destroy(objArray[i].gameObject);
        }
        renderTiles.Clear();

        Tile tileData = BuildTile();
        int layerHgt = columnPrint.Count - 1;
        SetupPosition();
       /* string s = "";
        for(int i = 0; i < layerHgt; i++)
        {
            s += "[" + columnPrint[i] + "]";
        }
        Debug.Log(s);
       */
        while (layerHgt >= 0 && columnPrint[layerHgt--] == TileType.Empty);
        layerHgt++;
        tileData.setHeight(layerHgt);
        tileData.type = columnPrint[layerHgt];
        renderTiles.Add(tileData);
        layerHgt--;

        //Debug.Log(front.Count + ", " + right.Count + ", " + back.Count + ", " + left.Count);
        while (layerHgt >= 0 &&
            ((columnPrint[layerHgt] == TileType.Water || columnPrint[layerHgt + 1 >= columnPrint.Count ? layerHgt : layerHgt + 1] == TileType.Water ||
            columnPrint[layerHgt] != TileType.Empty) ||
            HasSidesExposed(left == null? TileType.Empty : left[layerHgt],
            front == null? TileType.Empty : front[layerHgt],
            right == null? TileType.Empty : right[layerHgt],
            back == null? TileType.Empty : back[layerHgt])))
        { 
            //Debug.Log(layerHgt);
            tileData = BuildTile();
            tileData.setHeight(layerHgt);
            tileData.type = columnPrint[layerHgt];
            renderTiles.Add(tileData);
            layerHgt--;
        }
    }
    public void SetTileRenderFlags(List<TileType> front, List<TileType> right, List<TileType> back, List<TileType> left)
    {
        foreach(Tile tile in renderTiles)
        {
            int h = tile.getHeight();
           /* string frontValue = front == null ? "-" : front.Count.ToString();
            string rightValue = right == null ? "-" : right.Count.ToString();
            string backValue = back == null ? "-" : back.Count.ToString();
            string leftValue = left == null ? "-" : left.Count.ToString();

            //Debug.Log(h + "|" + frontValue + "," + rightValue + "," + backValue + "," + leftValue);
            */
            tile.SetUpFaceRendering(columnPrint, front == null ? TileType.Empty : front[h],
                right == null ? TileType.Empty : right[h],
                back == null ? TileType.Empty : back[h],
                left == null ? TileType.Empty : left[h]);
        }
    }
    public void BuildTileMesh(Vector3 offset)
    {
        foreach(Tile tile in renderTiles)
        {
            tile.Init(offset);
            Vector3[] verts = tile.Verticies();
            int[] triangles = tile.Triangles();
        }
    }
    public void Build(Vector3 offset, List<TileType> front, List<TileType> right, List<TileType> back, List<TileType> left)
    {
        SetTileRenderFlags(front, right, back, left);
        BuildTileMesh(offset);
    }
    public IEnumerable<Tile> Tiles { get { return renderTiles; } }
    public GameObject[] getTiles() { GameObject[] tileObjs = new GameObject[renderTiles.Count]; int i = 0; foreach (Tile t in renderTiles) { tileObjs[i++] = t.gameObject; } return tileObjs; }
    public static ICollection<TileType> GeneratedColumnCollection(int grass, int dirt, int sand, int water)
    {
        //8, 5, 3 
        // GGGDDSSS
        //Debug.Log(grass +"|" + dirt + "|" + sand);
        List<TileType> generatedColumn = new List<TileType>();
        int i = 0;
        while(i < maxHeight)
        {

            if (i <= sand)
                generatedColumn.Add(TileType.Sand);
            else if (grass <= dirt && i <= dirt)
                generatedColumn.Add(TileType.Rock);
            else if (i <= grass)
                generatedColumn.Add(TileType.Grass);
            else
            {
                if (i > water)
                    generatedColumn.Add(TileType.Empty);
                else
                    generatedColumn.Add(TileType.Water);
            }
            i++;
        }
        return generatedColumn;
    }
    public static ICollection<TileType> GenerateEmptyColumn()
    {
        List<TileType> generatedColumn = new List<TileType>();
        for (int i = 0; i < maxHeight; i++)
            generatedColumn.Add(TileType.Empty);
        return generatedColumn;
    }
    private bool HasSidesExposed(TileType left, TileType front, TileType right, TileType back)
    {
        if (left == TileType.Empty || left == TileType.Water)
            return true;
        if (front == TileType.Empty || front == TileType.Water)
            return true;
        if (right == TileType.Empty || right == TileType.Water)
            return true;
        if (back == TileType.Empty || back == TileType.Water)
            return true;
        return false;
    }
    public int getX() { return position.x; }
    public int getZ() { return position.y; }
    public Point getPosition() { return position; }
    private Tile BuildTile()
    {
        if (tilePrefab == null)
            tilePrefab = Resources.Load<GameObject>("Cube.prefab");
        if (tilePrefab)
        {
            GameObject obj = Instantiate(tilePrefab, this.transform);
            obj.transform.localPosition = Vector3.zero;
            return obj.GetComponent<Tile>();
        }
        else
        {
            GameObject obj = Instantiate(TileChunk.TilePrefab,this.transform);
            obj.transform.localPosition = Vector3.zero;
            return obj.GetComponent<Tile>();
        }
    }
    public void SetPrint(IEnumerable<TileType> print)
    {
        if (print.Count<TileType>() != TileColumn.maxHeight)
            throw new System.Exception("SetPrint(print):  print.count does not equal TileColumn.maxHeight!!!");

        columnPrint.Clear();
        columnPrint.AddRange(print);
    }
    private List<TileType> Print()
    {
        if (hasBeenModified)
        {
            Debug.Log("!");
            Dictionary<int, TileType> newPrint = new Dictionary<int, TileType>();
            List<int> availableSlots = new List<int>();

            for (int i = 0; i < TileColumn.maxHeight; i++)
                availableSlots.Add(i);

            foreach (Tile t in renderTiles)
            {
                int h = t.getHeight();
                if (!newPrint.ContainsKey(h))
                {
                    newPrint.Add(h, t.type);
                    availableSlots.Remove(h);
                }
            }
            foreach (int v in availableSlots)
            {
                newPrint.Add(v, TileType.Empty);
            }

            columnPrint.Clear();
            for (int i = 0; i < TileColumn.maxHeight; i++)
                columnPrint.Add(newPrint[i]);
        }
        return columnPrint;
    }
    public List<TileType> getPrint { get { return Print(); } }
    public void SetTile(int height, TileType type)
    {
        Debug.Log(height +"/"+ type);
        foreach (Tile t in renderTiles)
            if (t.getHeight() == height)
            {
                hasBeenModified = true;
                t.type = type;
                return;
            }
        Tile tileData = BuildTile();
        tileData.setHeight(height);
        tileData.type = type;
        renderTiles.Add(tileData);
        hasBeenModified = true;
    }
    public string Write()
    {
        string output = "";
        foreach (TileType type in columnPrint)
            output += type.ToString() + spacer;
        output.Remove(output.Length - 1);
        output += endOfTypes;
        return output;
    }
    public static List<TileType> Read(string colStr)
    {
        List<TileType> print = new List<TileType>();
        string[] splitStr = colStr.Split(spacer);
        foreach (string col in splitStr)
            print.Add((TileType)Enum.Parse(typeof(TileType), col));
        return print;
    }

    internal void RemoveColumn()
    {
        foreach(Tile tile in renderTiles)
        {
            Destroy(tile.gameObject);
        }
    }
    internal void UpdateRenderTiles(List<TileType> front, List<TileType> right, List<TileType> back, List<TileType> left)
    {
        Tile[] objArray = renderTiles.ToArray();
        int count = renderTiles.Count;
        for (int i = 0; i < count; i++)
        {
            Destroy(objArray[i].gameObject);
        }
        renderTiles.Clear();
        Tile tileData = BuildTile();
        int layerHgt = columnPrint.Count - 1;

        while (layerHgt >= 0 && columnPrint[layerHgt--] == TileType.Empty) ;
        layerHgt++;
        tileData.setHeight(layerHgt);
        tileData.type = columnPrint[layerHgt];
        renderTiles.Add(tileData);
        layerHgt--;

        while (layerHgt >= 0 &&
            ((columnPrint[layerHgt] == TileType.Water || columnPrint[layerHgt + 1 >= columnPrint.Count ? layerHgt : layerHgt + 1] == TileType.Water ||
            columnPrint[layerHgt] != TileType.Empty) ||
            HasSidesExposed(left == null ? TileType.Empty : left[layerHgt],
            front == null ? TileType.Empty : front[layerHgt],
            right == null ? TileType.Empty : right[layerHgt],
            back == null ? TileType.Empty : back[layerHgt])))
        {
            tileData = BuildTile();
            tileData.setHeight(layerHgt);
            tileData.type = columnPrint[layerHgt];
            renderTiles.Add(tileData);
            layerHgt--;
        }
    }
}
