# Work Time Calculator - Design System & Style Guide

## 📐 Tổng quan

Tài liệu này mô tả design system và style guide cho ứng dụng Work Time Calculator WPF. Tất cả các component, màu sắc, typography và spacing đều tuân theo các quy tắc được định nghĩa trong tài liệu này.

---

## 🎨 Color Palette

### Primary Colors
- **Primary Blue**: `#2196F3` - Màu chính cho CTA buttons, active states
- **Primary Dark**: `#1976D2` - Hover states
- **Primary Light**: `#BBDEFB` - Backgrounds, highlights

### Semantic Colors
- **Success Green**: `#4CAF50` - On track, completed tasks
- **Warning Orange**: `#FF9800` - Approaching limits
- **Error Red**: `#F44336` - Overtime, errors
- **Info Blue**: `#03A9F4` - Informational elements

### Neutral Palette
- **Background**: `#FAFAFA` - Main background
- **Surface**: `#FFFFFF` - Card backgrounds
- **Border**: `#E0E0E0` - Borders, dividers
- **Text Primary**: `#212121` - Main text
- **Text Secondary**: `#757575` - Secondary text
- **Text Disabled**: `#BDBDBD` - Disabled text

### Status Colors
- **On Track** (6-8h): `#4CAF50` (Green)
- **Overtime Warning** (8-10h): `#FF9800` (Orange)
- **Excessive** (>10h): `#F44336` (Red)
- **Under Target** (<6h): `#9E9E9E` (Gray)

---

## 📝 Typography

### Font Family
- **Primary**: Segoe UI (Windows)
- **Fallback**: Roboto

### Type Scale

| Style | Size | Weight | Usage |
|-------|------|--------|-------|
| Display | 32px | Bold | Page titles, large numbers |
| H1 | 24px | SemiBold | Section headers |
| H2 | 20px | SemiBold | Card titles |
| H3 | 18px | Medium | Subsections |
| Body Large | 16px | Regular | Emphasis text |
| Body | 14px | Regular | Default text |
| Caption | 12px | Regular | Labels, meta info |
| Overline | 10px | Medium | Small labels, tags |

---

## 📏 Spacing System

Base unit: **8px**

| Name | Size | Usage |
|------|------|-------|
| xs | 4px | Tight spacing |
| sm | 8px | Compact |
| md | 16px | Standard |
| lg | 24px | Comfortable |
| xl | 32px | Spacious |
| 2xl | 48px | Section breaks |

---

## 🔲 Border Radius

| Name | Size | Usage |
|------|------|-------|
| Small | 4px | Buttons, inputs |
| Medium | 8px | Cards, panels |
| Large | 16px | Major containers |
| Circle | 50% | Avatars, badges |

---

## 🌑 Shadows

| Name | Blur | Depth | Opacity | Usage |
|------|------|-------|---------|-------|
| Shadow-sm | 3px | 1px | 0.12 | Subtle elevation |
| Shadow-md | 6px | 4px | 0.1 | Cards (default) |
| Shadow-lg | 20px | 10px | 0.15 | Cards (hover) |
| Shadow-xl | 40px | 20px | 0.2 | Modals, dialogs |

---

## 🧩 Components

### Cards

#### Standard Card
```xml
<Border Style="{StaticResource CardStyle}">
    <!-- Content -->
</Border>
```

**Properties:**
- Background: `#FFFFFF`
- Border: `1px solid #E0E0E0`
- Border-radius: `8px`
- Padding: `24px`
- Shadow: `shadow-md`
- Hover: `shadow-lg`

#### Highlighted Card
```xml
<Border Style="{StaticResource HighlightedCardStyle}">
    <!-- Content -->
</Border>
```

**Properties:**
- Background: `#F5F9FF`
- Left border: `4px solid #2196F3`
- Other properties same as Standard Card

---

### Buttons

#### Primary Button
```xml
<Button Style="{StaticResource PrimaryButtonStyle}">
    Button Text
</Button>
```

