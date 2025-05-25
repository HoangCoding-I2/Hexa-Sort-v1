# Hướng dẫn thiết lập hệ thống âm thanh Hexa Sort (Đã tái cấu trúc)

## Tổng quan
Hệ thống âm thanh được thiết kế để dễ sử dụng, đơn giản và dễ mở rộng. Hệ thống sẽ tự động quản lý việc chuyển đổi nhạc nền giữa các màn hình và cung cấp các phương pháp đơn giản để phát các âm thanh hiệu ứng.

## Thiết lập trong Unity

### Bước 1: Thiết lập Audio Mixer
1. Mở Unity và tạo một Audio Mixer mới (nếu chưa có):
   - Menu: Window > Audio > Audio Mixer
   - Nhấn chuột phải vào panel và chọn "Create Mixer"
   - Đặt tên là "HexaSortMixer"

2. Tạo các nhóm âm thanh trong Audio Mixer:
   - Group "Music" (tạo mới)
   - Group "SFX" (tạo mới)

3. Cấu hình Exposed Parameters cho mỗi nhóm:
   - Nhấn chuột phải vào Volume của mỗi nhóm
   - Chọn "Expose Parameter" 
   - Đảm bảo tên tham số là:
     - "MusicVolume" cho nhóm Music
     - "SFXVolume" cho nhóm SFX

### Bước 2: Thiết lập Sound Library
1. Tạo Sound Library Scriptable Object:
   - Nhấn chuột phải trong Project window
   - Chọn Create > Hexa Sort > Audio > Sound Library
   - Đặt tên là "SoundLibrary"

2. Thiết lập các Sound Scriptable Object:
   - Tạo các Sound Scriptable Object: Create > Hexa Sort > Audio > Sound
   - Đặt tên và thiết lập như sau:
     - "BGM_MainMenu" - loại Music, loop = true
     - "BGM_GameLoop" - loại Music, loop = true
     - "SFX_HexagonPlace" - loại SFX
     - "SFX_HexagonMerge" - loại SFX
     - "SFX_HexagonVanish" - loại SFX
     - "SFX_StackShow" - loại SFX
     - "UI_ButtonClick" - loại SFX
     - "SFX_LevelComplete" - loại SFX
     - "SFX_LevelFail" - loại SFX

3. Thiết lập Sound Library:
   - Mở SoundLibrary trong Inspector
   - Gán từng Sound vào các trường tương ứng

### Bước 3: Thiết lập AudioService
1. Tạo một GameObject trong scene MainMenuScreen:
   - Đặt tên là "AudioService"
   - Thêm component AudioService vào GameObject

2. Thiết lập tham chiếu cho AudioService:
   - Kéo Audio Mixer vào trường "_audioMixer"
   - Kéo các Audio Mixer Group vào các trường:
     - "_musicMixerGroup" = Group "Music"
     - "_sfxMixerGroup" = Group "SFX"
   - Kéo Sound Library vào trường "_soundLibrary"
   - Đảm bảo "_autoPlayMusicOnSceneLoad" = true

### Bước 4: Thiết lập SettingsPopup
1. Mở prefab SettingsPopup trong UI/Popups
2. Đảm bảo có 2 slider chính trong Popup:
   - Music Volume slider: Gán vào trường "_musicSlider"
   - SFX Volume slider: Gán vào trường "_sfxSlider"

3. Cài đặt các slider:
   - Min Value = 0
   - Max Value = 1
   - Chế độ Whole Numbers = False

## Cách hoạt động của hệ thống
Hệ thống âm thanh sử dụng Singleton Pattern nên chỉ cần một instance của AudioService trong toàn bộ game. AudioService sẽ tự động chuyển nhạc nền khi chuyển scene, và cung cấp các phương thức để phát các âm thanh SFX.

### Đặc điểm nổi bật:
1. **Tự động phát nhạc theo scene**: Hệ thống sẽ tự động phát nhạc phù hợp khi chuyển scene
2. **Crossfade khi chuyển nhạc**: Nhạc cũ sẽ fade out và nhạc mới sẽ fade in mượt mà
3. **Quản lý âm lượng riêng biệt**: Có thể điều chỉnh âm lượng Music và SFX riêng biệt
4. **Hỗ trợ âm thanh vị trí 3D**: Có thể phát âm thanh tại vị trí 3D cụ thể
5. **Tự động dọn dẹp**: Các âm thanh ngắn sẽ tự động dọn dẹp, âm thanh lặp được quản lý hiệu quả

## Sử dụng AudioService trong code
```csharp
// Phát âm thanh theo loại có sẵn
AudioService.Instance.PlayButtonClick();
AudioService.Instance.PlayHexagonPlace();
AudioService.Instance.PlayHexagonMerge();
AudioService.Instance.PlayHexagonVanish();
AudioService.Instance.PlayStackShow();
AudioService.Instance.PlayLevelComplete();
AudioService.Instance.PlayLevelFail();

// Phát âm thanh từ thư viện bằng tên
AudioService.Instance.PlaySoundFromLibrary("UI_ButtonClick");

// Đặt âm lượng
AudioService.Instance.SetMusicVolume(0.7f); // 0 (tắt tiếng) đến 1 (âm lượng tối đa)
AudioService.Instance.SetSFXVolume(0.8f);   // 0 (tắt tiếng) đến 1 (âm lượng tối đa)

// Lấy âm lượng hiện tại
float musicVolume = AudioService.Instance.GetMusicVolume();
float sfxVolume = AudioService.Instance.GetSFXVolume();
```

## Mở rộng hệ thống
Để thêm một âm thanh mới:
1. Tạo một Sound Scriptable Object mới
2. Thêm trường mới trong SoundLibrarySO
3. Cập nhật phương thức GetSoundFromLibrary trong AudioService
4. Tạo phương thức phát âm thanh mới trong AudioService (ví dụ: PlayNewSound)

Để thêm một scene mới với nhạc nền riêng:
1. Thêm mapping mới vào Dictionary _musicByScene trong phương thức MapBackgroundMusic trong AudioService

## Xử lý sự cố
1. **Không nghe thấy âm thanh**:
   - Kiểm tra Audio Mixer - đảm bảo các nhóm không bị tắt tiếng
   - Kiểm tra SoundSO - đảm bảo clip và volume được đặt đúng
   - Đảm bảo AudioService GameObject đã được tạo và cấu hình đúng

2. **Nhiều instance của AudioService**:
   - AudioService sử dụng DontDestroyOnLoad nên sẽ tồn tại qua các scene
   - Nếu thấy nhiều instance, hãy đảm bảo chỉ có một AudioService trong hierarchy

3. **Nhạc nền không chuyển đổi khi đổi scene**:
   - Kiểm tra _autoPlayMusicOnSceneLoad đã được bật
   - Kiểm tra _musicByScene Dictionary đã có mapping đúng cho scene
   - Đảm bảo tên scene chính xác 