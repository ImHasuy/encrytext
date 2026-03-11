using Encrytext.UI.CustomType;
using Encrytext.UI.Menu;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Screens;

public class ChatWindow :InnerWindow
{
    public override void BeginInit()
    {
        var Overlay = new AddOverlay();
        var Menubar = Overlay.CreateMenuBar();
        var Statusbar = new StatusBar();

        var label = new Label { Title = "Chat", X = Pos.Center(), Y = Pos.Bottom(Menubar) + 2 };
        
        Add(Menubar,label, Statusbar);
    }
}