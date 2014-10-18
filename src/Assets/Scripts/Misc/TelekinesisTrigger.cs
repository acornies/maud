using UnityEngine;
using System.Collections;

public class TelekinesisTrigger : MonoBehaviour
{

    private TelekinesisController _telekinesisTrigger;
    private ParticleSystem _aura;

    // Use this for initialization
    void Start()
    {
        _telekinesisTrigger = GameObject.Find("TelekinesisControl").GetComponent<TelekinesisController>();
        _aura = transform.FindChild("TeleAura").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + " tele: " + _telekinesisTrigger.enabled);
        if (other.name != "Player" || _telekinesisTrigger.enabled) return;
        //Debug.Log("Turn on tele.");
        _telekinesisTrigger.enabled = true;
        _aura.Play();
        //Destroy(gameObject);
    }
}
