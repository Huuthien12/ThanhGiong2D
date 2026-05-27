<img width="754" height="391" alt="image" src="https://github.com/user-attachments/assets/44b45a49-78fd-439b-be3a-f0c0dcd08b94" />⚔️ Thánh Gióng – The Heavenly King of Phu Dong

📖 Mô tả ngắn

> “Vươn vai thành tráng sĩ, cưỡi ngựa sắt, quật ngã quân thù!”

Thánh Gióng là game nhập vai hành động (Action RPG) 2D, được xây dựng dựa trên truyền thuyết “Thánh Gióng” – một trong Tứ bất tử của văn hóa dân gian Việt Nam.  
Người chơi sẽ đồng hành cùng cậu bé làng Phù Đổng, từ những ngày đầu ăn cơm dân làng lớn nhanh như thổi, tìm kiếm vũ khí thần thánh (giáp sắt, giáo bạc, ngựa sắt, tre đằng ngà), cho đến khi ra trận dẹp tan giặc Ân và bay về trời. Game mang đậm màu sắc văn hóa dân tộc, kết hợp lối chơi platformer, chiến đấu đối kháng và khám phá cốt truyện.

---

🎮 Screenshot / GIF gameplay

| Menu chính | Màn chơi làng Gióng | Chiến đấu với giặc Ân |
|:----------:|:-------------------:|:---------------------:|
| ![Menu]| 
<img width="754" height="391" alt="image" src="https://github.com/user-attachments/assets/715c6541-da14-4e2e-b442-62239f24a797" />
|![Map1]|
<img width="754" height="391" alt="image" src="https://github.com/user-attachments/assets/5920cb5b-078d-44bb-8372-d617e7d9a199" />
| ![Combat]|
<img width="754" height="389" alt="image" src="https://github.com/user-attachments/assets/95ed8f24-d40e-49aa-82db-b3cd6b8b94c8" />

| Boss Rắn hổ mang | Cưỡi ngựa sắt | Kết thúc – bay về trời |
|:---------------:|:-------------:|:----------------------:|
| ![Boss]| 
<img width="754" height="389" alt="image" src="https://github.com/user-attachments/assets/91e0e919-b0c0-4e16-9e54-b1802fe45269" />

| ![Horse]| 
<img width="754" height="390" alt="image" src="https://github.com/user-attachments/assets/211f69da-06fb-4662-87f8-cbfe89d8270f" />

| ![Ending]|
<img width="754" height="388" alt="image" src="https://github.com/user-attachments/assets/16a6e580-3bcd-4fbe-9bb4-71e20acae4af" />

---

🛠️ Hướng dẫn cài đặt và chạy

Yêu cầu hệ thống (xem chi tiết ở mục dưới)
- Hệ điều hành: Windows 10/11 (64-bit), macOS, hoặc Linux (Ubuntu 20.04+)
- .NET Runtime (tự động cài cùng Unity build)
- Đồ họa hỗ trợ DirectX 11 / OpenGL 4.5

Cách chạy bản build (không cần Unity)
1. Tải game:  
   - Tải file `.zip` từ nhánh `Build` trên GitHub (hoặc theo link do nhóm cung cấp).
2. Giải nén vào thư mục bất kỳ (nên dùng đường dẫn không dấu, ví dụ: `D:\Game\ThanhGiong`).
3. Chạy file `ThanhGiong.exe` (trên Windows) hoặc `ThanhGiong.x86_64` (Linux/macOS).
4. Điều khiển:
   - `A/D` hoặc `←/→` : Di chuyển trái/phải
   - `Space` : Nhảy
   - `J` : Tấn công thường
   - `K` : Kỹ năng đặc biệt (nếu có)
   - `W` : Tương tác (leo thang, dịch chuyển, nhặt đồ)

 Chạy từ source code (Unity)
1. Cài đặt Unity Hub và Unity Editor 2022.3.x LTS.
2. Clone repository:
   ```bash
   git clone https://github.com/Huuthien12/ThanhGiong2D

---
STT	Họ tên	MSSV	Công việc phụ trách	Tỷ lệ đóng góp
1	Nguyễn Hồng Phúc Thọ	2312758	Video introl, âm thanh(hành động, nhạc nền, nút bấm), thiết kế map, viết báo cáo	20%
2	Lương Hữu Thiện	2312753	Trưởng nhóm (Team Lead). Dựng scene, thiết kế level, thiết kế UI, hệ thống menu, spawn enemy, 
hệ thống va chạm, xử lý sát thương, hệ thống combo, quản lý source code (Git), tích hợp các hệ thống, xử lý boss,
chuyển scene, lưu dữ liệu game, viết báo cáo chính	40%
3	Hoàng Bình Quân	2314236	Enemy AI; tạo vật phẩm, quái, boss(animation di chuyển, tấn công, chết của quái, boss), viết báo cáo	20%
4	Nguyễn Minh Long	2314361	Xử lý Player, lập trình Player Controller, animation nhân vật, hỗ trợ debug, viết báo cáo	20%
Tổng				100%
---
Công nghệ	sử dụng
Unity	2022.3.x LTS (hoặc 6)	Game engine chính
C#	.NET Standard 2.1	Ngôn ngữ lập trình logic game
Visual Studio 2022	Community	IDE viết code, debug
GitHub Desktop / Git	Latest	Quản lý source code, cộng tác nhóm
---  
