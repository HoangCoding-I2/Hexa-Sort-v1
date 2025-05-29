using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameplayScene : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private float _pulseScaleAmount = 1.1f;
    [SerializeField] private float _pulseDuration = 1.5f;
    [SerializeField] private float _delayBetweenSequences = 2f;
    
    private int pulseCount = 0;
    private Vector3 originalScale;
    private bool isTransitioning = false;

    void Start()
    {
        if (_playButton != null)
        {
            _playButton.onClick.AddListener(LoadGameplay);
            originalScale = _playButton.transform.localScale;
            StartPulseAnimation();
        }
    }

    void OnDestroy()
    {
        if (_playButton != null)
        {
            LeanTween.cancel(_playButton.gameObject);
            _playButton.onClick.RemoveListener(LoadGameplay);
        }
    }

    void StartPulseAnimation()
    {
        // Kiểm tra nếu đang chuyển scene hoặc button đã bị hủy
        if (isTransitioning || _playButton == null)
            return;

        // Tạo hiệu ứng phóng to
        Vector3 targetScale = originalScale * _pulseScaleAmount;
        
        LeanTween.scale(_playButton.gameObject, targetScale, _pulseDuration)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() => {
                // Kiểm tra lại trước khi tiếp tục
                if (isTransitioning || _playButton == null)
                    return;
                    
                // Tạo hiệu ứng thu nhỏ
                LeanTween.scale(_playButton.gameObject, originalScale, _pulseDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() => {
                        // Kiểm tra lại trước khi tiếp tục
                        if (isTransitioning || _playButton == null)
                            return;
                            
                        pulseCount++;
                        
                        // Nếu đã phóng to thu nhỏ đủ 2 lần
                        if (pulseCount >= 2)
                        {
                            // Reset counter và delay 2s trước khi lặp lại
                            pulseCount = 0;
                            
                            LeanTween.delayedCall(_delayBetweenSequences, () => {
                                // Kiểm tra lại trước khi tiếp tục
                                if (!isTransitioning && _playButton != null)
                                    StartPulseAnimation();
                            });
                        }
                        else
                        {
                            // Tiếp tục pulse lần tiếp theo
                            StartPulseAnimation();
                        }
                    });
            });
    }

    void LoadGameplay()
    {
        // Đánh dấu đang chuyển scene để tránh thêm animation mới
        isTransitioning = true;
        
        if (_playButton != null)
        {
            LeanTween.cancel(_playButton.gameObject);
        }
        
        if (AudioService.Instance != null)
        {
            AudioService.Instance.PlayButtonClick();
        }
        
        // Sử dụng SceneTransition để chuyển scene
        SceneTransition.ToGameplay();
    }
}
