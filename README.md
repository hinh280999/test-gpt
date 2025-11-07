# Work Time Calculator

Modern WPF application for planning and tracking your workday. The redesign introduces a modular MVVM architecture, Material Design styling, and analytics-driven dashboards.

## Prerequisites

- .NET 8 SDK

## Getting Started

```bash
cd WorkTimeCalculator
dotnet build
```

### Running the app

```
dotnet run
```

### Running tests

```
dotnet test
```

## Project Structure

- `WorkTimeCalculator/` – WPF application with MVVM view models, services, and Material Design UI.
- `WorkTimeCalculator.Tests/` – xUnit test project covering core services and view models.
- `Resources/` – shared color, typography, and component dictionaries for consistent styling.

## Features

- **Today tab** – Quick calculator with custom time picker, break planner, and rich daily summary.
- **History tab** – Interactive calendar, filters, and bulk operations with inline analytics.
- **Analytics tab** – Productivity insights, peak hours, and category distribution charts.
- **Settings tab** – Work schedule configuration, notifications, and data management.

## Data & Services

The app uses an EF Core in-memory database (with seeding) and service abstractions for calculations, exports, and notifications. Replace `WorkTimeDbContext` configuration in `App.xaml.cs` to connect a persistent data store.
