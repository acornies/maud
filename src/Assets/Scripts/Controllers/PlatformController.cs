﻿using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using LegendPeak.Platforms;

public class PlatformController : MonoBehaviour
{
    //private Transform _currentPlatformObject;
    private int _currentPlatform;
    private PlatformBuilder _platformBuilder;
    //private float _timer;

    public static PlatformController Instance { get; private set; }
    public IDictionary<int, GameObject> levelPlatforms { get; private set; }
    //public int maxPlatformsForLevel = 100;
    public int checkpointBuffer = 3;
    public float startingYAxisValue = 1.0f;
    public int platformSpawnBuffer = 3;
    public float platformSpacing = 2.1f;
    public float maxRotationLeft = 50.0f;
    public float maxRotationRight = 310.0f;
    public float platformSpawnInterval = 1.0f;
	public float trunkZAxis = 3.8f;

    public GameObject[] platformTypes;

    public delegate void ReachedNextCheckpoint(int platform, int childPlatformToDeleteIndex);
    public static event ReachedNextCheckpoint On_ReachedCheckpoint;

    public delegate void NewPlatform(float yPosition);
    public static event NewPlatform On_NewPlatform;

    // Subscribe to events
    void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandlePlatformReached;
        CameraMovement.On_DestroyLowerPlatforms += HandleDestroyLowerPlatforms;
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
        PlayerMovement.On_PlatformReached -= HandlePlatformReached;
        CameraMovement.On_DestroyLowerPlatforms -= HandleDestroyLowerPlatforms;
    }

    void Awake()
    {
        if (Instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _platformBuilder = new PlatformBuilder();
        levelPlatforms = new Dictionary<int, GameObject>();

    }

    // Use this for initialization
    IEnumerator Start()
    {
        var startingPlatforms = GameObject.FindGameObjectsWithTag("Rotatable");
        foreach (var platform in startingPlatforms)
        {
            var key = platform.name.Split('_')[1];
            var value = platform;
            levelPlatforms.Add(int.Parse(key), value);
        }
        levelPlatforms.OrderBy(x => x.Key);

        while (true)
        {
            yield return StartCoroutine("SpawnPlatforms");
        }
    }

    public int GetCurrentPlatformNumber()
    {
        return _currentPlatform;
    }

    // Spawn platforms based on player location
    IEnumerator SpawnPlatforms()
    {
        //_timer -= Time.deltaTime;
        var highestPlatformIndex = (levelPlatforms.Keys.Count > 0) ? levelPlatforms.Keys.Max() : 0;
        //Debug.Log("Highest platform is: " + highestPlatformIndex);

        if (_currentPlatform + platformSpawnBuffer > highestPlatformIndex)
        {
            //GameObject highestPlatformObj;
            GameObject highestPlatform = null;
            float yAxisMultiplier;
            levelPlatforms.OrderBy(p => p.Key);

            int nextPlatform = highestPlatformIndex + 1;
            if (levelPlatforms.TryGetValue(highestPlatformIndex, out highestPlatform))
            {
                yAxisMultiplier = highestPlatform.transform.position.y + highestPlatform.transform.localScale.y +
                                  platformSpacing;
            }
            else
            {
                yAxisMultiplier = startingYAxisValue;
            }

            var toRange = nextPlatform + platformSpawnBuffer;
            for (int i = nextPlatform; i <= toRange; i++)
            {

                GameObject newPlatform;
                //if (!(timer <= 0)) return;

                if (!levelPlatforms.TryGetValue(i, out newPlatform))
                {

                    newPlatform =
                        (GameObject)Instantiate(platformTypes[_platformBuilder.GetPlatformPrefabByNumber(i)],
                            new Vector3(0, yAxisMultiplier, trunkZAxis), Quaternion.identity);

                    yAxisMultiplier += newPlatform.transform.localScale.y + platformSpacing;

                    newPlatform.name = string.Format("Platform_{0}", i);
                    newPlatform.transform.localRotation = Quaternion.AngleAxis(GetRandomRotation(), Vector3.up);
                    AdjustProperties(newPlatform, i);
                    levelPlatforms.Add(i, newPlatform);

                    On_NewPlatform(newPlatform.transform.position.y);
                    //_timer = platformSpawnInterval;
                }
            }
        }

        Debug.Log("Platform creation finished. Waiting " + platformSpawnInterval + "s");
        yield return new WaitForSeconds(platformSpawnInterval);
    }

    private void AdjustProperties(GameObject newPlatform, int index)
    {
        UpAndDown upAndDownComponent = newPlatform.transform.GetComponent<UpAndDown>();
        Drop dropComponent = newPlatform.transform.GetComponent<Drop>();
        if (index > 40)
        {
            if (dropComponent != null) //TODO: change to Editor value
            {
                dropComponent.enabled = true;
            }
        }
        if (index > 60)
        {
            if (upAndDownComponent != null) //TODO: change to Editor value
            {
                upAndDownComponent.speed = 3f;
                upAndDownComponent.waitTime = .5f;
            }
        }
    }

    private float GetRandomRotation()
    {
        var options = new Dictionary<int, float> { { 1, maxRotationLeft }, { 2, maxRotationRight } };
        var leftOrRight = Random.Range(1, 3);
        return Random.Range(leftOrRight == 1 ? 0 : 360f, options[leftOrRight]);
    }

    void HandlePlatformReached(Transform platform, Transform player)
    {
        //Debug.Log (platform.parent.name);
        if (platform.parent == null) return;

        _currentPlatform = int.Parse(platform.parent.name.Split('_')[1]);

        //Debug.Log ("Current platform: " + _currentPlatform);

        int newcheckpoint = (_currentPlatform - checkpointBuffer);
        On_ReachedCheckpoint(newcheckpoint, newcheckpoint - 2);
    }

    void HandleDestroyLowerPlatforms(int platformIndex, int childPlatformToDeleteIndex)
    {
        //Debug.Log ("Destroy platforms under: " + platformIndex);
        DestroyChildPlatformUnderCheckpoint(childPlatformToDeleteIndex);

        var buffer = (from platform in levelPlatforms where platform.Key <= platformIndex select platform.Key).ToList();

        foreach (var item in buffer)
        {
            levelPlatforms.Remove(item);
            Destroy(GameObject.Find("Platform_" + item));
            //Debug.Log("Removed Platform_" + item);
        }
    }

    private void DestroyChildPlatformUnderCheckpoint(int platformIndex)
    {
        GameObject parent;
        if (!this.levelPlatforms.TryGetValue(platformIndex, out parent)) return;
        if (parent == null) return;
        var child = parent.transform.Find("Cube");
        if (child == null) return;
        Destroy(child.gameObject);
    }
}
