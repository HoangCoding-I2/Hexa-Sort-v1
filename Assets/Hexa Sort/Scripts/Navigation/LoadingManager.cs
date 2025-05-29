using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider _loadingSlider;
    
    [Header("Loading Settings")]
    [SerializeField] private float _minLoadingTime = 1.5f;
    [SerializeField] private float _maxLoadingTime = 3f;
    
    private string _targetScene;
    
    private void Start()
    {
        _targetScene = PlayerPrefs.GetString("TargetScene", "MainMenuScreen");
        PlayerPrefs.DeleteKey("TargetScene");
        StartCoroutine(LoadingProcess());
    }
    
    private IEnumerator LoadingProcess()
    {
        float loadingTime = Random.Range(_minLoadingTime, _maxLoadingTime);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_targetScene);
        asyncLoad.allowSceneActivation = false;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < loadingTime || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;
            
            float timeProgress = elapsedTime / loadingTime;
            float sceneProgress = asyncLoad.progress / 0.9f;
            float totalProgress = Mathf.Min(timeProgress, sceneProgress);
            
            if (_loadingSlider != null)
                _loadingSlider.value = totalProgress;
            
            yield return null;
        }
        
        if (_loadingSlider != null)
            _loadingSlider.value = 1f;
            
        yield return new WaitForSeconds(0.2f);
        
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        Destroy(gameObject);
    }
    
    public static void LoadSceneWithLoading(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is null or empty!");
            return;
        }
        
        PlayerPrefs.SetString("TargetScene", sceneName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoadingScreen");
    }
} 