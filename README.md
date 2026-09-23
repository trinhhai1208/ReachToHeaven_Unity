# Reach To Heaven

**Reach To Heaven** là game hành động sinh tồn 2D góc nhìn từ trên xuống, được phát triển bằng Unity. Người chơi di chuyển trong bản đồ, chiến đấu với các đợt quái vật, thu thập kinh nghiệm, nâng cấp chỉ số và cố gắng sống sót theo mục tiêu của từng chế độ chơi.

## Tính năng chính

- Chiến đấu thời gian thực với nhiều loại kẻ địch và đạn/hitbox khác nhau.
- Hệ thống wave điều khiển thời điểm xuất hiện, số lượng và độ khó của quái vật.
- Hai chế độ đã được cấu hình: **Survival** và **Endless**.
- Chọn map, chế độ chơi và level trước khi bắt đầu một lượt chơi.
- Tiến trình mở khóa level được lưu bằng `PlayerPrefs`.
- Thu thập gem/kinh nghiệm và lựa chọn nâng cấp chỉ số trong trận.
- Hệ thống vũ khí có thể chuyển đổi khi đang chơi.
- Giao diện HUD, thanh máu, thanh kinh nghiệm, thông báo wave, màn hình thắng/thua và bảng cài đặt.
- Âm thanh, hiệu ứng hình ảnh và camera sử dụng các hệ thống tích hợp của Unity.

## Điều khiển

| Thao tác | Bàn phím và chuột |
| --- | --- |
| Di chuyển | `WASD` hoặc phím mũi tên |
| Tấn công | Chuột trái hoặc `Enter` |
| Đổi vũ khí | `Left Shift` |
| Ngắm | Di chuyển chuột |

Dự án cũng có các binding cơ bản cho gamepad, joystick, cảm ứng và XR trong Input System.

## Công nghệ

- Unity `6000.3.20f1`
- Universal Render Pipeline `17.3.0`
- Unity Input System `1.19.0`
- Cinemachine `3.1.4`
- Unity UI (uGUI) và TextMesh Pro
- C#

## Cài đặt và chạy dự án

1. Clone repository:

   ```bash
   git clone https://github.com/trinhhai1208/ReachToHeaven_Unity.git
   ```

2. Mở Unity Hub và chọn **Add project from disk**.
3. Chọn thư mục vừa clone và mở bằng Unity `6000.3.20f1`.
4. Chờ Unity tải package và import toàn bộ asset.
5. Mở scene `Assets/Scenes/MainMenu.unity`, sau đó nhấn **Play**.

Package `com.coplaydev.unity-mcp` được tham chiếu trực tiếp từ GitHub, vì vậy lần mở dự án đầu tiên cần có kết nối Internet.

## Build game

1. Trong Unity, mở **File > Build Profiles**.
2. Chọn nền tảng mục tiêu và cài module tương ứng nếu Unity yêu cầu.
3. Kiểm tra các scene trong danh sách build.
4. Chọn **Build** hoặc **Build and Run**.

Các scene hiện được cấu hình trong Build Settings:

1. `MainMenu`
2. `GamePlay`
3. `LoadingScene`
4. `MapSelect`

## Cấu trúc thư mục

```text
Assets/
├── Project/
│   ├── Data/                 # ScriptableObject cấu hình map, mode, level, wave và enemy
│   ├── Prefabs/              # Player, enemy, UI, projectile và các prefab gameplay
│   └── Scripts/
│       ├── CharacterSystem/  # Điều khiển nhân vật, state machine và chỉ số
│       ├── GameFramework/    # Cấu hình lượt chơi, nội dung và tiến trình level
│       ├── SpawnerSystem/    # Sinh enemy, portal, gem, hitbox và projectile
│       ├── SystemScript/     # Quản lý scene, game state và các hệ thống dùng chung
│       └── UI/               # HUD, menu, cài đặt và giao diện nâng cấp
├── Scenes/                   # Các scene của game
Packages/                     # Khai báo package Unity
ProjectSettings/              # Cấu hình dự án Unity
```

## Kiến trúc nổi bật

- **State machine** quản lý các trạng thái idle, di chuyển, tấn công, nhận sát thương và chết của nhân vật.
- **ScriptableObject** lưu dữ liệu map, chế độ, level, wave, enemy và chỉ số người chơi.
- Hệ thống **spawner/product** tái sử dụng luồng tạo enemy, portal, gem, hitbox và projectile.
- Các catalog và `RunConfig` tách dữ liệu lựa chọn trước trận khỏi logic gameplay.

## Lưu ý

- Không commit các thư mục do Unity tự sinh như `Library`, `Temp`, `Logs`, `Build` và `UserSettings`.
- Khi thêm hoặc di chuyển asset trong Unity, cần commit cả file `.meta` tương ứng.
- Repository chưa khai báo giấy phép sử dụng mã nguồn; mọi quyền hiện được bảo lưu cho chủ sở hữu dự án.
