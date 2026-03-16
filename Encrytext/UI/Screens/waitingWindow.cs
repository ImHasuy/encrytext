using Encrytext.Networking.Services;
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
        var tcpConnection = new TcpConnectService();
        _ = Task.Run(async () => await tcpConnection.TcpConnectServiceAsync());
        
        var welcome = new Label { Text = $"Waiting for your partner to connect!", X = Pos.Center(), Y = Pos.Center() +1 };
        
        
        var spinner = new SpinnerView
        {
            Style = new SpinnerStyle.BouncingBall(), Visible = true, X = Pos.Center(), Y = Pos.Bottom(welcome)+1
        };

        
        
        _systemTimer?.Dispose ();
        _systemTimer = null;
        bool stopRequested = false;

        _systemTimer = new Timer(_ =>
            {
                App?.Invoke(_ => spinner.AdvanceAnimation());
                
                var chosenGuid = AppState.CurrentUser!.UserChosenMessageProfile!.PartnerGuid;
                var currentMsProfileGuid = AppState.CurrentUser?.CurrentMessageProfile?.PartnerGuid;

                if (chosenGuid != null && currentMsProfileGuid != null && !stopRequested && chosenGuid.Equals(currentMsProfileGuid))
                {
                    stopRequested = true;
                    App?.Invoke(_ => App.RequestStop());
                }
                
            }, null, 0, _systemTimerTick
        );
        
        Add(welcome, spinner);
    }
    
}