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
    public AudioClip player_death;
    public AudioClip enemy_death;
    public AudioClip rangedAttack;
    public AudioClip closeAttack_01;
    public AudioClip closeAttack_02;
    public AudioClip jump_01;
    public AudioClip jump_02;
    public AudioClip walk;
    public AudioClip landing;
    public AudioClip dash;
    public AudioClip player_hit;
    public AudioClip enemy_hit;
    

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
