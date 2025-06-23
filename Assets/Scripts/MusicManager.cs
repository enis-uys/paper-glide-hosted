using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    [SerializeField]
    private Echidna.Audio.MusicTrack musicTrack;

    [SerializeField]
    private float fadeDuration = 0.1f;

    [SerializeField]
    private Echidna.Audio.MusicContainer musicContainer;

    [SerializeField]
    private AudioMixerGroup mixerGroup;

    // Ensure only one instance of MusicManager exists
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    // Make sure audio files are in PCM Compression Format
    void Start()
    {
        musicContainer.Initialize();
        musicContainer.SetMixerGroup(mixerGroup);
        StartMusic();
    }

    public void StartMusic()
    {
        musicContainer.PlayTrack(musicTrack, fadeDuration);
    }
}
