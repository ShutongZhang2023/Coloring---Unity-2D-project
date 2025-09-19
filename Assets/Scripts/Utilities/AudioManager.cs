using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Scene BGM Clips")]
    public AudioClip level1BGM;
    //public AudioClip level2BGM;

    private Dictionary<string, AudioClip> sceneBGMMap;

    [Header("SFX Clips")]
    public AudioClip jumpClip;
    public AudioClip hurtClip;
    public AudioClip deadClip;
    public AudioClip coinClip;
    public AudioClip eatClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeBGMMap();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeBGMMap()
    {
        sceneBGMMap = new Dictionary<string, AudioClip>
        {
            { "ColorTest", level1BGM },
            //{ "Level2", level2BGM },
        };
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneBGMMap.TryGetValue(scene.name, out AudioClip clip))
        {
            if (bgmSource.clip != clip && clip != null)
            {
                bgmSource.clip = clip;
                bgmSource.Play();
            }
        }
    }

    public void PlayJumpSFX()
    {
        sfxSource.PlayOneShot(jumpClip);
    }

    public void PlayHurtSFX()
    {
        sfxSource.PlayOneShot(hurtClip);
    }

    public void PlayDeadSFX()
    {
        sfxSource.PlayOneShot(deadClip);
    }

    public void PlayCoinSFX()
    {
        sfxSource.PlayOneShot(coinClip);
    }

    public void PlayEatSFX()
    {
        sfxSource.PlayOneShot(eatClip);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
