using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchScript : MonoBehaviour
{
    [SerializeField] private GameObject lightObject;
    [SerializeField] private bool isLightOn = true;
    [SerializeField] private AudioClip[] lightSwitchSounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool IsLightOn
    {
        get => isLightOn;
        set => UpdateLightState(value);
    }

    private void UpdateLightState(bool value)
    {
        isLightOn = value;
        audioSource.clip = isLightOn ? lightSwitchSounds[0] : lightSwitchSounds[1];
        audioSource.Play();
        lightObject.SetActive(isLightOn);
    }
}
