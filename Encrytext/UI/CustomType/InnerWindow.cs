using Encrytext.UI.Menu;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.Resources;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.CustomType;

public abstract class InnerWindow : Window
{
    public InnerWindow()
    {
        Width = Dim.Fill();
        Height = Dim.Fill();
        BorderStyle = LineStyle.None;
    }
    
}
