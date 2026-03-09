using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace Encrytext.UI.Screens;


public sealed class SinginWindow : Runnable<string?>
{
    public SinginWindow ()
    {
        
        Title = $"Example App ({Application.QuitKey} to quit)";
        
        var usernameLabel = new Label { Text = "Username:", X = Pos.Center () -17, Y = Pos.Center()};

        var userNameText = new TextField
        {
            X = Pos.Right (usernameLabel) + 1,
            Y = Pos.Center (),
            
            Width = Dim.Percent(12)
        };
        
        
        var btnLogin = new Button
        {
            Text = "Login",
            Y = Pos.Bottom (usernameLabel) + 1,
            X = Pos.Center (),
            IsDefault = true
        };
        
        btnLogin.Accepting += (s, e) =>
        {
            if (string.IsNullOrEmpty(userNameText.Text))
            {
                MessageBox.ErrorQuery (App!, "Logging In", "A username has to be chosen", "Ok");
                                      
            }
            else
            {
                Result = userNameText.Text;
                App!.RequestStop ();
            }
            
            e.Handled = true;
        };
        
        Add (usernameLabel, userNameText, btnLogin);
    }
}
