using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class DataPersistenceManager : MonoBehaviour
{
    private GameData gData;
    [SerializeField]
    string fileName;
    [SerializeField]
    TextMeshProUGUI fileNameText;
    
    private FileDataHandler dataHandler;
    private List<IDataPersistance> dataPersistanceObjects;
    [SerializeField]
    public static DataPersistenceManager instance { get; private set; }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("More than one Data Persistence Manager in the scene!");
    }
    public void Start()
    {
        gData = new GameData();

        if (fileNameText)
            fileName = fileNameText.text;
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }
    public void SaveGame()
    {
        if (fileName != fileNameText.text)
        {
            fileName = fileNameText.text;
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }

        this.dataPersistanceObjects = FindAllPersData();
        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref gData);
        }

        dataHandler.Save(gData);
    }
    public void LoadGame()
    {
        if (fileName != fileNameText.text)
        {
            fileName = fileNameText.text;
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }


        this.gData = dataHandler.Load();
        this.dataPersistanceObjects = FindAllPersData();

        foreach (IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(gData);
        }
    }
    List<IDataPersistance> FindAllPersData()
    {
        IEnumerable<IDataPersistance> dataPersistances = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();
        return new List<IDataPersistance>(dataPersistances);
    }
}
