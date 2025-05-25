using System.Collections;
using UnityEngine;

public class BasePopup : MonoBehaviour
{
    protected CanvasGroup _canvasGroup;
    
    [Header("UI Elements")]
    [SerializeField] protected RectTransform _panelPopup; // Kéo Panel_Popup vào đây từ Hierarchy
    
    [Header("Animation Settings")]
    [SerializeField] protected float _showDuration = 0.3f;
    [SerializeField] protected float _hideDuration = 0.25f;
    [SerializeField] protected LeanTweenType _showEase = LeanTweenType.easeOutBack;
    [SerializeField] protected LeanTweenType _hideEase = LeanTweenType.easeInBack;
    
    private bool _isShowing = false;
    public bool IsShowing => _isShowing;
    private int _tweenId = -1;
    
    protected virtual void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
            
        // Kiểm tra xem _panelPopup đã được gán chưa
        if (_panelPopup == null)
            Debug.LogError($"Panel Popup chưa được gán cho {gameObject.name}. Vui lòng kéo Panel_Popup vào Inspector!");
    }
    
    public virtual void Show()
    {
        if (_tweenId != -1)
        {
            LeanTween.cancel(_tweenId);
            _tweenId = -1;
        }
        
        gameObject.SetActive(true);
        _isShowing = true;
        
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
        if (_panelPopup != null)
        {
            _panelPopup.localScale = Vector3.zero;
            _tweenId = LeanTween.scale(_panelPopup, Vector3.one, _showDuration)
                .setEase(_showEase)
                .setOnComplete(() => {
                    _tweenId = -1;
                })
                .id;
        }
    }
    
    public virtual void Hide()
    {
        if (_tweenId != -1)
        {
            LeanTween.cancel(_tweenId);
            _tweenId = -1;
        }
        
        _isShowing = false;
        
        if (_panelPopup != null)
        {
            _tweenId = LeanTween.scale(_panelPopup, Vector3.zero, _hideDuration)
                .setEase(_hideEase)
                .setOnComplete(() => {
                    CompleteHide();
                    _tweenId = -1;
                })
                .id;
        }
        else
        {
            CompleteHide();
        }
    }
    
    private void CompleteHide()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        
        gameObject.SetActive(false);
    }
    
    public virtual void OnCloseButtonClicked()
    {
        // Phát âm thanh khi nhấn nút đóng
        if (AudioService.Instance != null)
            AudioService.Instance.PlayButtonClick();
            
        Hide();
    }
}
