using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    private float volume = 1f;
    
    public static MusicManager Instance;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        
        _audioSource = GetComponent<AudioSource>();
  
    }

    private void Start()
    {
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.7f);
        _audioSource.volume = volume;
    }

    public void ChangeVolume(float amount)
    {
        volume = amount;

        _audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}