using Encrytext.Core.Entity;
using Encrytext.Networking.interfaces;
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
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Auto()
        };
        
        chatContent.SetSource(AppState.CurrentUser!.CurrentMessageProfile!.MessageHistory);
        
       
            AppState.CurrentUser.CurrentMessageProfile.MessageHistory.CollectionChanged += (s, e) =>
            {
                App?.Invoke(() =>
                {
                    chatContent.SetNeedsLayout();
                });
            };
        
            chatWindow.Add(chatContent);
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

        var userList = new ListView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        
        userList.SetSource(AppState.CurrentUser!.Contacts);
        
        availableUsers.Add(userList);
        
        
        #endregion

        
        
        #region Input Window
        
        Window inputWindow = new()
        {
            Title = "Input",
            X = 0,
            Y =Pos.Bottom(chatWindow),
            Width = Dim.Fill() - availableUsers.Width -1,
            Height = Dim.Absolute(4),
            HasFocus = true
        };

        var inputField = new TextField()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            HasFocus = true
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

                    var sentMesages = new MessageHistory
                    {
                        Message = message,
                        Sendername = AppState.CurrentUser.Name,
                        TimeStamp = DateTime.Now
                    };
                    
                    AppState.CurrentUser.CurrentMessageProfile.MessageHistory.Add(sentMesages);
                }
                
                inputField.Text = "";
                e.Handled = true;
            }
        };
        inputWindow.Add(inputField);

        base.BeginInit();
        #endregion
        
        
        
        
        Add(Menubar,chatWindow, availableUsers,inputWindow, Statusbar);
    }
}