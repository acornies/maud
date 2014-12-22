using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;
using LegendPeak.Player;

public class PlayerState : MonoBehaviour
{
    public string dataPath;

    public PlayerState Instance { get; private set; }

    public PlayerData Data { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        this.Load();
    }
    
    // Use this for initialization
    void Start()
    {
    }

    public void Save()
    {
        var binaryFormatter = new BinaryFormatter();
        var playerFile = File.Open(string.Format(dataPath, Application.persistentDataPath), FileMode.Open);
        
        binaryFormatter.Serialize(playerFile, Data);
        playerFile.Close();
    }

    public void Load()
    {
        if (!File.Exists(string.Format(dataPath, Application.persistentDataPath))) return;

        var binaryFormatter = new BinaryFormatter();
        var playerFile = File.Open(string.Format(dataPath, Application.persistentDataPath), FileMode.Open);
        Data = binaryFormatter.Deserialize(playerFile) as PlayerData;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
