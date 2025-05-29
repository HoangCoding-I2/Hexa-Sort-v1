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
            
        if (_musicSlider != null)
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    private void LoadSettings()
    {
        if (AudioService.Instance == null)
            return;
            
        if (_musicSlider != null)
            _musicSlider.value = AudioService.Instance.GetMusicVolume();
            
        if (_sfxSlider != null)
            _sfxSlider.value = AudioService.Instance.GetSFXVolume();
    }
    
    private void OnMusicVolumeChanged(float volume)
    {
        if (AudioService.Instance != null)
            AudioService.Instance.SetMusicVolume(volume);
    }
    
    private void OnSFXVolumeChanged(float volume)
    {
        if (AudioService.Instance != null)
            AudioService.Instance.SetSFXVolume(volume);
    }
    
    public override void Show()
    {
        base.Show();
        LoadSettings();
    }
    
    public override void OnCloseButtonClicked()
    {
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
