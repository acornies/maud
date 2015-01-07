using UnityEngine;
using System.Collections;

public class CloudSpawn : MonoBehaviour
{

    private float _spawnTimer;
    private int _currentPlatformNumber;
    private bool _reachedMinPlatform;
    private int _cloudCount;

    public Transform destinationBoundary;
	[Range(-4f, 0)]
	public float minHorizontalRange;
	[Range(0, 4f)]
	public float maxHorizontalRange;
    public Transform player;
    public float spawnInterval = 10;
    public int minPlatformNumber = 100;
    public int maxClouds = 5;
	public int cloudFastStartPlatform = 101;

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

        if (_currentPlatformNumber >= cloudFastStartPlatform)
        {
            maxClouds = 10;
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
		                        new Vector3(Random.Range(minHorizontalRange, maxHorizontalRange), transform.position.y, player.position.z),
            Quaternion.identity) as GameObject;

        if (cloud == null) return;

        _cloudCount++;
        cloud.renderer.material.color = new Color(1, 1, 1, 0.6f);
		cloud.name = "Cloud_" + _cloudCount;

		HeightAdjustments (cloud);
    }

	private void HeightAdjustments(GameObject cloud)
	{
		var cloudBehaviour = cloud.GetComponent<CloudBehaviour>();
		if (destinationBoundary == null || cloudBehaviour == null) return;

		cloudBehaviour.targetPosition = new Vector3(cloud.transform.position.x, destinationBoundary.position.y, cloud.transform.position.z);
		cloudBehaviour.speed = Random.Range(1f, 2f);
		
		if (_currentPlatformNumber >= cloudFastStartPlatform)
		{
		    //maxClouds = 10;
            cloudBehaviour.speed = Random.Range(2f, 3f);
		}

	    cloudBehaviour.cameraSpeed = (cloudBehaviour.cameraSpeed/2);
	}

    void HandleOnCloudDestroy()
    {
        _cloudCount--;
    }
}
