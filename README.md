# Glanceable Taskbar Tiles

Prototype designs and rendering code for glanceable Windows 11 taskbar tiles with rich hover flyouts.

## Structure

```
TaskbarTiles/
├── AdaptiveCards/          ← Adaptive Card JSON payloads (content)
│   ├── CiCdTileFlyout.json
│   ├── SystemMonitorTileFlyout.json
│   └── adaptive-cards-preview.html   ← Browser preview (JS SDK)
├── HostConfig/             ← Fluent Design dark theme host config
│   └── hostconfig-fluent-dark.json
├── Rendering/              ← C# WinUI 3 renderer
│   └── TileFlyoutRenderer.cs
├── XAML/                   ← Native WinUI 3 XAML (alternative to AC)
│   ├── CiCdTileFlyout.xaml
│   └── SystemMonitorTileFlyout.xaml
└── HTML/                   ← Interactive HTML mockups
    ├── cicd-tile-mockup.html
    └── sysmon-tile-mockup.html
```

## How It Works

### Architecture

| Layer | File | Purpose |
|-------|------|---------|
| **Content** | `AdaptiveCards/*.json` | Card structure & data — no styling |
| **Theme** | `HostConfig/hostconfig-fluent-dark.json` | Fluent dark colors, spacing, fonts |
| **Renderer** | `Rendering/TileFlyoutRenderer.cs` | WinUI 3 rendering + action handling |

### Rendering an Adaptive Card in WinUI 3

```csharp
var renderer = new TileFlyoutRenderer();
var cardJson = File.ReadAllText("Assets/Cards/CiCdTileFlyout.json");
var element = renderer.RenderCard(cardJson);

// Add to your flyout
myFlyoutContent.Children.Add(element);
```

### NuGet Dependencies

```xml
<PackageReference Include="AdaptiveCards.Rendering.WinUI3" Version="1.0.0" />
<PackageReference Include="AdaptiveCards.ObjectModel.WinUI3" Version="1.0.0" />
```

## Design Approach

- **Adaptive Cards** for the flyout content — portable, data-driven, easy to update from a service
- **Host Config** controls all Fluent styling — cards stay content-only
- **WinUI 3 renderer** gives native `TextBlock`, `Button`, `ProgressRing` — Fluent for free
- **XAML alternatives** available if you need full custom control (animations, ProgressRing gauges)

## Previewing

1. **In VS Code**: Install the [Adaptive Cards extension](https://marketplace.visualstudio.com/items?itemName=madewithcardsio.adaptivecardsstudiobeta), open a `.json` file, and use "Preview"
2. **Online**: Paste JSON into [adaptivecards.io/designer](https://adaptivecards.io/designer/)
3. **Local HTML**: Open `AdaptiveCards/adaptive-cards-preview.html` in a browser

## Tile Scenarios

| Tile | Glance (taskbar) | Flyout (hover) |
|------|-------------------|----------------|
| CI/CD Pipeline | Build status dot + pass/fail count | Recent builds, test bar, deployments |
| System Monitor | CPU/RAM/GPU mini-bars + temp | Ring gauges, top processes, actions |
