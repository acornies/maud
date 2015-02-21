using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class KillBox : MonoBehaviour
{
    //public static KillBox Instance { get; private set; }
    public float cameraPositionBuffer = 8.0f;

    public delegate void PlayerDeath();
    public static event PlayerDeath On_PlayerDeath;

    public delegate void HazzardDestroy();
    public static event HazzardDestroy On_HazzardDestroy;

    // Subscribe to events
    void OnEnable()
    {
        //CameraMovement.On_CameraUpdatedMinY += UpdateKillBoxAndCheckpointPosition;
        On_PlayerDeath += HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath += HandleOnPlayerDeath;
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
        //CameraMovement.On_CameraUpdatedMinY -= UpdateKillBoxAndCheckpointPosition;
        On_PlayerDeath -= HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath -= HandleOnPlayerDeath;
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

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 9 && !GameController.Instance.playerIsDead)
        {
            On_PlayerDeath();
        }

        if (collider.tag != "Hazzard") return;

        Destroy(collider.gameObject);
        On_HazzardDestroy();
    }

    void UpdateKillBoxAndCheckpointPosition(float newYPosition)
    {
        transform.position = new Vector3(transform.position.x, newYPosition - cameraPositionBuffer, transform.position.z);
    }

    void HandleOnPlayerDeath()
    {
        if (audio.clip != null && audio.clip.isReadyToPlay && !audio.isPlaying)
        {
            audio.Play();   
        }
    }
}
