using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public enum EnvHandleType
{
    IslandGen,
    Create,
    LoadFromFile
}

[RequireComponent(typeof(ChunkLoader))]
[Serializable]
public class EnvironmentHandler : MonoBehaviour, IDataPersistance
{
    private Point currentChunk = new Point();
    private Point currentColChunk = new Point();
    private ChunkLoader chunkLoader;

    private bool objectMoveX = true;
    private bool objectMoveY = true;
    private bool objectMoveZ = true;
    private bool objectSelect = true;

    [Header ("Environment Type")]
    [SerializeField]
    private EnvHandleType handleType = EnvHandleType.IslandGen;

    [Space(5)]
    [Header("File Settings")]
    [SerializeField]
    string fileName = "";

    [Space(5)]
    [Header("Create Settings")]
    [SerializeField]
    private TileType selectedTileType = TileType.Indestructable;
    [SerializeField]
    private Transform origin;
    [Space(5)]
    [Header("General Map Options")]
    private static int seed;
    public static int Seed { get { return seed; } set { seed = value; } }
    public bool randomSeed = false;
    public int chunkLength = 2;
    public int colChunkLoad = 1;
    public int loadChunkLength = 1;

    [Space(5)]
    [Header("Island Options")]
    static Dictionary<Point, float> mapMask;
    public static Dictionary<Point, float> GetMask { get { return mapMask; } }
    [SerializeField] private Point islandfromCenterMinMax = new Point(200, 300); //The minimum size (x) and maximum size (y) of the generated island
    [SerializeField] private int anchorIslandCount = 1;
    [SerializeField] private float anchorRadius = 50f;


    static List<Point> chunkPostEdit = new List<Point>();
    bool chunkIsLoading = false;
    private static Dictionary<Point,GameObject> chunkMap = new Dictionary<Point, GameObject>();
    public static Dictionary<Point,GameObject> Map { get { return chunkMap; } set { chunkMap = value; } }
    static List<GameObject> updateChunkQueue = new List<GameObject>();
    static List<GameObject> removeChunkQueue = new List<GameObject>();
    
    
    static Point boundsX = new Point();
    static Point boundsZ = new Point();
    Point colBoundsX = new Point();
    Point colBoundsZ = new Point();


    [Space(5)]
    [Header("Other")]
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject chunkPrefab;
    [SerializeField]
    GameObject colPrefab;
    [SerializeField]
    GameObject tilePrefab;


    [Space(5)]
    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI fpsValue;
    [SerializeField]
    TextMeshProUGUI chunkValue;
    [SerializeField]
    TextMeshProUGUI seedValue;

    public void Start()
    {
        TileColumn.maxHeight = TileChunk.length * 2;
        chunkLoader = GetComponent<ChunkLoader>();
        chunkLoader.setEnvHan(this);
        Begin();
    }
    public void Begin()
    {
        boundsZ.x = boundsX.x = -chunkLength;
        boundsZ.y = boundsX.y = chunkLength;
        colBoundsX.x = colBoundsZ.x = -colChunkLoad;
        colBoundsX.y = colBoundsZ.y = colChunkLoad;

        switch (handleType)
        {
            case EnvHandleType.IslandGen:
                Init();
                break;
            case EnvHandleType.Create:
                chunkMap.Add(Point.zero, CreateChunk(0, 0));
                break;
            case EnvHandleType.LoadFromFile:
                break;
            default:
                break;
        }
        int halfChunkPoint = (int)(TileChunk.length / 2);
        Vector3 pos = chunkMap[Point.zero].transform.position;
        Vector3 center = Tile.getGridLength() * halfChunkPoint * Vector3.one;
        Vector3 height = Vector3.up * (chunkMap[Point.zero].GetComponent<TileChunk>().GetTopTile(new Point(halfChunkPoint)) + 4);

        player.gameObject.transform.SetPositionAndRotation(pos + center + height, Quaternion.identity);

    }
    public void Init()
    {
        if (randomSeed)
            seed = (int)UnityEngine.Random.Range(0, 1000);
        seedValue.text = seed.ToString();
        Point[] points = chunkMap.Keys.ToArray();
        string debugStr = "";
        for (int i = 0; i < points.Length; i++)
        { RemoveChunk(points[i]); debugStr += (points[i].ToString() + '\n'); }

        Debug.Log(chunkMap.Keys.Count + "||" + debugStr);
        for (int x = -chunkLength; x <= chunkLength; x++)
        {
            for (int y = -chunkLength; y <= chunkLength; y++)
            {
                GameObject chunk = CreateChunk(x, y);
                chunkMap.Add(new Point(x, y), chunk);
            }
        }


        mapMask = BuildIsland(anchorRadius);
        foreach (GameObject obj in chunkMap.Values)
        {
            TileChunk chunk = obj.GetComponent<TileChunk>();
            switch (handleType)
            {
                case EnvHandleType.IslandGen:
                    chunk.InitSeed(seed, mapMask);
                    break;
                case EnvHandleType.Create:
                    break;
                case EnvHandleType.LoadFromFile:
                    break;
                default:
                    break;
            }
        }
        updateRenderFaces();
        InitColliderChunk();
    }
    public void InitColliderChunk()
    {
        for (int i = colBoundsX.x; i <= colBoundsX.y; i++)
            for (int j = colBoundsZ.x; j <= colBoundsZ.y; j++)
            {
                Point pos = new Point(i, j);
                TileChunk tC = chunkMap[pos].GetComponent<TileChunk>();
                tC.SetColliders(true);
            }
    }

