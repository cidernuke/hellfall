using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-----Audio Sources-----")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("-----Audio Clips-----")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip rangedAttack;
    public AudioClip closeAttack_01;
    public AudioClip closeAttack_02;
    public AudioClip jump;
    public AudioClip walk;
    

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
