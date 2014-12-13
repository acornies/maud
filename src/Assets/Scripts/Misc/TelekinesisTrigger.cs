using UnityEngine;
using System.Collections;

public class TelekinesisTrigger : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        //_telekinesisTrigger = GameObject.Find("TelekinesisControl").GetComponent<TelekinesisController>();
        //_aura = transform.FindChild("TeleAura").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + " tele: " + _telekinesisTrigger.enabled);
        if (other.name != "Player" || !GameController.Instance.inSafeZone) return;
        Debug.Log("Turn on tele bar.");
        GameController.Instance.UpdateSafeZone(false);
        //GameController.Instance.heightCounter.rectTransform.anchoredPosition = new Vector2(-20f, -20f);
        //Destroy(gameObject);
    }
}