    public void Update()
    {
        int current = (int)(1f / Time.unscaledDeltaTime);
        fpsValue.text = current.ToString();
        Point chunkID = getChunkPos(player.transform.position);
        Vector3 posID = getTilePosition(player.transform.position);
        chunkValue.text = "(" + chunkID.x + ", " + chunkID.y + ")\n" +
            "(" + (int)(posID.x + 0.5f) + ", " + (int)(posID.y + 0.5f) + ", " + (int)(posID.z + 0.5f) + ")";

        switch (handleType)
        {
            case EnvHandleType.IslandGen:
                updateChunkPosition(chunkID, mapMask);
                if (chunkIsLoading && !chunkLoader.isRunning) // Once Chunks have finished loading then
                    updateRenderFaces();
                break;
            case EnvHandleType.Create:
                if (origin != null)
                {
                    float xAxisInput = Input.GetAxis("HorizontalKP");
                    float zAxisInput = Input.GetAxis("VerticalKP");
                    float yAxisInput = Input.GetAxis("FloatKP");
                    float selectInput = Input.GetAxis("Submit");

                    Vector3 playerFwd = player.transform.forward;
                    Vector3 roundedVector = Mathf.Abs(playerFwd.x) > Mathf.Abs(playerFwd.z) ? playerFwd.x < 0 ? Vector3.left : Vector3.right : playerFwd.z < 0 ? Vector3.back : Vector3.forward;

                    Debug.DrawRay(player.transform.position, player.transform.forward * 10f, Color.red);
                    Debug.DrawRay(player.transform.position, roundedVector * 10f, Color.yellow);

                    if (objectMoveX)
                    {
                        if (xAxisInput > 0)
                        {
                            origin.position += Vector3.right * Tile.CubeLength;
                            objectMoveX = false;
                        }
                        else if (xAxisInput < 0)
                        {
                            origin.position += Vector3.left * Tile.CubeLength;
                            objectMoveX = false;
                        }
                    }
                    else if (xAxisInput == 0)
                        objectMoveX = true;

                    if (objectMoveZ)
                    {
                        if (zAxisInput > 0)
                        {
                            origin.position += Vector3.forward * Tile.CubeLength;
                            objectMoveZ = false;
                        }
                        else if (zAxisInput < 0)
                        {
                            origin.position += Vector3.back * Tile.CubeLength;
                            objectMoveZ = false;
                        }
                    }
                    else if (zAxisInput == 0)
                        objectMoveZ = true;


                    if (objectMoveY)
                    {
                        if (yAxisInput > 0)
                        {
                            origin.position += Vector3.up * Tile.CubeLength;
                            objectMoveY = false;
                        }
                        else if (yAxisInput < 0)
                        {
                            origin.position += Vector3.down * Tile.CubeLength;
                            objectMoveY = false;
                        }
                    }
                    else if (yAxisInput == 0)
                        objectMoveY = true;

                    if (objectSelect)
                    {
                        if (selectInput != 0)
                        {
                            this.SetTile(origin.position, selectedTileType, true);
                            updateRenderFaces();
                            objectSelect = false;
                        }
                    }
                    else if (selectInput == 0)
                        objectSelect = true;
                }
                break;
            case EnvHandleType.LoadFromFile:
                break;
            default:
                break;
        }
    }

