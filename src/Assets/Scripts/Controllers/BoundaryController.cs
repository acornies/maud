using UnityEngine;

public class BoundaryController : MonoBehaviour
{
    public Transform player;
    //public float boundaryForce = 300f;

    public delegate void PlayerDeath();
    public static event PlayerDeath On_PlayerDeath;

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
        //transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
    }

    void OnTriggerEnter(Collider collider)
    {
        // left and right boundary behaviour
        if (collider.gameObject.layer != 9 && GameController.Instance.playerIsDead) return;
        //BoundaryBounceBack();
        //Debug.Log("Player hit boundary " + transform.name);
        On_PlayerDeath();
    }

    /*void BoundaryBounceBack()
    {
        player.rigidbody.velocity = new Vector2(0, player.rigidbody.velocity.y);
        var dir = (player.position.x - transform.position.x);
        //Debug.Log (dir);
        player.rigidbody.AddForce(dir * boundaryForce, 0, 0);
        player.GetComponent<PlayerMovement>().forcePushed = true;
    }*/
}
