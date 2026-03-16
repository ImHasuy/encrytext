using System.Collections;
using System.Collections.Immutable;
using Encrytext.Networking.Services;
using Encrytext.UI.CustomType;
using Encrytext.UI.Menu;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Screens;

public class DashboardWindow : InnerWindow 
{
    private Timer _systemTimer;
    private uint _systemTimerTick = 100; // ms
    private ListView _partnerList;
    
    
    
    public override void BeginInit()
    {
        var Overlay = new AddOverlay();
        var Menubar = Overlay.CreateMenuBar();
        var Statusbar = Overlay.CreateStatusBar();
        
        
        Title = "Dashboard";
        
        var logo = new Label
        {
            Text = ("""
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
                    """),
            X = Pos.Center(),
            Y = Pos.Top(Menubar) + 2
        };
        
        var welcome = new Label { Text = $"Welcome {AppState.CurrentUser!.Name}!", X = Pos.Center(), Y = Pos.Bottom(logo) +1 };
        
        var suggestionText = new Label { Text = $"Please select a user below to chat with!", X = Pos.Center(), Y = Pos.Bottom(welcome) +1 };
        
        var spinner = new SpinnerView
        {
            Style = new SpinnerStyle.BouncingBall(), Visible = true, X = Pos.Center(), Y = Pos.Top(Statusbar) -2
        };
        
        
        DiscoveryService discoveryService = new ();
        discoveryService.Start();
        UdpListenerService udpListenerService = new ();
        udpListenerService.Start();
        TcpListenerService tcpListenerService = new ();
        _ = Task.Run(async () => await tcpListenerService.TcpListenerServiceAsync());
      
        
        _systemTimer?.Dispose ();
        _systemTimer = null;

        _systemTimer = new Timer(_ =>
            {
                App?.Invoke(_ => spinner.AdvanceAnimation());
            }, null, 0, _systemTimerTick
        );
    
         
         _partnerList = new ListView
         {
             X = Pos.Center(),
             Y = Pos.Bottom(suggestionText) + 1,
             Width = Dim.Auto(),
             Height = Dim.Fill(4),
         };
         
         _partnerList.SetSource(AppState.CurrentUser.Contacts);
         
      
         if (AppState.CurrentUser.Contacts.Count == 0)
         {
             AppState.CurrentUser.Contacts.CollectionChanged += (s, e) =>
             {
                 App?.Invoke(() =>
                 {
                     _partnerList.SetNeedsLayout();
                 });
             };
         }




         _partnerList.ValueChanged += (s, e) =>
         {
             var index = _partnerList.SelectedItem!.Value;
             var chosenPartner = AppState.CurrentUser.Contacts[index];
             AppState.CurrentUser.UserChosenMessageProfile = chosenPartner;
             discoveryService.Stop();
             udpListenerService.Stop();
             App!.RequestStop();
         };
         
         
         
         
         
        Add(Menubar,logo, welcome, suggestionText, spinner, _partnerList ,Statusbar);
    }
    
 
    
}