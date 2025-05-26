using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý hệ thống âm thanh của trò chơi 
/// Thiết kế đơn giản, dễ mở rộng
/// </summary>
public class AudioService : MonoBehaviour
{
    public static AudioService Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    [Header("Sound Library")]
    [SerializeField] private SoundLibrarySO _soundLibrary;

    // Audio Sources
    private AudioSource _musicSource;
    
    // Quản lý nhạc nền theo màn hình
    [SerializeField] private bool _autoPlayMusicOnSceneLoad = true;
    private readonly Dictionary<string, SoundSO> _musicByScene = new Dictionary<string, SoundSO>();
    
    // Constants for PlayerPrefs keys
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";

    #region Unity Lifecycle

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Initialization

    private void InitializeAudio()
    {
        // Khởi tạo AudioSource cho nhạc nền
        GameObject musicObj = new GameObject("MusicSource");
        musicObj.transform.SetParent(transform);
        _musicSource = musicObj.AddComponent<AudioSource>();
        _musicSource.outputAudioMixerGroup = _musicMixerGroup;
        _musicSource.loop = true;
        
        // Ánh xạ nhạc nền cho từng màn hình
        MapBackgroundMusic();
        
        // Tải các cài đặt âm lượng
        LoadVolumeSettings();
    }
    
    private void MapBackgroundMusic()
    {
        // Thiết lập ánh xạ giữa cảnh và nhạc nền
        _musicByScene.Clear();
        
        // Các mapping cơ bản
        _musicByScene.Add("MainMenuScreen", _soundLibrary.BGM_MainMenu);
        _musicByScene.Add("GameplayScreen", _soundLibrary.BGM_GameLoop);
        
        // Dễ dàng thêm các cảnh mới ở đây
    }
    
    private void LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    #endregion

    #region Scene Management

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!_autoPlayMusicOnSceneLoad) return;
        
        string sceneName = scene.name;
        
        // Tự động phát nhạc tương ứng với cảnh
        if (_musicByScene.TryGetValue(sceneName, out SoundSO bgm))
        {
            PlayMusic(bgm);
        }
    }

    #endregion

    #region Sound Playback

    /// <summary>
    /// Phát nhạc nền
    /// </summary>
    public void PlayMusic(SoundSO music)
    {
        if (music == null || music.Clip == null)
            return;
            
        // Nếu đang phát cùng một nhạc, không làm gì cả
        if (_musicSource.clip == music.Clip && _musicSource.isPlaying)
            return;
            
        // Crossfade nếu đang phát nhạc khác
        StartCoroutine(CrossfadeMusic(music));
    }
    
    /// <summary>
    /// Thực hiện crossfade khi chuyển nhạc
    /// </summary>
    private IEnumerator CrossfadeMusic(SoundSO newMusic)
    {
        float fadeTime = 0.5f;
        float startVolume = _musicSource.volume;
        
        // Fade out
        if (_musicSource.isPlaying)
        {
            float timer = 0;
            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                _musicSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeTime);
                yield return null;
            }
        }
        
        // Đổi nhạc
        _musicSource.clip = newMusic.Clip;
        _musicSource.volume = 0;
        _musicSource.Play();
        
        // Fade in
        float timer2 = 0;
        while (timer2 < fadeTime)
        {
            timer2 += Time.deltaTime;
            _musicSource.volume = Mathf.Lerp(0, startVolume, timer2 / fadeTime);
            yield return null;
        }
        
        _musicSource.volume = startVolume;
    }
    
    /// <summary>
    /// Phát âm thanh một lần (chỉ dành cho SFX, không dành cho nhạc nền)
    /// </summary>
    public AudioSource PlaySound(SoundSO sound)
    {
        if (sound == null || sound.Clip == null)
            return null;
            
        // Cảnh báo nếu cố gắng phát nhạc nền qua PlaySound
        if (sound.Loop)
        {
            Debug.LogWarning($"Âm thanh '{sound.name}' có Loop=true. Hãy sử dụng PlayMusic() thay vì PlaySound() cho nhạc nền!");
        }
            
        GameObject soundObj = new GameObject($"Sound_{sound.name}");
        
        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = sound.Clip;
        source.outputAudioMixerGroup = (sound.AudioType == SoundSO.AudioTypes.Music) ? _musicMixerGroup : _sfxMixerGroup;
        source.volume = sound.Volume;
        source.pitch = sound.RandomizePitch 
            ? UnityEngine.Random.Range(sound.Pitch - sound.PitchVariation, sound.Pitch + sound.PitchVariation)
            : sound.Pitch;
        source.loop = false; // Luôn false cho SFX
        source.spatialBlend = 0f; // 2D sound
        source.Play();
        
        // Tự động xóa GameObject sau khi âm thanh phát xong
        Destroy(soundObj, sound.Clip.length);
        
        return source;
    }


    #endregion

    #region Game-specific Sound Controls

    /// <summary>
    /// Phát âm thanh khi nhấn nút
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySound(_soundLibrary.UI_ButtonClick);
    }
    
    /// <summary>
    /// Phát âm thanh khi đặt hexagon
    /// </summary>
    public void PlayHexagonPlace()
    {
        PlaySound(_soundLibrary.SFX_HexagonPlace);
    }
    
    /// <summary>
    /// Phát âm thanh khi hợp nhất hexagon
    /// </summary>
    public void PlayHexagonMerge()
    {
        PlaySound(_soundLibrary.SFX_HexagonMerge);
    }
    
    /// <summary>
    /// Phát âm thanh khi hexagon biến mất
    /// </summary>
    public void PlayHexagonVanish()
    {
        PlaySound(_soundLibrary.SFX_HexagonVanish);
    }
    
    /// <summary>
    /// Phát âm thanh khi hiển thị stack mới
    /// </summary>
    public void PlayStackShow()
    {
        PlaySound(_soundLibrary.SFX_StackShow);
    }
    
    /// <summary>
    /// Phát âm thanh khi hoàn thành level
    /// </summary>
    public void PlayLevelComplete()
    {
        PlaySound(_soundLibrary.SFX_LevelComplete);
    }
    
    /// <summary>
    /// Phát âm thanh khi thất bại
    /// </summary>
    public void PlayLevelFail()
    {
        PlaySound(_soundLibrary.SFX_LevelFail);
    }

    #endregion

    #region Volume Control

    /// <summary>
    /// Đặt âm lượng nhạc
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float dbValue = ConvertToDecibel(volume);
        _audioMixer.SetFloat("MusicVolume", dbValue);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Đặt âm lượng hiệu ứng
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float dbValue = ConvertToDecibel(volume);
        _audioMixer.SetFloat("SFXVolume", dbValue);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Lấy âm lượng nhạc
    /// </summary>
    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
    }
    
    /// <summary>
    /// Lấy âm lượng hiệu ứng
    /// </summary>
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }
    
    /// <summary>
    /// Chuyển đổi từ giá trị tuyến tính sang decibel
    /// </summary>
    private float ConvertToDecibel(float linearVolume)
    {
        // Tránh logarithm của 0
        linearVolume = Mathf.Max(0.0001f, linearVolume);
        return Mathf.Log10(linearVolume) * 20f;
    }

    #endregion
} 