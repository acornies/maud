using UnityEngine;
using System.Collections;

public class CloudSpawn : MonoBehaviour
{

    private float _spawnTimer;
    private int _currentPlatformNumber;
    private bool _reachedMinPlatform;
    private int _cloudCount;

    public Transform leftBoundary;
    public Transform player;
    public float spawnInterval = 10;
    public int minPlatformNumber = 100;
    public int maxClouds = 5;

    public virtual void OnEnable()
    {
        CloudBehaviour.On_CloudDestroy += HandleOnCloudDestroy;
    }

    public virtual void OnDisable()
    {
        UnsubscribeEvent();
    }

    public virtual void OnDestroy()
    {
        UnsubscribeEvent();
    }

    public virtual void UnsubscribeEvent()
    {
        CloudBehaviour.On_CloudDestroy -= HandleOnCloudDestroy;
    }

    void Awake()
    {
    }
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _currentPlatformNumber = PlatformController.Instance.GetCurrentPlatformNumber();
        if (_currentPlatformNumber >= minPlatformNumber)
        {
            _reachedMinPlatform = true;
        }

        if (_currentPlatformNumber >= minPlatformNumber || _reachedMinPlatform)
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0 && _cloudCount < maxClouds)
            {
                SpawnCloud();
                _spawnTimer = spawnInterval;
            }
        }
    }

    void SpawnCloud()
    {
        var cloud = Instantiate(Resources.Load<GameObject>("Prefabs/Clouds/CloudProto_1"),
            new Vector3(transform.position.x, Random.Range((transform.position.y - transform.localScale.y/2), (transform.position.y + transform.localScale.y/2)), player.position.z),
            Quaternion.identity) as GameObject;

        if (cloud == null) return;

        _cloudCount++;
        cloud.renderer.material.color = new Color(1, 1, 1, 0.6f);


        var cloudBehaviour = cloud.GetComponent<CloudBehaviour>();
        if (leftBoundary != null && cloudBehaviour != null)
        {
            cloudBehaviour.targetPosition = new Vector3(leftBoundary.position.x, cloud.transform.position.y, cloud.transform.position.z);
            cloudBehaviour.speed = Random.Range(.5f, 1.5f);
        }
    }

    void HandleOnCloudDestroy()
    {
        _cloudCount--;
    }
}
