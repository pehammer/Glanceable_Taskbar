using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AdaptiveCards.Rendering.WinUI3;
using AdaptiveCards.ObjectModel.WinUI3;
using System;
using System.IO;
using Windows.Data.Json;

namespace TaskbarTiles.Rendering;

/// <summary>
/// Renders Adaptive Card JSON payloads in a WinUI 3 flyout surface
/// using a Fluent Design dark host config.
/// 
/// NuGet dependencies:
///   - AdaptiveCards.Rendering.WinUI3
///   - AdaptiveCards.ObjectModel.WinUI3
/// </summary>
public sealed class TileFlyoutRenderer
{
    private readonly AdaptiveCardRenderer _renderer;

    public TileFlyoutRenderer()
    {
        _renderer = new AdaptiveCardRenderer();
        ApplyFluentDarkHostConfig();
    }

    /// <summary>
    /// Renders an Adaptive Card JSON string into a UIElement for display in a flyout.
    /// </summary>
    public UIElement RenderCard(string cardJson)
    {
        var parseResult = AdaptiveCard.FromJsonString(cardJson);

        if (parseResult.AdaptiveCard is null)
        {
            return new TextBlock
            {
                Text = "Failed to parse Adaptive Card",
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Red)
            };
        }

        var renderResult = _renderer.RenderAdaptiveCard(parseResult.AdaptiveCard);

        if (renderResult.FrameworkElement is null)
        {
            return new TextBlock
            {
                Text = "Failed to render Adaptive Card",
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Red)
            };
        }

        // Wire up action handling
        renderResult.Action += OnAction;

        return renderResult.FrameworkElement;
    }

    /// <summary>
    /// Renders a card from a file path.
    /// </summary>
    public UIElement RenderCardFromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return RenderCard(json);
    }

    /// <summary>
    /// Applies the Fluent Design dark theme host config.
    /// </summary>
    private void ApplyFluentDarkHostConfig()
    {
        var hostConfigJson = LoadHostConfigJson();

        if (!string.IsNullOrEmpty(hostConfigJson))
        {
            var hostConfig = AdaptiveHostConfig.FromJsonString(hostConfigJson);
            if (hostConfig.HostConfig is not null)
            {
                _renderer.HostConfig = hostConfig.HostConfig;
            }
        }
    }

    /// <summary>
    /// Loads the host config JSON from the app package.
    /// Falls back to an inline minimal config if file not found.
    /// </summary>
    private static string LoadHostConfigJson()
    {
        try
        {
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Assets", "HostConfig", "hostconfig-fluent-dark.json");
            return File.ReadAllText(path);
        }
        catch
        {
            return """
            {
              "fontFamily": "Segoe UI Variable, Segoe UI",
              "containerStyles": {
                "default": {
                  "backgroundColor": "#2C2C2C",
                  "foregroundColors": {
                    "default": { "default": "#FFFFFF", "subtle": "#C8FFFFFF" },
                    "accent": { "default": "#60CDFF" },
                    "good": { "default": "#6CCB5F" },
                    "attention": { "default": "#FF99A4" }
                  }
                }
              },
              "spacing": { "small": 4, "default": 8, "medium": 12, "large": 16, "padding": 16 }
            }
            """;
        }
    }

    /// <summary>
    /// Handles actions from the rendered card.
    /// </summary>
    private void OnAction(RenderedAdaptiveCard sender, AdaptiveActionEventArgs args)
    {
        switch (args.Action)
        {
            case AdaptiveOpenUrlAction openUrl:
                _ = Windows.System.Launcher.LaunchUriAsync(openUrl.Url);
                break;

            case AdaptiveSubmitAction submit:
                HandleSubmitAction(submit.DataJson);
                break;
        }
    }

    /// <summary>
    /// Processes submit action data from card buttons.
    /// </summary>
    private static void HandleSubmitAction(JsonObject data)
    {
        if (data is null) return;

        var action = data.GetNamedString("action", "");

        switch (action)
        {
            case "deploy":
                var env = data.GetNamedString("environment", "staging");
                var version = data.GetNamedString("version", "latest");
                // TODO: Trigger deployment pipeline
                System.Diagnostics.Debug.WriteLine($"Deploy {version} to {env}");
                break;

            case "rerunFailed":
                var buildId = data.GetNamedString("buildId", "");
                // TODO: Re-run failed build via CI API
                System.Diagnostics.Debug.WriteLine($"Re-run build {buildId}");
                break;

            case "openDetails":
                // TODO: Open detailed system monitor view
                System.Diagnostics.Debug.WriteLine("Open details");
                break;
        }
    }
}
