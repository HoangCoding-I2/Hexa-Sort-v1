using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [Header("Settings")]
    [SerializeField] private int _baseScorePerLevel = 100; // Điểm cơ bản mỗi level
    [SerializeField] private int _scorePerHexagon = 2; // Điểm mỗi hexagon
    
    [Header("References")]
    [SerializeField] private ProgressBarUI _progressBar;
    
    private int _currentScore = 0;
    private int _targetScore = 0;
    
    public static Action<int> OnScoreChanged;
    public static Action OnLevelCompleted;
    
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
        OnLevelCompleted += OnLevelComplete;
        LevelManager.OnLevelLoaded += InitLevel;
    }
    
    private void OnDisable()
    {
        MergeManager.OnHexagonsVanished -= AddScoreForHexagons;
        OnLevelCompleted -= OnLevelComplete;
        LevelManager.OnLevelLoaded -= InitLevel;
    }
    
    /// <summary>
    /// Khởi tạo level với điểm số mục tiêu
    /// </summary>
    /// <param name="levelIndex">Chỉ số level</param>
    public void InitLevel(int levelIndex)
    {
        _currentScore = 0;
        _targetScore = CalculateTargetScore(levelIndex);
        UpdateProgressBar();
    }
    
    /// <summary>
    /// Tính toán điểm mục tiêu dựa vào cấp độ
    /// </summary>
    /// <param name="levelIndex">Chỉ số level</param>
    /// <returns>Điểm mục tiêu</returns>
    private int CalculateTargetScore(int levelIndex)
    {
        return _baseScorePerLevel * (levelIndex + 1);
    }
    
    /// <summary>
    /// Thêm điểm cho mỗi hexagon đã hợp nhất
    /// </summary>
    /// <param name="hexagonCount">Số lượng hexagon</param>
    public void AddScoreForHexagons(int hexagonCount)
    {
        int scoreToAdd = hexagonCount * _scorePerHexagon;
        _currentScore += scoreToAdd;
        
        UpdateProgressBar();
        
        if (_currentScore >= _targetScore)
        {
            OnLevelCompleted?.Invoke();
        }
        
        OnScoreChanged?.Invoke(_currentScore);
    }
    
    /// <summary>
    /// Cập nhật thanh tiến trình
    /// </summary>
    private void UpdateProgressBar()
    {
        if (_progressBar != null)
        {
            float progress = (float)_currentScore / _targetScore;
            _progressBar.UpdateProgress(progress);
        }
    }
    
    /// <summary>
    /// Xử lý khi hoàn thành level
    /// </summary>
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
    
    /// <summary>
    /// Lấy điểm hiện tại
    /// </summary>
    /// <returns>Điểm hiện tại</returns>
    public int GetCurrentScore()
    {
        return _currentScore;
    }
    
    /// <summary>
    /// Lấy điểm mục tiêu
    /// </summary>
    /// <returns>Điểm mục tiêu</returns>
    public int GetTargetScore()
    {
        return _targetScore;
    }
}
