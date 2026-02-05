using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI;

public class EncrytextRunnable: Runnable
{
    public static string? CachedRunnableScheme { get; set; }
    private static ViewDiagnosticFlags _diagnosticFlags;

    public EncrytextRunnable()
    {
        _diagnosticFlags = Diagnostics;
        SchemeName = CachedRunnableScheme = SchemeManager.SchemesToSchemeName(Schemes.Base);
        ConfigurationManager.Applied += ConfigAppliedHandler;
    }
    
    
    #region Configuration Manager

    private void ConfigApplied()
    {
        
    }
    
    #endregion Configuration Manager
    
}