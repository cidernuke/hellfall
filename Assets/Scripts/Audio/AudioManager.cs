using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-----Audio Sources-----")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource SFXSource;
    [SerializeField] private AudioSource SFXSource_walking;

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

    [Header("-Items")]

    public AudioClip healthitem;
    
    [Header("-Settings")]

    // Pitch is exaggerated in this example, normally you'd use 0.9f to 1.1f or similar.
    // Play with minPitch and maxPitch values in the Editor until you achieve the desired effect.
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    
    //Max Volume is 1
    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;

    //Stereo Pan 0 is the middle, + is right side and - is left side
    [SerializeField] private float leftMaxPan;
    [SerializeField] private float rightMaxPan;

    

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);

    }

    public void PlaySFX_walk()
    {
        SFXSource_walking.PlayOneShot(walk);

        float newPitch = Random.Range(minPitch, maxPitch);
        float newVolume = Random.Range(minVolume, maxVolume);
        float newPan = Random.Range(leftMaxPan, rightMaxPan);
        
        SFXSource_walking.pitch = newPitch;
        SFXSource_walking.volume = newVolume;
        SFXSource_walking.panStereo = newPan;
    }
    
}
