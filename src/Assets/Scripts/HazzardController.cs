using UnityEngine;
using System.Collections;

public class HazzardController : MonoBehaviour
{


    private Transform _player;
    private Transform _leftBoundary;
    private Transform _rightBoundary;
    public float _spawnTimer;
    public float spawnInterval = 10;
    //public Vector3 horizontalSpawnPosition;

    void Awake()
    {
        _player = GameObject.Find("Player").transform;
        _leftBoundary = GameObject.Find("LeftBoundary").transform;
        _rightBoundary = GameObject.Find("RightBoundary").transform;
    }
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            SpawnHazzard();
            _spawnTimer = spawnInterval;
        }

    }

    void SpawnHazzard()
    {
        //Debug.Log("Spawn!");
        var hazzard = Instantiate(Resources.Load<GameObject>("Prefabs/Hazzards/Bird"),
            new Vector3(_rightBoundary.position.x, _player.position.y, _player.position.z),
            Quaternion.identity) as GameObject;

        if (hazzard == null) return;

        var hazzardMovement = hazzard.GetComponent<HazzardMovement>();
        hazzardMovement.targetPosition = new Vector3(_leftBoundary.position.x, _player.position.y, _player.position.z);
    }
}
