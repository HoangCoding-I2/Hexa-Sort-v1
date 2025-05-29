using UnityEngine;

/// <summary>
/// Class tiện ích để quản lý việc chuyển scene với loading screen
/// Cung cấp các phương thức đơn giản và dễ sử dụng để chuyển đến các màn hình chính trong game
/// Lớp này được thiết kế là static để có thể được gọi từ bất kỳ đâu trong code mà không cần tham chiếu
/// </summary>
public static class SceneTransition
{
    // Tên các scene trong game
    // Các hằng số này giúp tránh lỗi chính tả khi viết tên scene và dễ dàng thay đổi nếu cần
    public const string MAIN_MENU = "MainMenuScreen";  // Scene màn hình chính
    public const string GAMEPLAY = "GameplayScreen";   // Scene gameplay
    public const string LOADING = "LoadingScreen";     // Scene loading
    
    /// <summary>
    /// Chuyển đến Main Menu với loading screen
    /// Phương thức này gọi LoadingManager để tải scene MainMenu thông qua loading screen
    /// </summary>
    public static void ToMainMenu()
    {
        // Sử dụng LoadingManager để tải scene MainMenu với loading screen
        LoadingManager.LoadSceneWithLoading(MAIN_MENU);
    }
    
    /// <summary>
    /// Chuyển đến Gameplay với loading screen
    /// Phương thức này gọi LoadingManager để tải scene Gameplay thông qua loading screen
    /// </summary>
    public static void ToGameplay()
    {
        // Sử dụng LoadingManager để tải scene Gameplay với loading screen
        LoadingManager.LoadSceneWithLoading(GAMEPLAY);
    }
    
    /// <summary>
    /// Chuyển đến scene bất kỳ với loading screen
    /// Phương thức tổng quát để chuyển đến bất kỳ scene nào thông qua loading screen
    /// </summary>
    /// <param name="sceneName">Tên scene cần chuyển đến</param>
    public static void ToScene(string sceneName)
    {
        // Kiểm tra tên scene có giá trị null hoặc rỗng (kiểm tra được thực hiện trong LoadingManager)
        // Sử dụng LoadingManager để tải scene được chỉ định với loading screen
        LoadingManager.LoadSceneWithLoading(sceneName);
    }
} 