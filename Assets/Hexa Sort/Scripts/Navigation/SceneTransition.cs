using UnityEngine;

/// <summary>
/// Class tiện ích để quản lý việc chuyển scene với loading screen
/// </summary>
public static class SceneTransition
{
    // Tên các scene trong game
    public const string MAIN_MENU = "MainMenuScreen";
    public const string GAMEPLAY = "GameplayScreen";
    public const string LOADING = "LoadingScreen";
    
    /// <summary>
    /// Chuyển đến Main Menu với loading screen
    /// </summary>
    public static void ToMainMenu()
    {
        LoadingManager.LoadSceneWithLoading(MAIN_MENU);
    }
    
    /// <summary>
    /// Chuyển đến Gameplay với loading screen
    /// </summary>
    public static void ToGameplay()
    {
        LoadingManager.LoadSceneWithLoading(GAMEPLAY);
    }
    
    /// <summary>
    /// Chuyển đến scene bất kỳ với loading screen
    /// </summary>
    /// <param name="sceneName">Tên scene cần chuyển đến</param>
    public static void ToScene(string sceneName)
    {
        LoadingManager.LoadSceneWithLoading(sceneName);
    }
} 