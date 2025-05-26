using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelCompletePopup : BasePopup
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _rewardAmountText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _imageColor; // Reference to Image_color
    
    private int _rewardAmount;
    private int _rotationTweenId = -1; // ID for tracking the rotation tween
    
    protected override void Awake()
    {
        base.Awake();
        
        if (_nextButton != null)
            _nextButton.onClick.AddListener(OnNextButtonClicked);
    }
    
    public void Initialize(int level, int reward)
    {
        _rewardAmount = reward;
        
        if (_levelText != null)
            _levelText.text = $"Level {level}";
            
        if (_rewardAmountText != null)
            _rewardAmountText.text = reward.ToString();
    }
    
    public override void Show()
    {
        // First call the base Show implementation
        base.Show();
        
        // Phát âm thanh hoàn thành level
        if (AudioService.Instance != null)
            AudioService.Instance.PlayLevelComplete();
        
        // Prepare buttons for animation
        if (_nextButton != null)
        {
            _nextButton.transform.localScale = Vector3.zero;
            LeanTween.scale(_nextButton.gameObject, Vector3.one, 0.5f)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(0.2f);
        }
        
        // Start continuous rotation for Image_color
        if (_imageColor != null)
        {
            _imageColor.transform.rotation = Quaternion.identity;
            _rotationTweenId = LeanTween.rotateAround(_imageColor.gameObject, Vector3.forward, 360f, 4f)
                .setLoopClamp()
                .id;
        }
    }
    
    public override void Hide()
    {
        // Stop rotation animation
        if (_rotationTweenId != -1)
        {
            LeanTween.cancel(_rotationTweenId);
            _rotationTweenId = -1;
        }
        
        base.Hide();
    }
    
    private void OnNextButtonClicked()
    {
        // Phát âm thanh button click
        if (AudioService.Instance != null)
            AudioService.Instance.PlayButtonClick();
            
        Hide();
        LevelManager.Instance.LoadNextLevel();
    }
    
    private void OnDestroy()
    {
        // Stop animation if it's still running
        if (_rotationTweenId != -1)
        {
            LeanTween.cancel(_rotationTweenId);
            _rotationTweenId = -1;
        }
        
        if (_nextButton != null)
            _nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }
}
