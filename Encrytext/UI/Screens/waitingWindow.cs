using Encrytext.UI.CustomType;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Screens;

public class waitingWindow : InnerWindow
{

    private Timer _systemTimer;
    private uint _systemTimerTick = 100; // ms

    public override void BeginInit()
    {
        var welcome = new Label { Text = $"Wainting for your partner to connect!", X = Pos.Center(), Y = Pos.Center() +1 };
        
        
        var spinner = new SpinnerView
        {
            Style = new SpinnerStyle.BouncingBall(), Visible = true, X = Pos.Center(), Y = Pos.Bottom(welcome)+1
        };

        
        
        _systemTimer?.Dispose ();
        _systemTimer = null;

        _systemTimer = new Timer(_ =>
            {
                App?.Invoke(_ => spinner.AdvanceAnimation());
                
                if ( AppState.CurrentUser?.UserChosenMessageProfile?.PartnerGuid == AppState.CurrentUser?.CurrentMessageProfile?.PartnerGuid)
                {
                    App.RequestStop();
                }
                
                
            }, null, 0, _systemTimerTick
        );

        
        Add(welcome, spinner);
        
    }
    
}