using System.Diagnostics;
using UnityEngine;
using System.Collections;

public class UpAndDown : PlatformBehaviour 
{
	public float smoothing = 1.0f;
	public float waitTime = 1.0f;
	public bool isMoving = true;
	public Vector3 maxY;
	public Vector3 minY;

	// Use this for initialization
	IEnumerator Start () 
	{
		base.Start();
        maxY = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), child.position.z);
		minY = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2), child.position.z);

		while (isMoving) 
		{
			yield return StartCoroutine(MoveObject(child, minY, maxY, smoothing));
			yield return StartCoroutine(MoveObject(child, maxY, minY, smoothing));
		}
	}
	
	IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
	{
		var i= 0.0f;
		var rate= 1.0f/time;
		while (i < 1.0f && child != null) {
			i += Time.deltaTime * rate;
            child.rigidbody.MovePosition(Vector3.Lerp(new Vector3(child.position.x, startPos.y, child.position.z), 
			                                      new Vector3(child.position.x, endPos.y, child.position.z), i));
			yield return new WaitForFixedUpdate(); 
		}

		yield return new WaitForSeconds(waitTime);
	}

    public void StopMovement()
    {
        StopAllCoroutines();
    }

    public IEnumerator StartMovement()
    {
       return this.Start();
    }

    /*public override void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform.GetInstanceID() == this.transform.GetInstanceID())
        {
            isOnPlatform = true;
            var difference = (platform.position.y - player.position.y);
            //player.rigidbody.MovePosition(new Vector3(player.position.x, platform.position.y, player.position.z));
            //Debug.Log(difference);
            //player.position = Vector3.MoveTowards(player.position, transform.position, difference);

        }
    }*/
}
