using UnityEngine;
using System.Collections;

public class HazzardController : MonoBehaviour
{
    private float _spawnTimer;
    private int _currentPlatformNumber;
    private bool _reachedMinPlatform;
    private int _hazzardCount;

    public Transform player;
    public float spawnInterval = 10;
    public int minPlatformNumber = 100;
    public int maxHazzards = 5;
    public float initialHorizontalForce = 10;
    public float initialVerticalForce = 10;
    [Range(-4, 4)]
    public float minHorizonalSpawnPosition;
    [Range(-4, 4)]
    public float maxHorizonalSpawnPosition;

    void OnEnable()
    {
        KillBox.On_HazzardDestroy += HandleOnHazzardDestroy;
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
        KillBox.On_HazzardDestroy -= HandleOnHazzardDestroy;
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

            if (_spawnTimer <= 0 && _hazzardCount < maxHazzards)
            {
                SpawnHazzard();
                _spawnTimer = spawnInterval;
            }
        }
    }

    void SpawnHazzard()
    {

        //Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, _player.position.z));
        //Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _player.position.z));

        /*float maxY = _rightBoundary.position.y + (_leftBoundary.localScale.y / 2);
        float minY = _rightBoundary.position.y - (_leftBoundary.localScale.y / 2);*/

        var hazzard = Instantiate(Resources.Load<GameObject>("Prefabs/Hazzards/Meteor"),
            new Vector3(Random.Range(minHorizonalSpawnPosition, maxHorizonalSpawnPosition), transform.position.y, player.position.z),
            Quaternion.identity) as GameObject;

        if (hazzard == null) return;

        _hazzardCount++;

        //var hazzardMovement = hazzard.GetComponent<HazzardMovement>();
        hazzard.rigidbody.AddForce(new Vector3(Vector3.left.x * initialHorizontalForce, Vector3.down.y * initialVerticalForce));

        //Debug.Log("TopLeft is: "+ topLeft);
        //Debug.Log("TopRight is: " + topLeft);
        /*hazzardMovement.targetPosition = new Vector3(
            _leftBoundary.position.x,
            hazzard.transform.position.y, 
            _player.position.z);*/
    }

    void HandleOnHazzardDestroy()
    {
        _hazzardCount--;
    }
}
