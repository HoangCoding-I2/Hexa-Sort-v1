using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [Header("Settings")]
    [SerializeField] private int _baseScorePerLevel = 100;
    [SerializeField] private int _scorePerHexagon = 2;
    
    [Header("References")]
    [SerializeField] private ProgressBarUI _progressBar;
    
    private int _currentScore = 0;
    private int _targetScore = 0;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void OnEnable()
    {
        MergeManager.OnHexagonsVanished += AddScoreForHexagons;
        LevelManager.OnLevelLoaded += InitLevel;
    }
    
    private void OnDisable()
    {
        MergeManager.OnHexagonsVanished -= AddScoreForHexagons;
        LevelManager.OnLevelLoaded -= InitLevel;
    }
    
    public void InitLevel(int levelIndex)
    {
        _currentScore = 0;
        _targetScore = CalculateTargetScore(levelIndex);
        UpdateProgressBar();
    }
    
    private int CalculateTargetScore(int levelIndex)
    {
        return _baseScorePerLevel * (levelIndex + 1);
    }
    
    public void AddScoreForHexagons(int hexagonCount)
    {
        int scoreToAdd = hexagonCount * _scorePerHexagon;
        _currentScore += scoreToAdd;
        
        UpdateProgressBar();
        
        if (_currentScore >= _targetScore)
        {
            OnLevelComplete();
        }
    }
    
    private void UpdateProgressBar()
    {
        if (_progressBar != null)
        {
            float progress = (float)_currentScore / _targetScore;
            _progressBar.UpdateProgress(progress);
        }
    }
    
    private void OnLevelComplete()
    {
        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.ShowLevelComplete(
                LevelManager.Instance.GetCurrentLevelIndex() + 1,
                _currentScore
            );
        }
    }
    
    public int CurrentScore => _currentScore;
    
    public int TargetScore => _targetScore;
}
