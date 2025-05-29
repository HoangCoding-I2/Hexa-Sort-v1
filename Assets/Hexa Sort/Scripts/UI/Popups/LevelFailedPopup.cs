using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelFailedPopup : BasePopup
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _redoButton;
    [SerializeField] private Button _homeButton;
    
    protected override void Awake()
    {
        base.Awake();
        
        if (_redoButton != null)
            _redoButton.onClick.AddListener(OnRedoButtonClicked);
            
        if (_homeButton != null)
            _homeButton.onClick.AddListener(OnHomeButtonClicked);
    }
    
    public void Initialize(int level)
    {
        if (_levelText != null)
            _levelText.text = $"Level {level}";
    }
    
    private void OnRedoButtonClicked()
    {
        if (AudioService.Instance != null)
            AudioService.Instance.PlayButtonClick();
            
        Hide();
        LevelManager.Instance.RestartLevel();
    }
    
    private void OnHomeButtonClicked()
    {
        if (AudioService.Instance != null)
            AudioService.Instance.PlayButtonClick();
            
        Hide();
        
        // Sử dụng SceneTransition để chuyển scene
        SceneTransition.ToMainMenu();
    }
    
    public override void Show()
    {
        base.Show();
        
        if (AudioService.Instance != null)
            AudioService.Instance.PlayLevelFail();
        
        if (_redoButton != null)
        {
            _redoButton.transform.localScale = Vector3.zero;
            LeanTween.scale(_redoButton.gameObject, Vector3.one, 0.5f)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(0.2f);
        }
        
        if (_homeButton != null)
        {
            _homeButton.transform.localScale = Vector3.zero;
            LeanTween.scale(_homeButton.gameObject, Vector3.one, 0.5f)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(0.4f);
        }
    }
    
    private void OnDestroy()
    {
        if (_redoButton != null)
            _redoButton.onClick.RemoveListener(OnRedoButtonClicked);
            
        if (_homeButton != null)
            _homeButton.onClick.RemoveListener(OnHomeButtonClicked);
    }
}
