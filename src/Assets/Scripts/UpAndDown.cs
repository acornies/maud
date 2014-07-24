using UnityEngine;
using System.Collections;

public class UpAndDown : MonoBehaviour 
{
	public float speed = 1.0f;
	public float waitTime = 1.0f;
	public bool isMoving = true;
	public Vector3 maxY;
	public Vector3 minY;

	// Use this for initialization
	IEnumerator Start () 
	{
		maxY = new Vector3(transform.position.x, transform.parent.position.y + (transform.parent.localScale.y / 2), transform.position.z);
		minY = new Vector3(transform.position.x, transform.parent.position.y - (transform.parent.localScale.y / 2), transform.position.z);

		while (isMoving) 
		{
			yield return StartCoroutine(MoveObject(transform, minY, maxY, speed));
			yield return StartCoroutine(MoveObject(transform, maxY, minY, speed));
		}
	}
	
	IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
	{
		var i= 0.0f;
		var rate= 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			thisTransform.position = Vector3.Lerp(new Vector3(transform.position.x, startPos.y, transform.position.z), 
			                                      new Vector3(transform.position.x, endPos.y, transform.position.z), i);
			yield return null; 
		}

		yield return new WaitForSeconds(waitTime);
	}
}
