using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Quản lý quá trình tải và chuyển scene với loading screen
/// Lớp này xử lý việc hiển thị màn hình loading và tải scene mới một cách bất đồng bộ
/// </summary>
public class LoadingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider _loadingSlider; // Thanh slider hiển thị tiến độ loading
    
    [Header("Loading Settings")]
    [SerializeField] private float _minLoadingTime = 1.5f; // Thời gian loading tối thiểu (giây)
    [SerializeField] private float _maxLoadingTime = 3f;   // Thời gian loading tối đa (giây)
    
    private string _targetScene; // Tên scene cần chuyển đến
    
    /// <summary>
    /// Được gọi khi scene loading được tải
    /// Lấy tên scene đích từ PlayerPrefs và bắt đầu quá trình loading
    /// </summary>
    private void Start()
    {
        // Lấy tên scene cần tải từ PlayerPrefs, nếu không có thì mặc định là "MainMenuScreen"
        _targetScene = PlayerPrefs.GetString("TargetScene", "MainMenuScreen");
        
        // Xóa key để tránh sử dụng lại trong tương lai
        PlayerPrefs.DeleteKey("TargetScene");
        
        // Bắt đầu coroutine loading
        StartCoroutine(LoadingProcess());
    }
    
    /// <summary>
    /// Coroutine xử lý quá trình loading scene mới
    /// Kết hợp giữa thời gian tối thiểu và tiến độ tải thực tế
    /// </summary>
    private IEnumerator LoadingProcess()
    {
        // Tạo thời gian loading ngẫu nhiên trong khoảng min-max để trải nghiệm người dùng nhất quán
        float loadingTime = Random.Range(_minLoadingTime, _maxLoadingTime);
        
        // Bắt đầu tải scene mới bất đồng bộ
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_targetScene);
        
        // Không chuyển scene ngay khi tải xong, mà đợi đến khi chúng ta cho phép
        asyncLoad.allowSceneActivation = false;
        
        float elapsedTime = 0f; // Thời gian đã trôi qua
        
        // Vòng lặp chạy cho đến khi đã đạt thời gian loading tối thiểu VÀ scene đã tải gần xong
        // Lưu ý: progress của AsyncOperation chỉ tải đến 0.9 (90%), 10% còn lại sẽ hoàn thành khi allowSceneActivation = true
        while (elapsedTime < loadingTime || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime; // Cập nhật thời gian đã trôi qua
            
            // Tính toán tiến độ dựa trên 2 yếu tố: thời gian và tiến độ tải thực tế
            float timeProgress = elapsedTime / loadingTime;      // Tiến độ dựa trên thời gian
            float sceneProgress = asyncLoad.progress / 0.9f;     // Tiến độ tải thực tế (chia cho 0.9 để scale lên 100%)
            float totalProgress = Mathf.Min(timeProgress, sceneProgress); // Lấy giá trị nhỏ hơn để đảm bảo không vượt quá tiến độ thực
            
            // Cập nhật giá trị slider nếu slider tồn tại
            if (_loadingSlider != null)
                _loadingSlider.value = totalProgress;
            
            yield return null; // Đợi đến frame tiếp theo
        }
        
        // Đảm bảo slider hiển thị 100% khi hoàn thành
        if (_loadingSlider != null)
            _loadingSlider.value = 1f;
            
        // Thêm một khoảng dừng ngắn để người dùng thấy thanh loading đã đầy
        yield return new WaitForSeconds(0.2f);
        
        // Cho phép kích hoạt scene mới
        asyncLoad.allowSceneActivation = true;
        
        // Đợi cho đến khi scene mới được tải hoàn toàn
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        // Hủy GameObject chứa LoadingManager vì không cần thiết nữa
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Phương thức static để chuyển đến scene khác thông qua loading screen
    /// Lưu tên scene vào PlayerPrefs và tải loading screen
    /// </summary>
    /// <param name="sceneName">Tên scene cần chuyển đến</param>
    public static void LoadSceneWithLoading(string sceneName)
    {
        // Kiểm tra tên scene hợp lệ
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }
        
        // Lưu tên scene đích vào PlayerPrefs để LoadingManager có thể truy cập
        PlayerPrefs.SetString("TargetScene", sceneName);
        PlayerPrefs.Save(); // Đảm bảo dữ liệu được lưu ngay lập tức
        
        // Tải loading screen
        SceneManager.LoadScene("LoadingScreen");
    }
} 