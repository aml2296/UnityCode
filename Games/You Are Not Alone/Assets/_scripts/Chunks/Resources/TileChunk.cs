using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class TileChunk : MonoBehaviour, IDataPersistance
{
    private static GameObject colPrefab;
    private static GameObject tilePrefab;
    public static GameObject TilePrefab { get { return tilePrefab; } }


    public static float scale = 100;
    public static int length = 7;
    private int seed = 0;
    private float loadValue = 0;


    protected bool isPure = true;
    public bool Pure { get { return this.isPure; } set { isPure = value; } }
    protected Point position = new Point();
    public Point getPosition { get { return position; } }

    [SerializeField]
    public SerializableDictionary<Point, TileColumn> tileMap = new SerializableDictionary<Point, TileColumn>();
    public SerializableDictionary<Point, TileColumn> Map { get { return tileMap; } }
    public IEnumerable<TileColumn> Columns { get { return tileMap.Values; } }
    public IEnumerable<Point> Keys { get { return tileMap.Keys; } }
    List<Collider> mapColliders = new List<Collider>();
    private bool initalized = false;


    public TileChunk(int posX, int posY)
    {
        this.position.x = posX;
        this.position.y = posY;
        this.isPure = true;
    }
    public TileChunk(Point position)
    {
        this.position = position;
        this.isPure = true;
    }

    public void Start()
    {
        this.isPure = true;
        if(colPrefab == null || tilePrefab == null)
          setPrefabs(Resources.Load<GameObject>("Column.prefab"), Resources.Load<GameObject>("Cube.prefab"));
    }
    internal void setPrefabs(GameObject _colPrefab, GameObject _tilePrefab)
    {
        colPrefab = _colPrefab;
        tilePrefab = _tilePrefab;
    }

    
    
    public void InitBlank()
    {
        //Setup Name
        gameObject.name = "[" + position.x + ", " + position.y + "]";
        if (initalized)
            return;

        //Init Seed
        seed = 0;

        //Init Lists
        if (tileMap != null)
            tileMap.Clear();
        else
            tileMap = new SerializableDictionary<Point, TileColumn>();

        //Build Tiles 
        for (int x = 0; x < length; x++)
            for (int y = 0; y < length; y++)
            {
                BuildColumn(new Point(x, y));
            }

        //Finalize
        initColumns(tileMap);
        SetColliders(false);
        initalized = true;
    }
    public void InitSeed(int _seed, Dictionary<Point,float> islandMask, bool _log = false)
    {
        string debugStr = "Init Seeded: ";


        //Setup Name
        gameObject.name = "["+position.x+", " + position.y + "]";
        debugStr += gameObject.name;
        if (initalized)
            return;

        //Init Seeds
        UnityEngine.Random.InitState(_seed);
        seed = (int)(UnityEngine.Random.value * 10000d);
        
        //Init Lists
        if (tileMap != null)
            tileMap.Clear();
        else
            tileMap = new SerializableDictionary<Point, TileColumn>();

        //is Within Mask
        bool inIslandBounds = islandMask.ContainsKey(position);

        //Build Tiles with seed
        for (int x = 0; x < length; x++)
            for (int y = 0; y < length; y++)
            {
                Point pos = new Point(x, y);

                float[] xCord = new float[3];
                float[] yCord = new float[3];

                xCord[0] = (pos.x + (position.x * Tile.getGridLength() * length) + seed) / scale;
                yCord[0] = (pos.y + (position.y * Tile.getGridLength() * length) + seed) / scale;

                xCord[1] = (pos.x + (position.x * Tile.getGridLength() * length) + seed) / scale * 2;
                yCord[1] = (pos.y + (position.y * Tile.getGridLength() * length) + seed) / scale * 2;

                xCord[2] = (pos.x + (position.x * Tile.getGridLength() * length) + seed * 4) / scale * 2;
                yCord[2] = (pos.y + (position.y * Tile.getGridLength() * length) + seed * 4) / scale * 2;


                float grassSample = Mathf.PerlinNoise(xCord[0], yCord[0]);
                float dirtSample = Mathf.PerlinNoise(xCord[1], yCord[1]);
                float sandSample = Mathf.PerlinNoise(xCord[2], yCord[2]);



                int sandLayer = inIslandBounds ? (int)(islandMask[position] * sandSample * (int)(2 * sandSample) + 1) : (int)(2 * sandSample) + 1;
                int dirtLayer = inIslandBounds ? (int)(islandMask[position] * dirtSample * length + sandLayer) + sandLayer : 0;
                int grassLayer = inIslandBounds ? (int)(islandMask[position] * grassSample * TileColumn.maxHeight) : 0;
                int waterLevel = 3;
                debugStr += pos + (sandLayer + ", " + dirtLayer + ", " + grassLayer + "||" + grassSample + ", " + dirtSample + ", " + grassSample) + '\n';


                BuildColumn(pos, sandLayer, dirtLayer, grassLayer, waterLevel);
            }
        if(_log)
        Debug.Log(debugStr);

        //Finalize
        initColumns(tileMap);
        SetColliders(false);
        initalized = true;
    }

    public GameObject AddToMap(Point pos)
    {
        GameObject column = BuildColumn(pos);
        AddToMap(pos, column);
        return column;
    }
    private void AddToMap(Point pos, GameObject column)
    {
        TileColumn columnTile = column.GetComponent<TileColumn>();
        if (columnTile == null)
            throw new System.Exception("No TileColumn Component Attatched To GameObject!!!");
        AddToMap(pos, columnTile);
    }
    private void AddToMap(Point pos, TileColumn column)
    {
        if (tileMap == null)
            tileMap = new SerializableDictionary<Point, TileColumn>();

        if (tileMap.ContainsKey(pos))
            tileMap.Remove(pos);

        TileColumn columnTile = column.GetComponent<TileColumn>();
        if (columnTile == null)
            throw new System.Exception("No TileColumn Component Attatched To GameObject!!!");
        tileMap.Add(pos, columnTile);
    }
    private GameObject BuildColumn(Point pos)
    {
        GameObject columnObject = Instantiate(colPrefab, this.transform);
        TileColumn tileCol = columnObject.GetComponent<TileColumn>();
        tileCol.Setup(tilePrefab, pos);
        AddToMap(pos, tileCol);
        return columnObject;
    }
    private GameObject BuildColumn(Point pos, int sandLayer, int dirtLayer, int grassLayer, int waterLevel)
    {
        GameObject columnObject = Instantiate(colPrefab, this.transform);
        TileColumn tileCol = columnObject.GetComponent<TileColumn>();
        tileCol.Setup(tilePrefab, pos, TileColumn.GeneratedColumnCollection(grassLayer, dirtLayer, sandLayer, waterLevel));
        AddToMap(pos,tileCol);
        return columnObject;
    }
    internal void initColumns(Dictionary<Point,TileColumn> map)
    {
        foreach (TileColumn col in map.Values)
        {
            Point pos = col.getPosition();
            Point front = new Point(pos.x, pos.y + 1);
            List<TileType> frontList = tileMap.ContainsKey(front) ? tileMap[front].getPrint : getColumnFromNeighbor(new Point(position.x, position.y + 1), new Point(pos.x, 0));
            Point right = new Point(pos.x + 1, pos.y);
            List<TileType> rightList = tileMap.ContainsKey(right) ? tileMap[right].getPrint : getColumnFromNeighbor(new Point(position.x + 1, position.y), new Point(0, pos.y));
            Point back = new Point(pos.x, pos.y - 1);
            List<TileType> backList = tileMap.ContainsKey(back) ? tileMap[back].getPrint : getColumnFromNeighbor(new Point(position.x, position.y - 1), new Point(pos.x, length - 1));
            Point left = new Point(pos.x - 1, pos.y);
            List<TileType> leftList = tileMap.ContainsKey(left) ? tileMap[left].getPrint : getColumnFromNeighbor(new Point(position.x - 1, position.y), new Point(length - 1, pos.y));

            col.Init(Vector3.zero, frontList, rightList, backList, leftList);
            col.gameObject.name = pos.ToString();
        }
    }


    private bool IsNeighbor(List<Point> neighborHood, Point other)
    {
        if (neighborHood.Count < 1)
            return true;
        foreach(Point h in neighborHood)
        {
            if (h.Equals(other) || h.isNeighbor(other))
                return true;
        }
        return false;
    }
    public int GetTopTile(Point p)
    {
        if (tileMap != null && tileMap.ContainsKey(p))
        {
            List<TileType> print = tileMap[p].getPrint;
            int value = -1;
            for (int i = 0; i < print.Count; i++)
                if (print[i] == TileType.Empty || print[i] == TileType.Water)
                {
                    value = i - 1;
                    break;
                }
            return value;
        }
        return 0;
    }
    internal void setPosition(Point pos)
    {
        this.position = pos;
        this.transform.localPosition = new Vector3(pos.x, 0.0f, pos.y) * Tile.getGridLength() * length;
    }
    public void UpdateRender()
    {
        if(tileMap != null)
        foreach(TileColumn col in tileMap.Values)
        {
            Point pos = col.getPosition();
            Point front = new Point(pos.x, pos.y + 1);
            List<TileType> frontList = tileMap.ContainsKey(front) ? tileMap[front].getPrint : getColumnFromNeighbor(new Point(position.x, position.y + 1), new Point(pos.x, 0));
            Point right = new Point(pos.x + 1, pos.y);
            List<TileType> rightList = tileMap.ContainsKey(right) ? tileMap[right].getPrint : getColumnFromNeighbor(new Point(position.x + 1, position.y), new Point(0, pos.y));
            Point back = new Point(pos.x, pos.y - 1);
            List<TileType> backList = tileMap.ContainsKey(back) ? tileMap[back].getPrint : getColumnFromNeighbor(new Point(position.x, position.y - 1), new Point(pos.x, length - 1));
            Point left = new Point(pos.x - 1, pos.y);
            List<TileType> leftList = tileMap.ContainsKey(left) ? tileMap[left].getPrint : getColumnFromNeighbor(new Point(position.x - 1, position.y), new Point(length - 1, pos.y));

            col.UpdateRenderTiles(frontList, rightList, backList, leftList);
            col.SetTileRenderFlags(frontList, rightList, backList, leftList);
            col.BuildTileMesh(Vector3.zero);
        }
    }
    
    public void SetColliders(bool val)
    {
        if (mapColliders== null)
            mapColliders = new List<Collider>();
        if (tileMap != null && val)
        {
            //Debug.Log(position.ToString());
            List<List<Point>> allLayers = new List<List<Point>>();
            Dictionary<Point, TileColumn> map = tileMap.ToDictionary(entry => entry.Key, entry => entry.Value);
            int layer = 0;
           while (map.Count > 0)
            {
                List<Point> tilesToRemove = new List<Point>();
                //Debug.Log("Map count [" + map.Count + "] Layer: " + layer);
                List<Vector3> layerdTiles = new List<Vector3>();
                foreach (TileColumn c in map.Values)
                {
                    //Debug.Log(c.getPosition().ToString());
                    
                    GameObject[] tilesRenderedInColumn = c.getTiles();
                    int i = 0;
                    Tile topTile;
                    topTile = tilesRenderedInColumn[i++].GetComponent<Tile>();
                    try
                    {
                        while (i < tilesRenderedInColumn.Length && (topTile.type == TileType.Empty || topTile.type == TileType.Water))
                            topTile = tilesRenderedInColumn[i++].GetComponent<Tile>();
                    }
                    catch
                    {
                        Debug.Log(i);
                    }
                    if (topTile.getHeight() == layer)
                    {
                        Point pos = c.getPosition();

                        layerdTiles.Add(new Vector3(pos.x, layer, pos.y));
                        tilesToRemove.Add(pos);
                    }
                }
                string s = "";
                foreach (Point p in tilesToRemove)
                {
                    s += p.ToString() + " ";
                    map.Remove(p);
                }
                if(tilesToRemove.Count > 0)
                //Debug.Log("Removed: " + s);

                if (layerdTiles.Count > 0)
                    mapColliders.AddRange(BestCollidersFromLayer(tilesToRemove, layer));
                layer++;
            }
        }
        else if (mapColliders.Count > 0)
        {
            foreach (BoxCollider c in mapColliders)
                c.enabled = false;
        }
    }
    private BoxCollider[] BestCollidersFromLayer(List<Point> list, int layer)
    {
        List<BoxCollider> colliderSets = new List<BoxCollider>();
        int counter = 0;
        string pstr = "List: ";
        foreach (Point p in list)
            pstr += p.ToString();
       // Debug.Log(pstr);
        while(list.Count > 0)
        {
            List<Point> neighbors = new List<Point>();
            List<Vector3> neighborsAsVector = new List<Vector3>();
           // string s = "Neighborhood [" + counter + "] :  ";
            bool foundNeighbor;
            do
            {
                foundNeighbor = false;
                Point foundNeighborTile = Point.zero;
                foreach (Point tile in list)
                {
                    bool nCheck = IsNeighbor(neighbors, tile);
                    if (nCheck)
                    {
                        neighbors.Add(tile);
                        neighborsAsVector.Add(new Vector3(tile.x, layer, tile.y));
                        //s += tile.ToString();
                        foundNeighbor = true;
                        foundNeighborTile = tile;
                        break;
                    }
                }
                list.Remove(foundNeighborTile);

            } while (foundNeighbor);

            colliderSets.Add(ColliderFromTilePositions(neighborsAsVector));
            counter++;
        }
        return colliderSets.ToArray();
    }
    private BoxCollider ColliderFromTilePositions(List<Vector3> pos)
    {
        BoxCollider collider = null;
        if (pos.Count > 0)
        {
            //Debug.Log("C");
            float leftBound = pos[0].x;
            float rightBound = leftBound;
            float forwardBound = pos[0].z;
            float backwardBound = forwardBound;

            foreach (Vector3 p in pos)
            {

                if (p.x < leftBound)
                    leftBound = p.x;
                if (p.x > rightBound)
                    rightBound = p.x;
                if(p.z > forwardBound)
                    forwardBound = p.z;
                if (p.z < backwardBound)
                    backwardBound = p.z;
            }


            float width = rightBound - leftBound + 1;
            float length = forwardBound - backwardBound + 1;

            collider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
            collider.size = new Vector3(width, 1, length) * Tile.CubeLength;
            Vector3 v = (Vector3.left + Vector3.back) * (Tile.CubeLength / 2);
            //Debug.Log(v.ToString());
            collider.center = new Vector3(leftBound + (width / 2), pos[0].y, backwardBound + (length / 2)) * Tile.CubeLength + (Vector3.left + Vector3.back) * (Tile.CubeLength / 2);
            
            
            //Debug.Log("[l: "+ leftBound + " f: " + forwardBound + " r: " + rightBound +" b:"+ backwardBound +" ]" + collider.size.ToString() + "||" + collider.center.ToString());
        }
        return collider;
    }
    public List<TileType> getColumnFromNeighbor(Point neighbor, Point column)
    {
        string debugStr = "";
        var map = EnvironmentHandler.Map;
        if (map.ContainsKey(neighbor))
        {
            TileChunk chunk = map[neighbor].GetComponent<TileChunk>();
            if (chunk != null)
            {
                if (chunk.tileMap != null && chunk.tileMap.ContainsKey(column))
                {
                    return chunk.tileMap[column].getPrint;
                }
                debugStr += chunk.tileMap == null ? "Chunk's tile map was: NULL" : column.ToString();
            }
            debugStr += " From chunk id: " + neighbor.ToString();
        }
        else
            debugStr += "Could not find Neighbor: " + neighbor.ToString();
        //Debug.Log(debugStr);
        return null;
    }

    public void LoadData(GameData data)
    {
        string debugStr = "Chunk " + position.ToString() + " ||   '\n'";

            if (data.seed != 0)
            {
                this.InitSeed(data.seed, EnvironmentHandler.GetMask);
            }
            else
                this.InitBlank();

        if (data.Chunks.Keys.Contains(position))
        {
            foreach (Point key in data.Chunks[position].map.Keys)
            {
                GameObject columnObj = this.BuildColumn(key);
                TileColumn column = columnObj.GetComponent<TileColumn>();
                column.SetPrint(data.Chunks[position].map[key].columnMap.Values);
                debugStr += column.getPosition().ToString();
                foreach (TileType Ttype in data.Chunks[position].map[key].columnMap.Values)
                {
                    debugStr += (int)Ttype;
                }
                debugStr += '\n';
            }
            Debug.Log(debugStr);
            this.isPure = false;
            this.initColumns(tileMap);
            this.UpdateRender();
        }
    }
    public void SaveData(ref GameData data)
    {
        if (data.Chunks == null)
            data.Chunks = new SerializableDictionary<Point, SerializableChunk>();
        if (data.Chunks.Keys.Contains(position))
            data.Chunks.Remove(position);
        /*
        SerializableChunk sChunk = new SerializableChunk(position);
        Debug.Log("Converting to Serial Chunk");

        foreach (KeyValuePair<Point, TileColumn> kvp in this.tileMap)
        {
            sChunk.tileMap.Add(kvp.Key, kvp.Value);
        }
        data.Chunks.Add(position, sChunk);
    */
        SerializableChunk chunk = new SerializableChunk(ModifiedColumns());
        data.seed = seed;
        data.Chunks.Add(position, chunk);
    }

    private SerializableDictionary<Point, TileColumn> ModifiedColumns()
    {
        SerializableDictionary<Point, TileColumn> columns = new SerializableDictionary<Point, TileColumn>();
        foreach (Point key in tileMap.Keys)
        {
            if (tileMap[key].Modified)
                columns.Add(key, tileMap[key]);
        }
        return columns;
    }

    public void RemoveChunk()
    {
        foreach (TileColumn obj in tileMap.Values)
        {
            obj.RemoveColumn();
            Destroy(obj);
        }
    }
}
