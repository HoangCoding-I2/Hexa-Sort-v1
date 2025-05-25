using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : BasePopup
{
    [Header("UI Elements")]
    [SerializeField] private Button _okButton;
    
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;
    
    protected override void Awake()
    {
        base.Awake();
        
        if (_okButton != null)
            _okButton.onClick.AddListener(OnCloseButtonClicked);
            
        // Thiết lập sự kiện khi slider thay đổi
        if (_musicSlider != null)
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private void LoadSettings()
    {
        if (AudioService.Instance == null)
            return;
            
        // Lấy và hiển thị giá trị âm lượng hiện tại
        if (_musicSlider != null)
            _musicSlider.value = AudioService.Instance.GetMusicVolume();
            
        if (_sfxSlider != null)
            _sfxSlider.value = AudioService.Instance.GetSFXVolume();
    }
    
    // Các callback khi slider thay đổi
    private void OnMusicVolumeChanged(float volume)
    {
        if (AudioService.Instance != null)
            AudioService.Instance.SetMusicVolume(volume);
    }
    
    private void OnSFXVolumeChanged(float volume)
    {
        if (AudioService.Instance != null)
            AudioService.Instance.SetSFXVolume(volume);
            
        // Không phát âm thanh UI_ButtonClick khi điều chỉnh âm thanh nữa
        // Vì có thể làm rối loạn người dùng khi có quá nhiều âm thanh
    }
    
    public override void Show()
    {
        base.Show();
        LoadSettings();
    }
    
    public override void OnCloseButtonClicked()
    {
        // Phát âm thanh button click
        if (AudioService.Instance != null)
            AudioService.Instance.PlayButtonClick();
            
        base.OnCloseButtonClicked();
    }
    
    private void OnDestroy()
    {
        if (_okButton != null)
            _okButton.onClick.RemoveListener(OnCloseButtonClicked);
            
        if (_musicSlider != null)
            _musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            
        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }
}
