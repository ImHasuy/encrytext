using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using Encrytext.UI.CustomType;
using Microsoft.DotNet.PlatformAbstractions;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.Drivers;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Menu;

public class AddOverlay : Runnable
{
    
  
    private CheckBox? _force16ColorsMenuItemCb;
    private OptionSelector? _themesSelector;
    private OptionSelector? _topSchemesSelector;
    private OptionSelector? _logLevelSelector;
    private FlagSelector<ViewDiagnosticFlags>? _diagnosticFlagsSelector;
    private CheckBox? _disableMouseCb;
    public static string? CachedRunnableScheme { get; set; }
    private View _contentArea;
    
    public AddOverlay()
    {
        SchemeName = CachedRunnableScheme = SchemeManager.SchemesToSchemeName (Schemes.Base);
        ConfigurationManager.Applied += ConfigAppliedHandler;
    }

    public MenuBar CreateMenuBar ()
    {
          
        BorderStyle = LineStyle.None;
        Width = Dim.Fill();
        Height = Dim.Fill();

        MenuBar menuBar = new([
            new MenuBarItem("_File", [
                    new MenuItem
                    {
                        Title = "Exit",
                        HelpText = "Quit Encrytext",
                        Key = Application.QuitKey,
                        Command = Command.Quit
                    }
                ]
            ),
            new MenuBarItem("_Themes", CreateThemeMenu()),
            new MenuBarItem("_Help", [
                new MenuItem
                {
                    Title = "_README",
                    HelpText = "README of Encrytext",
                    Action = () => OpenUrl("https://github.com/ImHasuy/encrytext/blob/main/README.md"),
                    Key = Key.F1
                },
                new MenuItem(
                    "_About...",
                    "About Encrytext",
                    () => MessageBox.Query(null, 
                        "", 
                        GetAboutBoxMessage(),
                        "Ok"
                    ))
            ])
        ]);

        return menuBar;
    }
   


    public static string GetAboutBoxMessage()
    {
        StringBuilder msg = new();
        msg.AppendLine("Encrytext is a simple and secure messaging app.");
        msg.AppendLine();
        msg.AppendLine("""
                       $$$$$$$$\                                           $$\                           $$\     
                       $$  _____|                                          $$ |                          $$ |    
                       $$ |      $$$$$$$\   $$$$$$$\  $$$$$$\  $$\   $$\ $$$$$$\    $$$$$$\  $$\   $$\ $$$$$$\   
                       $$$$$\    $$  __$$\ $$  _____|$$  __$$\ $$ |  $$ |\_$$  _|  $$  __$$\ \$$\ $$  |\_$$  _|  
                       $$  __|   $$ |  $$ |$$ /      $$ |  \__|$$ |  $$ |  $$ |    $$$$$$$$ | \$$$$  /   $$ |    
                       $$ |      $$ |  $$ |$$ |      $$ |      $$ |  $$ |  $$ |$$\ $$   ____| $$  $$<    $$ |$$\ 
                       $$$$$$$$\ $$ |  $$ |\$$$$$$$\ $$ |      \$$$$$$$ |  \$$$$  |\$$$$$$$\ $$  /\$$\   \$$$$  |
                       \________|\__|  \__| \_______|\__|       \____$$ |   \____/  \_______|\__/  \__|   \____/ 
                                                               $$\   $$ |                                        
                                                               \$$$$$$  |                                        
                                                                \______/                                         
                       """);
        msg.AppendLine();
        msg.AppendLine("Encrytext is open source and free to use.");
        msg.AppendLine();
        msg.AppendLine("https://github.com/ImHasuy/encrytext");

        return msg.ToString();
    }

