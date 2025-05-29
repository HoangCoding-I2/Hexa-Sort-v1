using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _progressBarFill;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _levelText;
    
    [Header("Settings")]
    [SerializeField] private bool _smoothFill = true;
    [SerializeField] private float _fillSpeed = 5f;
    
    private float _targetProgress = 0f;
    
    private void Start()
    {
        UpdateUI();
    }
    
    private void Update()
    {
        if (_smoothFill && _progressBarFill.fillAmount != _targetProgress)
        {
            _progressBarFill.fillAmount = Mathf.Lerp(_progressBarFill.fillAmount, _targetProgress, Time.deltaTime * _fillSpeed);
        }
    }
    

    
    public void UpdateProgress(float progress)
    {
        _targetProgress = Mathf.Clamp01(progress);
        
        if (!_smoothFill)
        {
            _progressBarFill.fillAmount = _targetProgress;
        }
        
        // Cập nhật UI text luôn khi progress thay đổi
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (ScoreManager.Instance != null)
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"{ScoreManager.Instance.CurrentScore} / {ScoreManager.Instance.TargetScore}";
            }
            
            if (_levelText != null)
            {
                _levelText.text = $"LEVEL {LevelManager.Instance.GetCurrentLevelIndex() + 1}";
            }
        }
    }
} 