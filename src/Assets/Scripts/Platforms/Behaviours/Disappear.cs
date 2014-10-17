using UnityEngine;
using System.Collections;

public class Disappear : PlatformBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    public float timer = 0.0f;
    public float interval = 3.0f;
    public bool isInvisible;

    public delegate void PlatformReappear(Transform reappearingObj);
    public static event PlatformReappear OnPlatformReappear;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (child == null) return;
        _initialPosition = child.localPosition;
        _initialRotation = child.localRotation;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        timer += Time.deltaTime;

        if (isInvisible)
        {
            if (!(timer >= interval)) return;
            /*GameObject copyPlatform = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Platforms/5_Platform"),
                _initialPosition, Quaternion.identity);

            var newPlatform = copyPlatform.transform.FindChild("Cube");
            newPlatform.parent = null;
            Destroy(copyPlatform);

            newPlatform.transform.parent = transform;
            newPlatform.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            newPlatform.transform.localPosition = _initialPosition;
            newPlatform.transform.localRotation = _initialRotation;
            child = newPlatform.transform;
            */
            child.gameObject.SetActive(true);
            timer = 0;
            isInvisible = false;
            if (OnPlatformReappear != null)
            {
                OnPlatformReappear(transform);
            }
        }
        else
        {
            if (!(timer >= interval)) return;
            Transform platformToDestroy = child;
            if (platformToDestroy == null) return;
            if (isOnPlatform)
            {
                var player = platformToDestroy.FindChild("Player");
                if (player != null)
                {
                    //Debug.Log("Found player, unparenting...");
                    player.parent = null;
                }
            }
            //Destroy(platformToDestroy.gameObject);
            child.gameObject.SetActive(false);
            timer = 0;
            isInvisible = true;
        }
    }
}
