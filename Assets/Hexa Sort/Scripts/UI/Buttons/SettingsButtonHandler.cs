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
        else
        {
            Debug.LogWarning("Settings button is not assigned in the Inspector!");
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
        else
        {
            Debug.LogError("PopupManager instance not found!");
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