# Work Time Calculator - Modern UI/UX Design

## 📖 Giới thiệu

Work Time Calculator là ứng dụng WPF hiện đại giúp bạn theo dõi và quản lý thời gian làm việc một cách hiệu quả. Ứng dụng đã được redesign hoàn toàn với focus vào UI/UX chuyên nghiệp, trực quan và dễ sử dụng.

## ✨ Tính năng chính

### 🕐 Time Calculator
- Tính toán thời gian kết thúc làm việc dựa trên thời gian bắt đầu
- Điều chỉnh thời gian làm việc (6-10 giờ)
- Quản lý thời gian nghỉ trưa (15m, 30m, 1h, 1.5h)
- Timeline visualization trực quan
- Progress tracking với time remaining

### 📝 Work Logging
- Ghi nhận các entry làm việc trong ngày
- Status indicators với color coding
- Xem tổng số giờ làm việc hôm nay
- Progress bar để theo dõi tiến độ

### 📊 Dashboard & Analytics
- **This Week**: Circular progress, weekly summary, overtime tracking
- **This Month**: Monthly hours, average per day, days worked
- **Recent Activity**: List các ngày gần đây với status indicators

### 📚 History
- Xem lịch sử làm việc
- Filters: Time period, Type, Search
- Status visualization
- Export to CSV

## 🎨 Design System

Ứng dụng sử dụng design system hiện đại với:

- **Color Palette**: Semantic colors cho status, consistent primary colors
- **Typography**: Clear hierarchy với Display, H1, H2, H3, Body styles
- **Spacing**: 8px-based spacing system
- **Components**: Reusable card, button, progress bar styles
- **Animations**: Smooth transitions và micro-interactions

Xem chi tiết trong [STYLE_GUIDE.md](STYLE_GUIDE.md)

## 🚀 Cài đặt và Chạy

### Yêu cầu
- .NET 8.0 SDK
- Visual Studio 2022 hoặc Visual Studio Code
- Windows (WPF application)

### Build và Run
```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

Hoặc mở `WorkTimeCalculator.sln` trong Visual Studio và nhấn F5.

## 📁 Cấu trúc Project

```
WorkTimeCalculator/
├── App.xaml                 # Application resources và styles
├── MainWindow.xaml          # Main UI
├── Models/                  # Data models
│   ├── WorkEntry.cs
│   └── LunchSettings.cs
├── ViewModels/              # MVVM ViewModels
│   ├── MainViewModel.cs
│   └── SettingsViewModel.cs
├── Views/                   # Views và dialogs
│   └── SettingsDialog.xaml
├── Services/                # Business logic
│   └── WorkTimeCalculatorService.cs
├── Converters/              # Value converters
│   └── ValueConverters.cs
├── STYLE_GUIDE.md          # Design system documentation
└── REDESIGN_SUMMARY.md     # Redesign summary
```

## 🎯 Cách sử dụng

### Tính thời gian kết thúc
1. Chọn thời gian bắt đầu (hoặc click "Now")
2. Điều chỉnh số giờ làm việc bằng slider
3. Chọn thời gian nghỉ trưa
4. Xem kết quả "Recommended End Time"
5. (Tùy chọn) Bật notification 15 phút trước

### Ghi nhận công việc
1. Click "Add" trong Today's Work Log
2. Nhập thông tin entry
3. Entry sẽ hiển thị với status indicator

### Xem lịch sử
1. Chuyển sang tab "History"
2. Sử dụng filters để lọc dữ liệu
3. Click "Export" để xuất CSV

## 🎨 UI Components

### Cards
- Standard cards với shadow và hover effects
- Highlighted cards với blue border
- Entry cards với status indicators

### Buttons
- Primary buttons (blue, filled)
- Secondary buttons (outlined)
- Text buttons
- Icon buttons

### Progress Indicators
- Horizontal progress bars
- Circular progress indicators
- Status dots với color coding

### Status Colors
- 🟢 Green (6-8h): On target
- 🟡 Orange (8-10h): Overtime warning  
- 🔴 Red (>10h): Excessive hours
- ⚪ Gray (<6h): Under target

## 🔧 Customization

### Thay đổi màu sắc
Tất cả màu sắc được định nghĩa trong `App.xaml`:

```xml
<SolidColorBrush x:Key="PrimaryBlueBrush" Color="#2196F3" />
<SolidColorBrush x:Key="SuccessGreenBrush" Color="#4CAF50" />
<!-- ... -->
```

### Thay đổi styles
Styles được định nghĩa trong `App.xaml` và có thể được override:

```xml
<Style TargetType="Button" x:Key="PrimaryButtonStyle">
    <!-- Customize here -->
</Style>
```

## 📚 Documentation

- [STYLE_GUIDE.md](STYLE_GUIDE.md) - Complete design system documentation
- [REDESIGN_SUMMARY.md](REDESIGN_SUMMARY.md) - Summary of redesign changes

## 🛠️ Technologies

- **.NET 8.0** - Framework
- **WPF** - UI Framework
- **MaterialDesignThemes** - UI Components
- **CommunityToolkit.Mvvm** - MVVM Pattern
- **C#** - Programming Language

## 📝 License

This project is provided as-is for educational and demonstration purposes.

## 👥 Contributors

Design và implementation bởi AI Assistant.

## 🎉 Acknowledgments

- Material Design for design inspiration
- MaterialDesignThemes team for excellent WPF components

---

**Version**: 2.0 (Redesigned)  
**Last Updated**: 2024

