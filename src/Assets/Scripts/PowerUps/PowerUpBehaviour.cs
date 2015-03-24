﻿using System.Linq;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PowerUpBehaviour : MonoBehaviour
{
    private Animator _animator;
    private Light _light;
    private bool _shouldOrbitAroundPlayer;
    private Vector3 _newLocation = Vector3.zero;
    private AudioSource _powerUpAudio;
    private Transform _model;
    private Vector3 _initialSize;

    public float pickUpPower = 2f;
    public float lightDimSpeed = 5f;
    public float lightIntensity = 10f;

    //orbit
    public Transform orbitCenter;
    public Vector3 axis = Vector3.up;
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float orbitSpeed = 10.0f;
    public AudioClip powerUpSound;

    //public Vector3 newLocation = Vector2.zero;

    public delegate void PowerPickUp(float powerToAdd);
    public static event PowerPickUp OnPowerPickUp;

    // Subscribe to events
    void OnEnable()
    {
        PowerUpController.OnNewPowerUpLocation += HandleOnNewPowerUpLocation;
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
        PowerUpController.OnNewPowerUpLocation -= HandleOnNewPowerUpLocation;
    }

    // Use this for initialization
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _light = transform.FindChild("Light").GetComponent<Light>();
        _powerUpAudio = GetComponent<AudioSource>();
        _model = transform.FindChild("Egg");
        _initialSize = _model.localScale;
    }

    // Update is called once per frame
    private void Update()
    {
        _light.intensity = Mathf.Lerp(_light.intensity, (_shouldOrbitAroundPlayer || _newLocation != Vector3.zero) ? 0 : lightIntensity, lightDimSpeed * Time.deltaTime);

        _model.localScale = Vector3.Lerp(_model.localScale, (_shouldOrbitAroundPlayer || _newLocation != Vector3.zero)
           ? new Vector3(0.001f, 0.001f, 0.001f)
           : _initialSize, 5f * Time.deltaTime);

        if (_newLocation != Vector3.zero)
        {
            if (_light.intensity <= 0.01f)
            {
                transform.parent.position = _newLocation;
                Reactivate();
            }
        }

        if (_shouldOrbitAroundPlayer && orbitCenter != null)
        {
            transform.parent.RotateAround(orbitCenter.position, axis, orbitSpeed);
            var desiredPosition = (transform.parent.position - orbitCenter.position).normalized * radius + orbitCenter.position;
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, desiredPosition, radiusSpeed);
        }
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.layer != 9) return;

        _animator.enabled = false;
        particleSystem.Stop();
        if (powerUpSound != null && powerUpSound.isReadyToPlay)
        {
            _powerUpAudio.PlayOneShot(powerUpSound);
        }
        collider.enabled = false;
        if (OnPowerPickUp != null)
        {
            OnPowerPickUp(pickUpPower);
        }
        _shouldOrbitAroundPlayer = true;
        orbitCenter = otherCollider.transform.root;
    }

    public void Reactivate()
    {
        _animator.enabled = true;
        particleSystem.Play();
        collider.enabled = true;
        _newLocation = Vector3.zero;
    }

    private void HandleOnNewPowerUpLocation(Vector3 location, string transformName)
    {
        //Debug.Log("New pick-up position: " + location);
        //transform.parent.position = location;
        particleSystem.Stop();
        _shouldOrbitAroundPlayer = false;
        orbitCenter = null;
        _newLocation = (transform.parent.name == transformName) ? location : new Vector3(0, -5.9f, 9.48f); // TODO move to editor
        collider.enabled = false;
        //Reactivate();
    }

}