    public static void OpenUrl(string url)
    {
        if (PlatformDetection.IsWindows())
        {
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        else if (PlatformDetection.IsMac())
        {
            Process.Start("open", url);
        }
        else if (PlatformDetection.IsLinux())
        {
            using Process process = new();

            process.StartInfo = new ProcessStartInfo
            {
                FileName = "xdg-open",
                Arguments = url,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            process.Start();
        }
    }
    
    
    View [] CreateThemeMenu ()
        {
            List<View> menuItems = [];

            _force16ColorsMenuItemCb = new ()
            {
                Title = "Force _16 Colors",
                Value = Driver.Force16Colors ? CheckState.Checked : CheckState.UnChecked,

                // Best practice for CheckBoxes in menus is to disable focus and highlight states
                CanFocus = false,
                MouseHighlightStates = MouseState.None
            };

            _force16ColorsMenuItemCb.ValueChanging += (_, args) =>
                                                      {
                                                          if (Driver.Force16Colors && args.NewValue == CheckState.UnChecked && !App!.Driver!.SupportsTrueColor)
                                                          {
                                                              args.Handled = true;
                                                          }
                                                      };

            _force16ColorsMenuItemCb.ValueChanged += (_, args) =>
                                                     {
                                                         Driver.Force16Colors = args.NewValue == CheckState.Checked;

                                                         _force16ColorsShortcutCb!.Value = args.NewValue;
                                                         SetNeedsDraw ();
                                                     };

            menuItems.Add (new MenuItem { CommandView = _force16ColorsMenuItemCb });

            menuItems.Add (new Line ());

            if (ConfigurationManager.IsEnabled)
            {
                _themesSelector = new OptionSelector
                {
                    // MouseHighlightStates = MouseState.In,
                    CanFocus = false

                    // InvertFocusAttribute = true
                };

                _themesSelector.ValueChanged += (_, args) =>
                                                {
                                                    if (args.NewValue is null)
                                                    {
                                                        return;
                                                    }
                                                    
                                                    ThemeManager.Theme = ThemeManager.GetThemeNames () [(int)args.NewValue];
                                                };

                var menuItem = new MenuItem { CommandView = _themesSelector, HelpText = "Cycle Through Themes", Key = Key.T.WithCtrl };
                menuItems.Add (menuItem);
                

                menuItems.Add (new Line ());

                _topSchemesSelector = new OptionSelector
                {
                    CanFocus = false
                };

                _topSchemesSelector.ValueChanged += (_, args) =>
                                                    {
                                                        if (args.NewValue is null)
                                                        {
                                                            return;
                                                        }
                                                        CachedRunnableScheme = SchemeManager.GetSchemesForCurrentTheme ().Keys.ToArray () [(int)args.NewValue];
                                                        SchemeName = CachedRunnableScheme;
                                                        SetNeedsDraw ();
                                                    };

                menuItem = new MenuItem
                {
                    Title = "Scheme for Runnable",
                    SubMenu = new Terminal.Gui.Views.Menu ([new MenuItem { CommandView = _topSchemesSelector, HelpText = "Cycle Through schemes", Key = Key.S.WithCtrl }])
                };
                menuItems.Add (menuItem);

                UpdateThemesMenu ();
            }
            else
            {
                menuItems.Add (new MenuItem { Title = "Configuration Manager is not Enabled", Enabled = false });
            }

            return menuItems.ToArray ();
        }
    
    
    #region StatusBar

    [ConfigurationProperty (Scope = typeof (AppSettingsScope), OmitClassName = true)]
    [JsonPropertyName ("UICatalog.StatusBar")]
    public static bool ShowStatusBar
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }
            field = value;
            StatusBarChanged?.Invoke (null, new ValueChangedEventArgs<bool> (!field, field));
        }
    } = true;

    public void SetStatusBar(StatusBar statusBar)
    {
        
    }
    
    /// <summary>Raised when "UICatalog.StatusBar" changes.</summary>
    public static event EventHandler<ValueChangedEventArgs<bool>>? StatusBarChanged;    private StatusBar? _statusBar;
    

    private Shortcut? _shQuit;
    private Shortcut? _shVersion;
    private CheckBox? _force16ColorsShortcutCb;
    
    
    
    public StatusBar CreateStatusBar ()
    {
        StatusBar statusBar = new () { Visible = ShowStatusBar, AlignmentModes = AlignmentModes.IgnoreFirstOrLast, CanFocus = false };

        // ReSharper disable All
        statusBar.Height = Dim.Auto (DimAutoStyle.Auto,
                                     minimumContentDim: Dim.Func (_ => statusBar.Visible ? 1 : 0),
                                     maximumContentDim: Dim.Func (_ => statusBar.Visible ? 1 : 0));

        // ReSharper restore All

        _shQuit = new Shortcut { CanFocus = false, Title = "Quit", Key = Application.QuitKey };

        _shVersion = new Shortcut { Title = $"OS - {RuntimeEnvironment.OperatingSystem} {RuntimeEnvironment.OperatingSystemVersion}" , CanFocus = false };

        Shortcut statusBarShortcut = new () { Key = Key.F10, Title = "Show/Hide Status Bar", CanFocus = false };

        statusBarShortcut.Accepting += (_, args) =>
                                       {
                                           statusBar.Visible = !_statusBar.Visible;
                                           args.Handled = true;
                                       };

        _force16ColorsShortcutCb = new CheckBox
        {
            Title = "16 color mode", Value = Driver.Force16Colors ? CheckState.Checked : CheckState.UnChecked, CanFocus = true
        };

        Shortcut force16ColorsShortcut = new ()
        {
            CanFocus = false,
            CommandView = _force16ColorsShortcutCb,
            HelpText = "",
            BindKeyToApplication = true,
            Key = Key.F7
        };

        force16ColorsShortcut.Accepting += (_, args) =>
                                           {
                                               Driver.Force16Colors = !Driver.Force16Colors;
                                               _force16ColorsMenuItemCb!.Value = Driver.Force16Colors ? CheckState.Checked : CheckState.UnChecked;
                                               SetNeedsDraw ();
                                               args.Handled = true;
                                           };
        statusBar.Add (_shQuit, statusBarShortcut, force16ColorsShortcut, _shVersion);
        
        StatusBarChanged += (_, args) =>
        {
            switch (args.NewValue)
            {
                case true:
                    _statusBar!.Height = Dim.Auto ();

                    break;

                case false:
                    _statusBar!.Height = 0;

                    break;
            }
        };

        

        return statusBar;
    }

    #endregion StatusBar
    
    
    #region Configuration Manager

    /// <summary>
    ///     Called when CM has applied changes.
    /// </summary>
    private void ConfigApplied ()
    {
        UpdateThemesMenu ();

        SchemeName = CachedRunnableScheme;
        
        SetNeedsLayout();
        SetNeedsDraw();

        _shQuit?.Key = Application.QuitKey;

        _statusBar!.Visible = ShowStatusBar;
        _force16ColorsShortcutCb!.Value = Driver.Force16Colors ? CheckState.Checked : CheckState.UnChecked;

        App.TopRunnableView?.SetNeedsDraw ();
    }

    private void ConfigAppliedHandler (object? sender, ConfigurationManagerEventArgs? a) => ConfigApplied ();

    #endregion Configuration Manager
    
    private void UpdateThemesMenu ()
    {
        if (_themesSelector is null)
        {
            return;
        }

        
        _themesSelector.Value = null;
        _themesSelector.AssignHotKeys = true;
        _themesSelector.UsedHotKeys.Clear ();
        _themesSelector.Labels = ThemeManager.GetThemeNames ().ToArray ();
        _themesSelector.Value = ThemeManager.GetThemeNames ().IndexOf (ThemeManager.GetCurrentThemeName ());

        if (_topSchemesSelector is null)
        {
            return;
        }

        _topSchemesSelector.AssignHotKeys = true;
        _topSchemesSelector.UsedHotKeys.Clear ();
        int? selectedScheme = _topSchemesSelector.Value;
        _topSchemesSelector.Labels = SchemeManager.GetSchemeNames ().ToArray ();
        _topSchemesSelector.Value = selectedScheme;

        if (CachedRunnableScheme is null || !SchemeManager.GetSchemeNames ().Contains (CachedRunnableScheme))
        {
            CachedRunnableScheme = SchemeManager.SchemesToSchemeName (Schemes.Base);
        }

        int newSelectedItem = SchemeManager.GetSchemeNames ().IndexOf (CachedRunnableScheme!);

        // if the item is in bounds then select it
        if (newSelectedItem >= 0 && newSelectedItem < SchemeManager.GetSchemeNames ().Count)
        {
            _topSchemesSelector.Value = newSelectedItem;
        }
    }

    
}
