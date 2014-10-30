using UnityEngine;
using System.Collections;

public class Disappear : PlatformBehaviour
{
    public float timer = 0.0f;
    public float interval = 3.0f;
    public bool isInvisible;

    public delegate void PlatformReappear(Transform reappearingObj);

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        if (child == null) return;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        timer += Time.deltaTime;

        if (isInvisible)
        {
            if (!(timer >= interval)) return;
            if (child == null) return;
            child.gameObject.SetActive(true);
            timer = 0;
            isInvisible = false;
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
