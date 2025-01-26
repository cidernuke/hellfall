using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-----Audio Sources-----")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("-----Audio Clips-----")]

    [Header("-Music")]
    public AudioClip background;

    [Header("-Attack")]
    public AudioClip player_hit;
    public AudioClip player_death;
    public AudioClip enemy_hit;
    public AudioClip enemy_death;
    public AudioClip rangedAttack;
    public AudioClip closeAttack_01;
    public AudioClip closeAttack_02;

    [Header("-Movement")]
    public AudioClip jump_01;
    public AudioClip jump_02;
    public AudioClip walk;
    public AudioClip landing;
    public AudioClip dash;

    [Header("-Enviroment")]

    public AudioClip checkpoint;
    public AudioClip open_door;
    
    

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
    
}
