using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private string fileName;
    private string fileLocation;
    public FileDataHandler(string location, string fileName)
    {
        this.fileLocation = location;
        this.fileName = fileName;
    }

    public GameData Load()
    {
        string path = Path.Combine(this.fileLocation, fileName);
        GameData loadData = null;
        if(File.Exists(path))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error Reading Data: " + path + "\n" + e.Message);
            }
        }
        return loadData;
    }
    public void Save(GameData data)
    {
        string path = Path.Combine(this.fileLocation, fileName);
        Debug.Log(path);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            string dataToStore = JsonUtility.ToJson(data,false);

            using(FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error while saving: " + path + "\n" + e.Message);
        }
    }
}
