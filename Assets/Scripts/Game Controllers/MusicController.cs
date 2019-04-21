using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    public static MusicController instance;

    public AudioClip[] audioClips;

    [HideInInspector]
    public AudioSource audioSource;
    // Start is called before the first frame update


    /*void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    void Awake()
    {
        CreateInstance();
        audioSource = GetComponent<AudioSource>();
    }

    void CreateInstance()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBgMusic()
    {
        if (!audioSource.isPlaying)
        {
            AudioClip bgMusic = audioClips[0];
            if (bgMusic)
            {
                audioSource.clip = bgMusic;
                audioSource.loop = true;
                audioSource.volume = 0.6f;
                audioSource.Play();
            }
        }
    }

    public void GameplaySound()
    {
        if (!audioSource.isPlaying)
        {
            AudioClip gameplaySound = audioClips[1];
            if (gameplaySound)
            {
                audioSource.clip = gameplaySound;
                audioSource.loop = true;
                audioSource.volume = 0.6f;
                audioSource.Play();
            }
        }
    }

    public void StopAllSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