**Properties:**
- Background: `#2196F3`
- Text: White
- Padding: `12px 24px`
- Border-radius: `4px`
- Font: `14px Medium`
- Hover: `#1976D2`
- Active: Scale `0.98x`

#### Secondary Button
```xml
<Button Style="{StaticResource AppOutlinedButtonStyle}">
    Button Text
</Button>
```

**Properties:**
- Background: Transparent
- Border: `1px solid #2196F3`
- Text: `#2196F3`

#### Text Button
```xml
<Button Style="{StaticResource TextButtonStyle}">
    Button Text
</Button>
```

**Properties:**
- Background: Transparent
- Text: `#2196F3`
- No border

#### Icon Button
```xml
<Button Style="{StaticResource IconButtonStyle}">
    <materialDesign:PackIcon Kind="IconName" />
</Button>
```

**Properties:**
- Size: `40x40px`
- Background: Transparent
- Hover: `#F5F5F5`

---

### Progress Bars

#### Horizontal Progress Bar
```xml
<ProgressBar Style="{StaticResource ModernProgressBar}"
             Value="{Binding Progress}"
             Maximum="100"
             Height="8" />
```

**Properties:**
- Height: `8px` (or `10px` for emphasis)
- Foreground: `#2196F3` (or semantic colors)
- Background: `#E0E0E0`
- Border-radius: `4px`

#### Circular Progress
```xml
<Ellipse Style="{StaticResource CircularProgressTrack}" />
<Ellipse Width="120" 
         Height="120" 
         Stroke="{StaticResource PrimaryBlueBrush}" 
         StrokeThickness="12"
         StrokeDashArray="{Binding Progress, Converter={StaticResource ProgressToDashArrayConverter}}" />
```

---

### Input Fields

#### Text Input
```xml
<TextBox Style="{StaticResource MaterialDesignTextBox}"
         materialDesign:HintAssist.Hint="Hint text"
         materialDesign:HintAssist.IsFloating="True" />
```

**Properties:**
- Font-size: `14px`
- Focus: Blue border `2px` + glow
- Error: Red border + shake animation

#### Time Picker
```xml
<materialDesign:TimePicker SelectedTime="{Binding StartTime}"
                           materialDesign:HintAssist.Hint="Select time" />
```

---

### Status Indicators

#### Status Dot
```xml
<Ellipse Width="12"
         Height="12"
         Fill="{Binding Hours, Converter={StaticResource HoursToStatusColorConverter}}" />
```

**Color Mapping:**
- Green: 6-8h (On target)
- Orange: 8-10h (Overtime warning)
- Red: >10h (Excessive)
- Gray: <6h (Under target)

---

## 🎭 Animations & Transitions

### Button Hover
- **Duration**: 200ms
- **Effect**: Scale up `1.02x` + shadow increase

### Button Active
- **Duration**: 200ms
- **Effect**: Scale down `0.98x`

### Card Hover
- **Duration**: 300ms
- **Effect**: Shadow-md → Shadow-lg

### Progress Bar Fill
- **Duration**: 500ms
- **Effect**: Animated fill on load

### Tab Switch
- **Duration**: 400ms
- **Effect**: Fade + slide

---

## 📱 Layout Guidelines

### Grid Layout - Today Tab

```
┌─────────────────────────────┬──────────────────────┐
│                             │                      │
│  LEFT COLUMN (65%)          │  RIGHT COLUMN (35%)  │
│                             │                      │
│  1. Time Calculator Card    │  1. Summary Card     │
│  2. Today's Log Card        │  2. Recent Days Card │
│                             │  3. Reminders Card   │
│                             │                      │
└─────────────────────────────┴──────────────────────┘
```

### Spacing Between Cards
- Standard: `16px` (margin-bottom)
- Section breaks: `24px` or `32px`

---

## 🎯 Do's and Don'ts

### ✅ Do's

