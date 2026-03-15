using Encrytext.Networking.Protocol.Services;
using Encrytext.UI.CustomType;
using Encrytext.UI.Menu;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Screens;

public class ChatWindow :InnerWindow
{
    public override void BeginInit()
    {

        var tcpConnection = new TcpConnectService();
        _ = Task.Run(async () => await tcpConnection.TcpConnectServiceAsync());
        
        var Overlay = new AddOverlay();
        var Menubar = Overlay.CreateMenuBar();
        var Statusbar = Overlay.CreateStatusBar();

        var messageSender = new MessageSender();
        
        #region Chat Window
        Window chatWindow = new()
        {
            Title = "Chat",
            X =0,
            Y =Pos.Bottom(Menubar) + 1,
            Width = Dim.Fill() - Dim.Percent(21),
            Height = Dim.Percent(90) - Statusbar.Height
        };

        var chatContent = new ListView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        chatContent.SetSource(AppState.CurrentUser.CurrentMessageProfile.MessageHistory);
        
        if (AppState.CurrentUser.CurrentMessageProfile.MessageHistory.Count == 0)
        {
            AppState.CurrentUser.CurrentMessageProfile.MessageHistory.CollectionChanged += (s, e) =>
            {
                App?.Invoke(() =>
                {
                    chatContent.SetNeedsLayout();
                });
            };
        }
        
        #endregion 


        #region Available Users

        

        Window availableUsers = new()
        {
            Title = "Users",
            X =Pos.Right(chatWindow),
            Y = Pos.Bottom(Menubar) + 1 ,
            Width = Dim.Percent(20),
            Height = Dim.Fill() - 1
        };
        
        #endregion

        
        
        #region Input Window
        
        Window inputWindow = new()
        {
            Title = "Input",
            X = 0,
            Y =Pos.Bottom(chatWindow),
            Width = Dim.Fill() - availableUsers.Width -1,
            Height = Dim.Absolute(4)
        };

        var inputField = new TextField()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        

        inputField.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Key.Enter)
            {
                var message = inputField.Text;
                if (!string.IsNullOrEmpty(message))
                {
                    _ = Task.Run(async () =>
                       await messageSender.SendMessageAsync(AppState.CurrentUser.CurrentMessageProfile.ActiveStream, message,
                            AppState.CurrentUser.CurrentMessageProfile));

                }
                
                inputField.Text = "";
                e.Handled = true;
            }
        };
        inputWindow.Add(inputField);

        #endregion
        
        
        
        
        Add(Menubar,chatWindow, availableUsers,inputWindow, Statusbar);
    }
}