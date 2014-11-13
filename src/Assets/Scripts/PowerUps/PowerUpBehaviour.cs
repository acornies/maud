using System.Linq;
using UnityEngine;
using System.Collections;

public class PowerUpBehaviour : MonoBehaviour
{
    private Animator _animator;

    public float pickUpPower = 2f;

    public delegate void PowerPickUp(float powerToAdd);
    public static event PowerPickUp OnPowerPickUp;

    // Use this for initialization
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.name != "Player") return;

        _animator.enabled = false;
        particleSystem.Stop();

        if (OnPowerPickUp != null)
        {
            OnPowerPickUp(pickUpPower);
        }
    }

    public void Reactivate()
    {
        _animator.enabled = true;
        particleSystem.Play();
    }
}
