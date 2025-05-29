using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Xử lý sự kiện khi nhấn nút Settings
/// </summary>
public class SettingsButtonHandler : MonoBehaviour
{
    [SerializeField] private Button settingsButton;
    
    void Start()
    {
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }
    }
    
    private void OnSettingsButtonClicked()
    {
        // Phát âm thanh button click
        if (AudioService.Instance != null)
        {
            AudioService.Instance.PlayButtonClick();
        }
        
        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.ShowSettingsPopup();
        }
    }
    
    private void OnDestroy()
    {
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        }
    }
} 