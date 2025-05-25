using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMainMenuScene : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    
    void Start()
    {
        if (_backButton != null)
        {
            _backButton.onClick.AddListener(LoadMainMenu);
        }
        else
        {
            Debug.LogWarning("Back button is not assigned in the Inspector!");
        }
    }
    
    void LoadMainMenu()
    {
        // Phát âm thanh button click
        if (AudioService.Instance != null)
        {
            AudioService.Instance.PlayButtonClick();
        }
        
        Debug.Log("Returning to main menu...");
        SceneManager.LoadScene("MainMenuScreen");
        
        // Không cần gọi phát nhạc nền vì AudioService sẽ tự động phát khi scene được load
    }
    
    private void OnDestroy()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(LoadMainMenu);
        }
    }
} 