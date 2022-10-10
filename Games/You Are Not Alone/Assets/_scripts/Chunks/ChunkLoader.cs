using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public static string DEAD_CHUNK_NAME = "AvailableChunk";
    Dictionary<Point, GameObject> map;
    EnvironmentHandler environment = null;

    [SerializeField]
    GameObject chunkPrefab;

    List<GameObject> availableChunkMaps = new List<GameObject>();


    List<GameObject> loadChunkList = new List<GameObject>();
    List<GameObject> removeChunkList = new List<GameObject>();

    public event Action<List<GameObject>, List<GameObject>, Dictionary<Point, float>> startQueueEvent;
    public bool updateFlag = false;

    static private bool running = false;
    public bool isRunning { get { return running; } }
    [SerializeField]
    float timeLimit = 0.1f;
    List<TileChunk> tileChunkRenderMap = new List<TileChunk>();

    public void Start()
    {
        startQueueEvent += Run;
    }
    public void OnDestroy()
    {
        startQueueEvent -= Run;
    }

    public void Queue(List<GameObject> queue, List<GameObject> remove, Dictionary<Point, float> mask)
    {
        startQueueEvent?.Invoke(queue, remove, mask);
    }
    private void Run(List<GameObject> queue, List<GameObject> remove, Dictionary<Point, float> mask)
    {
        UpdateLists(queue, remove);
        if (!isRunning)
        {
            running = true;
            StartCoroutine(StartUpdate(mask));
        }
    }
    private void UpdateLists(List<GameObject> queue, List<GameObject> remove)
    {
        updateFlag = true;
        List<GameObject> duplicates = new List<GameObject>();
        foreach(GameObject item in queue)
            if(remove.Contains(item))
                duplicates.Add(item);

        foreach(GameObject item in duplicates)
        {
            remove.Remove(item);
            queue.Remove(item);
        }

        loadChunkList.AddRange(queue);
        removeChunkList.AddRange(remove);
        updateFlag = false;
    }
  
    public void setEnvHan(EnvironmentHandler eH)
    {
        this.environment = eH;
        if (eH == null)
            return;
        InitPool();
    }
    public void InitPool()
    {
        if (chunkPrefab == null)
            throw new System.Exception("No ChunkPrefab assigned for: " + gameObject.name);

        int size = ((int)Mathf.Pow((environment.chunkLength * 2 + 1), 2)) * 3;
        for(int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(chunkPrefab, environment.transform);
            obj.name = DEAD_CHUNK_NAME;
            obj.SetActive(false);
            availableChunkMaps.Add(obj);
        }
    }
    public GameObject TakeFromPool()
    {
        if (availableChunkMaps.Count == 0)
            throw new System.Exception("Error taking From Pool, no GameObject in Pool to Take!!!");

        GameObject returnValue = availableChunkMaps[0];
        availableChunkMaps.RemoveAt(0);
        returnValue.SetActive(true);
        return returnValue;
    }
    public void GiveToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.name = DEAD_CHUNK_NAME;
        availableChunkMaps.Add(obj);
    }

    private IEnumerator StartUpdate(Dictionary<Point, float> mask)
    {
        yield return StartCoroutine(LoadChunks(mask));
        yield return StartCoroutine(CheckForChunksOutOfBounds());
        yield return StartCoroutine(RemoveChunks());
        environment.updateRenderFaces();
        running = false;
    }
    private IEnumerator CheckForChunksOutOfBounds()
    {
       Point xBounds = EnvironmentHandler.getBounds(0);
       Point zBounds = EnvironmentHandler.getBounds(1);
       foreach (GameObject chunkObj in EnvironmentHandler.Map.Values)
       {
                TileChunk tC = chunkObj.GetComponent<TileChunk>();
                Point pos = tC.getPosition;
                if (pos.x > xBounds.y || pos.x < xBounds.x
                        || pos.y > zBounds.y || pos.y < zBounds.x)
                    removeChunkList.Add(chunkObj);
       }
       yield return null;
    }
    private IEnumerator LoadChunks(Dictionary<Point,float> mask)
    {
        map = EnvironmentHandler.Map;
        int seed = EnvironmentHandler.Seed;
        float t = Time.time;
        string debugStr = t.ToString() + "||BUILD  Queue total: " + loadChunkList.Count + '\n'; 

        while (loadChunkList.Count > 0)
        {
            float duration = Time.time + timeLimit;
            int index = 0;

            while (Time.time <= duration && index < loadChunkList.Count && loadChunkList.Count > 0)
            {
                if (updateFlag)
                    break;

                try
                {
                    TileChunk tc = loadChunkList[index].GetComponent<TileChunk>();
                    if (!map.ContainsKey(tc.getPosition))
                    { 
                        debugStr += tc.getPosition.ToString();
                        EnvironmentHandler.Map.Add(tc.getPosition, loadChunkList[index]);
                        tc.InitSeed(seed, mask);
                        tc.SetColliders(false);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(debugStr + "\n Index cut at " + index);
                    Debug.LogException(e);
                    break;
                }
                index++;
                yield return null;
            }
            loadChunkList.RemoveRange(0, index);
            yield return null;
        }
        Debug.Log(debugStr + "||completed in: " + (Time.time - t).ToString());
        loadChunkList.Clear();
    }
    private IEnumerator RemoveChunks()
    {
        map = EnvironmentHandler.Map;
        float initTime = Time.time;
        string debugStr = initTime.ToString() + "||REMOVAL  ";
        while (removeChunkList.Count > 0)
        {
            int index = 0;
            float duration = Time.time + timeLimit;


            while (Time.time <= duration && index < removeChunkList.Count && removeChunkList.Count > 0)
            {
                if (updateFlag)
                    break;


                try
                {
                    if (removeChunkList[index] != null)
                    {
                        GameObject chunk = removeChunkList[index];
                        GiveToPool(chunk);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(debugStr);
                    Debug.LogException(e);
                    break;
                }
                index++;
                yield return null;
            }
            removeChunkList.RemoveRange(0, index);
            yield return null;
        }
        Debug.Log(debugStr + "||completed in " + (Time.time - initTime).ToString());
        removeChunkList.Clear();
    }
}