    public List<float> generateRadiusValues(float radius)
    {
        List<float> values = new List<float>();
        int index = 1;
        int roundedCount = (int)(radius + 0.5f);
        float indexValue = 1f / (float)roundedCount;
        while (radius > 0)
        {
            values.Add((roundedCount - index) * indexValue);
            radius--;
            index++;
        }
        String s = "";
        foreach (float str in values)
        { s += ", " + str.ToString(); };

        Debug.Log(s + "||" + roundedCount + ", " + indexValue + ", " + radius);
        return values;
    }
    public Dictionary<Point, float> BuildIsland(float radius)
    {
        Dictionary<Point, float> circleMask = new Dictionary<Point, float>();
        float radiusMag = radius * radius;
        List<float> radiusValues = generateRadiusValues(radius);
        for (int x = -(int)radius; x <= radius; x++)
        {
            for (int y = -(int)radius; y <= radius; y++)
            {
                Point pos = new Point(x, y);
                float value = 0;
                if (Mathf.Abs(x * x) + Mathf.Abs(y * y) <= radiusMag)
                {
                    int v = Mathf.Abs(Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)));
                    if (v >= radiusValues.Count)
                        v = radiusValues.Count - 1;
                    value = radiusValues[v];
                }
                circleMask.Add(pos, value);
            }
        }
        return circleMask;
    }

    public void updateChunkPosition(Point newChunkID, Dictionary<Point, float> mask)
    {
        //Update Loaded Chunks into/outof Game
        if (!currentChunk.Equals(newChunkID))
        {
            int deltaLoadX = newChunkID.x - currentChunk.x;
            int deltaLoadZ = newChunkID.y - currentChunk.y;
            if (Mathf.Abs(deltaLoadX) >= loadChunkLength || Mathf.Abs(deltaLoadZ) >= loadChunkLength)
            {
                int counter = 0;
                while ((deltaLoadX <= -loadChunkLength || deltaLoadX >= loadChunkLength) || (deltaLoadZ <= -loadChunkLength || deltaLoadZ >= loadChunkLength))
                {
                    Debug.Log("Change Bounds!\nChange in X-Axis:" + deltaLoadX + " new Bounds{" + boundsX.x + ", " + boundsX.y + "}\nChange in Z-Axis:" + deltaLoadZ + " new Bounds{" + boundsZ.x + ", " + boundsZ.y + "}\nload length: " + loadChunkLength);
                    if (deltaLoadX > 0)
                    {
                        ShiftRight();
                        deltaLoadX -= loadChunkLength;
                    }
                    else if (deltaLoadX < 0)
                    {
                        ShiftLeft();
                        deltaLoadX += loadChunkLength;
                    }

                    if (deltaLoadZ > 0)
                    {
                        ShiftForward();
                        deltaLoadZ -= loadChunkLength;
                    }
                    else if (deltaLoadZ < 0)
                    {
                        ShiftBack();
                        deltaLoadZ += loadChunkLength;
                    }
                    counter += loadChunkLength;
                }


                chunkLoader.Queue(updateChunkQueue, removeChunkQueue, mask);
                chunkIsLoading = true;
                currentChunk = newChunkID;
            }
        }

        //Change local Collision Box
        if (!currentColChunk.Equals(newChunkID))
        {
            int deltaX = newChunkID.x - currentColChunk.x;
            int deltaZ = newChunkID.y - currentColChunk.y;
            while (deltaX != 0 || deltaZ != 0)
            {
                //Debug.Log("Change Collision!" + deltaX + ": " + colBoundsX.x + " <-> " + colBoundsX.y + " || " + deltaZ + ": " + colBoundsZ.x + " <-> " + colBoundsZ.y);
                if (deltaX != 0 || deltaZ != 0)
                {
                    if (deltaX > 0)
                    {
                        ShiftColRight();
                        deltaX--;
                    }
                    else if (deltaX < 0)
                    {
                        ShiftColLeft();
                        deltaX++;
                    }

                    if (deltaZ > 0)
                    {
                        ShiftColForward();
                        deltaZ--;
                    }
                    else if (deltaZ < 0)
                    {
                        ShiftColBack();
                        deltaZ++;
                    }
                }
            }
            currentColChunk = newChunkID;
        }
    }



    public void updateRenderFaces()
    {
        Debug.Log("Triggered Render Update!");
        string names = "";
        foreach (GameObject o in chunkMap.Values)
        {
            TileChunk tC = o.GetComponent<TileChunk>();
            tC.UpdateRender();
            names += tC.getPosition.ToString() + "|";
        }
        //Debug.Log(names);
        chunkIsLoading = false;
    }
    public void updateChunkLoadQueue(Point addKey)
    {
        GameObject chunk = CreateChunk(addKey.x, addKey.y);
        updateChunkQueue.Add(chunk);
    }
    private void updateChunkRemoveQueue(Point removeKey)
    {
        try
        {
            GameObject chunkObj;
            if (chunkMap.ContainsKey(removeKey))
            {
                chunkObj = chunkMap[removeKey];
                if (chunkMap.Remove(removeKey))
                {
                    Debug.Log("Added " + removeKey.ToString() + "into remove Array");
                    removeChunkQueue.Add(chunkObj);
                } 
            }
        }
        catch(Exception e)
        {
            Debug.Log("UpdateChunkMapError");
            Debug.LogException(e);
        }
    }
    
    private void ShiftForward()
    {
        boundsZ.y += loadChunkLength;
        for (int shift = 0; shift < loadChunkLength; shift++)
        {
            for (int i = boundsX.x; i <= boundsX.y; i++)
            {
                Point chunkAddKey = new Point(i, boundsZ.y - shift);
                updateChunkLoadQueue(chunkAddKey);
            }
        }
        boundsZ.x += loadChunkLength;
    }
    private void ShiftRight()
    {
        boundsX.y += loadChunkLength;
        for (int shift = 0; shift < loadChunkLength; shift++)
        {
            for (int i = boundsZ.x; i <= boundsZ.y; i++)
            {
                Point chunkAddKey = new Point(boundsX.y - shift, i);
                updateChunkLoadQueue(chunkAddKey);
            }
        }
        boundsX.x += loadChunkLength;
    }
    private void ShiftBack()
    {
        boundsZ.x -= loadChunkLength;
        for (int shift = 0; shift < loadChunkLength; shift++)
        {
            for (int i = boundsX.x; i <= boundsX.y; i++)
            {
                Point chunkAddKey = new Point(i, boundsZ.x + shift);
                updateChunkLoadQueue(chunkAddKey);
            }
        }
        boundsZ.y -= loadChunkLength;
    }
    private void ShiftLeft()
    {
        boundsX.x-= loadChunkLength;
        for (int shift = 0; shift < loadChunkLength; shift++)
        {
            for (int i = boundsZ.x; i <= boundsZ.y; i++)
            {
                Point chunkAddKey = new Point(boundsX.x + shift, i);
                updateChunkLoadQueue(chunkAddKey);
            }
        }
        boundsX.y-= loadChunkLength;
    }

    private void ShiftColForward()
    {
        colBoundsZ.y++;
        for (int i = colBoundsX.x; i <= colBoundsX.y; i++)
        {
            Point chunkAddKey = new Point(i, colBoundsZ.y);
            Point chunkRemovekey = new Point(i, colBoundsZ.x);
            //Debug.Log(chunkAddKey.x + ", " + chunkAddKey.y + " CHUNK KEY");   
            TileChunk chunkAddObj = chunkMap[chunkAddKey].GetComponent<TileChunk>();
            TileChunk chunkRemoveObj = chunkMap[chunkRemovekey].GetComponent<TileChunk>();
            chunkAddObj.SetColliders(true);
            chunkRemoveObj.SetColliders(false);
        }
        colBoundsZ.x++;
    }
    private void ShiftColRight()
    {
        colBoundsX.y++;//colChunkLoad;

            for (int i = colBoundsZ.x; i <= colBoundsZ.y; i++)
            {
                Point chunkAddKey = new Point(colBoundsX.y, i);
                Point chunkRemovekey = new Point(colBoundsX.x, i);
                TileChunk chunkAddObj = chunkMap[chunkAddKey].GetComponent<TileChunk>();
                TileChunk chunkRemoveObj = chunkMap[chunkRemovekey].GetComponent<TileChunk>();
                chunkAddObj.SetColliders(true);
                chunkRemoveObj.SetColliders(false);
            }
        colBoundsX.x++;
    }
    private void ShiftColBack()
    {
        colBoundsZ.x--;
        for (int i = colBoundsX.x; i <= colBoundsX.y; i++)
        {
            Point chunkAddKey = new Point(i, colBoundsZ.x);
            Point chunkRemovekey = new Point(i, colBoundsZ.y);
            TileChunk chunkAddObj = chunkMap[chunkAddKey].GetComponent<TileChunk>();
            TileChunk chunkRemoveObj = chunkMap[chunkRemovekey].GetComponent<TileChunk>();
            chunkAddObj.SetColliders(true);
            chunkRemoveObj.SetColliders(false);
        }
        colBoundsZ.y--;
    }
    private void ShiftColLeft()
    {
        colBoundsX.x--;
        for (int i = colBoundsZ.x; i <= colBoundsZ.y; i++)
        {
            Point chunkAddKey = new Point(colBoundsX.x, i);
            Point chunkRemovekey = new Point(colBoundsX.y, i);
            TileChunk chunkAddObj = chunkMap[chunkAddKey].GetComponent<TileChunk>();
            TileChunk chunkRemoveObj = chunkMap[chunkRemovekey].GetComponent<TileChunk>();
            chunkAddObj.SetColliders(true);
            chunkRemoveObj.SetColliders(false);
        }
        colBoundsX.y--;
    }
    public void SetTile(Vector3 position, TileType t, bool log = false)
    {

        Point tilePos = new Point(getTilePosition(position, true));
        Point chunkPos = getChunkPos(position,true);
        int posY = (int)(position.y / Tile.CubeLength);
        if (posY < 0)
            posY = 0;
        if (posY > TileColumn.maxHeight)
            posY = TileColumn.maxHeight;
        
        string debugStr = "Map: " + ((chunkMap == null) ? "NULL" : "EXISTS") + "!\n";
        try
        {
            if (chunkMap != null)
            {
                GameObject chunk;
                debugStr += "Chunk: ";
                if (chunkMap.ContainsKey(chunkPos))
                {
                    chunk = chunkMap[chunkPos];
                    debugStr += "Found " + chunkPos;
                }
                else
                {
                    chunk = CreateChunk(chunkPos);
                    chunkMap.Add(chunkPos, chunk);
                    debugStr += "Created " + chunkPos;
                }


                debugStr += "'\n'";
                TileChunk tileChunk = chunk.GetComponent<TileChunk>();
                if (tileChunk.tileMap == null)
                {
                    debugStr += "InitBlank()\n";
                    tileChunk.InitBlank();
                }


                debugStr += "Column: ";
                if (tileChunk.Keys != null)
                {
                    if (tileChunk.Keys.Contains(tilePos))
                    {
                        debugStr += "Found " + tilePos + "\n";
                        TileColumn column = tileChunk.Map[tilePos];
                        column.SetTile(posY, selectedTileType);
                        tileChunk.Pure = false;
                    }
                    else if (IsInBounds(tilePos))
                    {
                        debugStr += "Created " + tilePos + "\n";
                        GameObject column = tileChunk.AddToMap(tilePos);
                        column.GetComponent<TileColumn>().SetTile(posY, selectedTileType);
                        tileChunk.Pure = false;
                    }
                    else
                    {
                        throw new System.Exception(tilePos.ToString() + " is out of Chunk Bounds:  X:{ 0," + TileChunk.length + "}Z:{ 0," + TileChunk.length + "}");
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError(debugStr + "\n\n" + e.Message);
        }
        if (log)
            Debug.Log(debugStr);
}
    public GameObject CreateChunk(float x, float y)
    {
        return CreateChunk(new Point(x, y));
    }
    public GameObject CreateChunk(Point pos)
    {
        GameObject chunk = chunkLoader.TakeFromPool();
        chunk.name = "Chunk[" + pos.x + ", " + pos.y + "]";
        chunk.transform.localPosition = (Vector3.zero);
        TileChunk chunkData = chunk.AddComponent<TileChunk>();
        chunkData.setPosition(pos);
        chunkData.setPrefabs(colPrefab, tilePrefab);
        return chunk;
    }
    public void RemoveChunk(Point p)
    {
        GameObject chunk = chunkMap.ContainsKey(p) ? chunkMap[p] : null;
        if (chunk == null)
            return;

        TileChunk tileChunk = chunk.GetComponent<TileChunk>();
        tileChunk.RemoveChunk();
        chunkMap.Remove(p);
        chunkLoader.GiveToPool(chunk);
    }
    public static Vector3 getTilePosition(Vector3 position, bool log = false)
    {
        Vector3 tilePosition = Vector3.zero;
        if (position.x > 0)
            tilePosition.x = (position.x / Tile.getGridLength()) % TileChunk.length;
        else if (position.x < 0)
        {
            tilePosition.x = (position.x / Tile.getGridLength()) % TileChunk.length;
            tilePosition.x += TileChunk.length;
        }

        if (position.z > 0)
            tilePosition.z = (position.z / Tile.getGridLength()) % TileChunk.length;
        else if (position.z < 0)
        {
            tilePosition.z = (position.z / Tile.getGridLength()) % TileChunk.length;
            tilePosition.z += TileChunk.length;
        }
        
        if (position.y > 0)
            tilePosition.y = (position.y / Tile.getGridLength()) % TileChunk.length;
        else if (position.y < 0)
        {
            tilePosition.y = (position.y / Tile.getGridLength()) % TileChunk.length;
            tilePosition.y += TileChunk.length;
        }

        if (log)
             Debug.Log(tilePosition);
        return tilePosition;
    }
    public static Point getChunkPos(Vector3 position, bool log = false)
    {
        Point chunkPOS = Point.zero;
        Vector3 pos = position / (Tile.getGridLength() * TileChunk.length);
        if (pos.x > 0)
            chunkPOS.x = (int)pos.x;
        else if(position.x < 0)
        {
            chunkPOS.x = (int)Mathf.Floor(pos.x);
        }

        if (pos.z > 0)
            chunkPOS.y = (int)pos.z;
        else if (pos.z < 0)
        {
            chunkPOS.y = (int)Mathf.Floor(pos.z);
        }
        if (log)
            Debug.Log(pos + " => " + chunkPOS);
        return chunkPOS;
    }
    public static Point getBounds(int dimension)
    {
        switch(dimension)
        {
            case 0:
                return boundsX;
            case 1:
                return boundsZ;
            default:
                return Point.zero;
        }
    }
    public static bool IsInBounds(Point pos, Point xDimension, Point yDimension)
    {
        return pos.x <= xDimension.y && pos.x >= xDimension.x && pos.y <= yDimension.y && pos.y >= yDimension.x;
    }
    public static bool IsInBounds(Point pos)
    {
        Point tileSize = new Point(0, TileChunk.length);
        return IsInBounds(pos, tileSize, tileSize);
    }
    public string BoundaryStr()
    {
        return "[Zmax:" + boundsZ.y + " Xmax: " + boundsX.y + "Zmin: " + boundsZ.x + "Xmin: " + boundsX.x + "]";
    }

    public void LoadData(GameData gameData)
    {
        foreach (Point p in gameData.Chunks.Keys)
        {
            if (chunkMap.ContainsKey(p))
                RemoveChunk(p);
            chunkMap.Add(p,CreateChunk(p));
            chunkMap[p].GetComponent<TileChunk>().LoadData(gameData);
        }
    }


    public void SaveData(ref GameData gameData)
    {
    }
}

