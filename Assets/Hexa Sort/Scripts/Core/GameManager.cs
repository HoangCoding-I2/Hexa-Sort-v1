using UnityEngine;

/// <summary>
/// Quản lý toàn bộ luồng game chính
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Kiểm tra nếu game over (các ô đều đầy)
    /// </summary>
    public bool IsGameOver()
    {
        GridCell[] allCells = FindObjectsOfType<GridCell>();
        
        // Kiểm tra nếu có ô trống
        bool hasEmptyCell = false;
        foreach (GridCell cell in allCells)
        {
            if (!cell.IsOccupied)
            {
                hasEmptyCell = true;
                break;
            }
        }
        
        // Nếu còn ô trống, chưa thua
        if (hasEmptyCell)
            return false;
        
        // Kiểm tra khả năng hợp nhất
        foreach (GridCell cell in allCells)
        {
            if (!cell.IsOccupied) continue;
            
            Color topColor = cell.Stack.GetTopHexagonColor();
            
            // Lấy các ô kề
            Collider[] neighborColliders = Physics.OverlapSphere(
                cell.transform.position, 2f, LayerMask.GetMask("GridCell"));
            
            foreach (Collider neighborCollider in neighborColliders)
            {
                GridCell neighbor = neighborCollider.GetComponent<GridCell>();
                if (neighbor != cell && neighbor.IsOccupied)
                {
                    if (neighbor.Stack.GetTopHexagonColor() == topColor)
                    {
                        // Còn có thể hợp nhất, chưa thua
                        return false;
                    }
                }
            }
        }
        
        // Không còn cách nào để chơi tiếp
        return true;
    }
    
    /// <summary>
    /// Gọi khi hoàn thành level
    /// </summary>
    public void OnLevelComplete()
    {
        int currentLevel = LevelManager.Instance.GetCurrentLevelIndex() + 1;
        int currentScore = ScoreManager.Instance.GetCurrentScore();
        
        // Sử dụng PopupManager trực tiếp
        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.ShowLevelComplete(currentLevel, currentScore);
        }
    }
    
    /// <summary>
    /// Gọi để kiểm tra game over sau mỗi lượt di chuyển
    /// </summary>
    public void CheckGameOver()
    {
        if (IsGameOver())
        {
            int currentLevel = LevelManager.Instance.GetCurrentLevelIndex() + 1;
            
            // Sử dụng PopupManager trực tiếp
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.ShowLevelFailed(currentLevel);
            }
            else
            {
                Debug.LogError("PopupManager.Instance là null khi cố gắng hiển thị LevelFailed popup");
            }
        }
    }
}
