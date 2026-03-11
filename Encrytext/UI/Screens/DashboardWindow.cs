using System.Collections;
using System.Collections.Immutable;
using Encrytext.Networking.Protocol.interfaces;
using Encrytext.Networking.Protocol.Services;
using Encrytext.UI.CustomType;
using Encrytext.UI.Menu;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Screens;

public class DashboardWindow : InnerWindow // Vagy Runnable<bool>
{
    private Timer _systemTimer;
    private uint _systemTimerTick = 100; // ms
    private ListView _partnerList;
    
    
    public override void BeginInit()
    {
        var Overlay = new AddOverlay();
        var Menubar = Overlay.CreateMenuBar();
        var Statusbar = new StatusBar();
        
        
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
        
        var label = new Label { Text = $"Welcome {AppState.CurrentUser!.Name}!", X = Pos.Center(), Y = Pos.Bottom(logo) +1 };
        
        
        var spinner = new SpinnerView
        {
            Style = new SpinnerStyle.BouncingBall(), Visible = true, X = Pos.Center(), Y = Pos.Top(Statusbar) -2
        };
        
        
        DiscoveryService discoveryService = new ();
        discoveryService.Start();
      
        
        _systemTimer?.Dispose ();
        _systemTimer = null;

        _systemTimer = new Timer(_ =>
            {
                App?.Invoke(_ => spinner.AdvanceAnimation());
            }, null, 0, _systemTimerTick
        );
         /*
          *To stop run
          *  _systemTimer?.Dispose ();
          * _systemTimer = null;
          *
          */
         
         _partnerList = new ListView
         {
             X = Pos.Center(),
             Y = Pos.Bottom(label) + 2,
             Width = Dim.Fill(),
             Height = Dim.Fill() - Dim.Height(Statusbar) 
         };
         
         _partnerList.SetSource(AppState.CurrentUser.Contacts);


         if (AppState.CurrentUser.Contacts != null)
         {
             AppState.CurrentUser.Contacts.CollectionChanged += (s, e) =>
             {
                 App?.Invoke(() =>
                 {
                     _partnerList.SetNeedsLayout();
                 });
             };
         }
      
        var btnLogout = new Button { Text = "Logout", X = Pos.Center(), Y = Pos.Bottom(label) + 3 };
        
        btnLogout.Accepting += (s, e) => {
            RequestStop();
            e.Handled = true;
        };  
        
        Add(Menubar,logo, label, spinner, btnLogout, _partnerList ,Statusbar);
    }
    
 
    
}