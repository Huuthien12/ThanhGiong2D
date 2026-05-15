using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;  // Singleton

    // Âm thanh hiệu ứng
    public AudioClip eatSound;
    public AudioClip hitSound;
    public AudioClip healSound;

    // Nhạc nền
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float masterVolume = 0.8f;

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Giữ âm thanh khi chuyển map
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Tạo AudioSource cho SFX
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // Tạo AudioSource cho nhạc nền
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        // Phát nhạc nền nếu có
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume * masterVolume);
        }
    }

    public void PlayEatSound()
    {
        PlaySound(eatSound, 0.7f);
    }

    public void PlayHitSound()
    {
        PlaySound(hitSound, 0.5f);
    }

    public void PlayHealSound()
    {
        PlaySound(healSound, 0.6f);
    }

    // Điều chỉnh nhạc nền
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
    }
}