1. **Use consistent spacing**: Always use multiples of 8px
2. **Maintain visual hierarchy**: Use appropriate text sizes and weights
3. **Provide feedback**: Show hover states and transitions
4. **Use semantic colors**: Apply status colors appropriately
5. **Keep it clean**: Use white space effectively
6. **Be accessible**: Maintain minimum 44x44px touch targets

### ❌ Don'ts

1. **Don't mix design systems**: Stick to this guide consistently
2. **Don't use arbitrary colors**: Use only defined palette
3. **Don't skip animations**: Provide smooth transitions
4. **Don't overcrowd**: Leave adequate spacing
5. **Don't ignore accessibility**: Ensure proper contrast ratios
6. **Don't use non-standard fonts**: Stick to Segoe UI

---

## 🔧 Usage Examples

### Creating a Card with Content

```xml
<Border Style="{StaticResource CardStyle}">
    <StackPanel>
        <!-- Card Header -->
        <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
            <materialDesign:PackIcon Kind="IconName" 
                                     Width="24" 
                                     Height="24"
                                     Foreground="{StaticResource PrimaryBlueBrush}"
                                     Margin="0,0,12,0"
                                     VerticalAlignment="Center" />
            <TextBlock Text="Card Title" 
                       Style="{StaticResource H2Text}" />
        </StackPanel>
        
        <!-- Card Content -->
        <TextBlock Text="Card content goes here" 
                   Style="{StaticResource BodyText}" />
    </StackPanel>
</Border>
```

### Creating a Status-Aware Component

```xml
<Border Background="{Binding Hours, Converter={StaticResource HoursToStatusColorConverter}}"
        CornerRadius="8"
        Padding="16">
    <TextBlock Text="{Binding Hours, StringFormat='{}{0:F1}h'}" 
               Style="{StaticResource BodyText}"
               Foreground="White" />
</Border>
```

---

## 📚 Resources

### Key Files
- `App.xaml` - All styles and resources
- `MainWindow.xaml` - Main UI implementation
- `Converters/ValueConverters.cs` - Value converters

### Static Resources
All styles are defined in `App.xaml` and can be referenced using:
```xml
Style="{StaticResource StyleName}"
```

---

## 🎨 Color Usage Guide

### When to Use Each Color

**Primary Blue (#2196F3)**
- CTA buttons
- Active tab indicators
- Links
- Important highlights

**Success Green (#4CAF50)**
- Completed tasks
- On-target status
- Positive feedback

**Warning Orange (#FF9800)**
- Approaching limits
- Attention-needed items
- Caution states

**Error Red (#F44336)**
- Errors
- Overtime warnings
- Critical alerts

**Text Colors**
- Primary (#212121): Main content
- Secondary (#757575): Labels, metadata
- Disabled (#BDBDBD): Inactive elements

---

## 📐 Component Specifications

### Card Specifications
- **Min Height**: None (content-driven)
- **Max Width**: Container width
- **Padding**: 24px all sides
- **Border**: 1px solid #E0E0E0
- **Corner Radius**: 8px

### Button Specifications
- **Min Width**: Auto (content + padding)
- **Min Height**: 40px (touch target)
- **Padding**: 12px 24px (vertical horizontal)
- **Border Radius**: 4px
- **Font**: 14px Medium Segoe UI

### Input Specifications
- **Height**: 48px (including label)
- **Border**: 1px solid #E0E0E0
- **Focus Border**: 2px solid #2196F3
- **Padding**: 12px horizontal
- **Font**: 14px Regular Segoe UI

---

## 🚀 Implementation Notes

1. **Responsive Design**: Components should adapt to different window sizes
2. **Dark Mode**: Colors defined for potential dark mode support
3. **Accessibility**: All interactive elements should have proper tooltips and ARIA labels
4. **Performance**: Use efficient converters and avoid unnecessary re-renders

---

**Version**: 1.0  
**Last Updated**: 2024  
**Maintained By**: Design Team

