using UnityEngine;

/// <summary>
/// ScriptableObject lưu trữ tất cả các âm thanh trong game
/// </summary>
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Hexa Sort/Audio/Sound Library")]
public class SoundLibrarySO : ScriptableObject
{
    [Header("Background Music")]
    [Tooltip("Nhạc nền cho màn hình chính")]
    public SoundSO BGM_MainMenu;
    [Tooltip("Nhạc nền cho gameplay")]
    public SoundSO BGM_GameLoop;
    
    [Header("Game SFX")]
    [Tooltip("Âm thanh khi đặt hexagon")]
    public SoundSO SFX_HexagonPlace;
    [Tooltip("Âm thanh khi hợp nhất hexagon")]
    public SoundSO SFX_HexagonMerge;
    [Tooltip("Âm thanh khi xóa hexagon")]
    public SoundSO SFX_HexagonVanish;
    [Tooltip("Âm thanh khi hiển thị stack mới")]
    public SoundSO SFX_StackShow;
    
    [Header("UI SFX")]
    [Tooltip("Âm thanh khi nhấn nút")]
    public SoundSO UI_ButtonClick;
    [Tooltip("Âm thanh khi hoàn thành màn chơi")]
    public SoundSO SFX_LevelComplete;
    [Tooltip("Âm thanh khi thất bại")]
    public SoundSO SFX_LevelFail;
}