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
    }
    
    void LoadMainMenu()
    {
        if (AudioService.Instance != null)
        {
            AudioService.Instance.PlayButtonClick();
        }
        
        Debug.Log("Returning to main menu...");
        
        // Sử dụng SceneTransition để chuyển scene
        SceneTransition.ToMainMenu();
    }
    
    private void OnDestroy()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(LoadMainMenu);
        }
    }
} 