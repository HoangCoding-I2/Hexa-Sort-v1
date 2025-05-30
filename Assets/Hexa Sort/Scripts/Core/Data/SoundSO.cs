using UnityEngine;

/// <summary>
/// ScriptableObject lưu trữ thông tin của một âm thanh
/// </summary>
[CreateAssetMenu(fileName = "NewSound", menuName = "Hexa Sort/Audio/Sound")]
public class SoundSO : ScriptableObject
{
    public enum AudioTypes
    {
        SFX,
        Music
    }
    
    [Header("Sound Properties")]
    public AudioTypes AudioType = AudioTypes.SFX;
    public AudioClip Clip;
    
    [Header("Playback Settings")]
    public bool Loop = false;
    
    [Range(0f, 1f)]
    public float Volume = 1f;
    
    [Range(0.1f, 3f)]
    public float Pitch = 1f;
    
    [Header("Randomization")]
    public bool RandomizePitch = false;
    
    [Range(0f, 1f)]
    public float PitchVariation = 0.1f;
}