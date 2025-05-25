using System.Collections;
using UnityEngine;


public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    
    [Header("Popup References")]
    [SerializeField] private LevelCompletePopup _levelCompletePopup;
    [SerializeField] private LevelFailedPopup _levelFailedPopup;
    [SerializeField] private SettingsPopup _settingsPopup;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        // Đảm bảo các popup đều bị tắt khi khởi động
        if (_levelCompletePopup != null)
            _levelCompletePopup.gameObject.SetActive(false);
        
        if (_levelFailedPopup != null)
            _levelFailedPopup.gameObject.SetActive(false);
        
        if (_settingsPopup != null)
            _settingsPopup.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Hiển thị popup khi hoàn thành level
    /// </summary>
    /// <param name="level">Level hiện tại</param>
    /// <param name="reward">Điểm thưởng</param>
    public void ShowLevelComplete(int level, int reward)
    {
        if (_levelCompletePopup != null)
        {
            _levelCompletePopup.Initialize(level, reward);
            _levelCompletePopup.Show();
        }
    }
    
    public void ShowLevelFailed(int level)
    {
        if (_levelFailedPopup != null)
        {
            _levelFailedPopup.Initialize(level);
            _levelFailedPopup.Show();
        }
    }
    
    public void ShowSettingsPopup()
    {
        if (_settingsPopup != null)
        {
            _settingsPopup.Show();
        }
    }
}
