using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public static Action<int> OnLevelLoaded;
    [SerializeField] private Transform levelContainer; // Container để đặt level
    [SerializeField] private GameObject[] levelPrefabs; // Mảng chứa tất cả prefab level
    
    private GameObject currentLevel; // Level đang hoạt động hiện tại
    private int currentLevelIndex = 0; // Chỉ số level hiện tại

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        // Tải level đầu tiên khi khởi động
        LoadLevel(0);
    }
    
    // Tải một level cụ thể
    public void LoadLevel(int levelIndex)
    {
        // Hủy level hiện tại nếu có
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
            
        if (levelIndex >= 0 && levelIndex < levelPrefabs.Length)
        {
            currentLevel = Instantiate(levelPrefabs[levelIndex], levelContainer);
            currentLevelIndex = levelIndex;
            
            // Phát event khi level mới được tải
            OnLevelLoaded?.Invoke(currentLevelIndex);
        }
    }
    
    // Tải level tiếp theo
    public void LoadNextLevel()
    {
        LoadLevel(currentLevelIndex + 1);
    }
    
    // Tải lại level hiện tại
    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }
    
    // Lấy chỉ số level hiện tại
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
}