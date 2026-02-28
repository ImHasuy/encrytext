using Encrytext.Core.Entity;
using Encrytext.Core.Services;
using Terminal.Gui.App;
using Terminal.Gui.Configuration;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views; 
ConfigurationManager.Enable (ConfigLocations.All);

AppState.CurrentUser =  new User
{
    Name = "LocalUser",
    Guid = Guid.NewGuid(),
    IpAddress = AppState.LocalIpAddress.GetIpAddress()
};
    
IApplication app = Application.Create ().Init ();
var userName = app.Run<ExampleWindow> ().GetResult<string> ();

if (!string.IsNullOrEmpty(userName))
{
    AppState.CurrentUser.Name = userName;
    
    app.Run<DashboardWindow>();
}
app.Dispose ();
public static class AppState 
{
    public static User? CurrentUser { get; set; }
    public static IpAddressService LocalIpAddress = new IpAddressService();
}


/*
var userCred =  PublicKeyBox.GenerateKeyPair();

LocalUser.Contacts.Add(new MessageProfile
{
    PrivateKey = userCred.PrivateKey,
    PublicKey = userCred.PublicKey,
    
});

*/



// Defines a top-level window with border and title
public sealed class ExampleWindow : Runnable<string?>
{
    public ExampleWindow ()
    {
        
        Title = $"Example App ({Application.QuitKey} to quit)";

        // Create input components and labels
        var usernameLabel = new Label { Text = "Username:" };

        var userNameText = new TextField
        {
            // Position text field adjacent to the label
            X = Pos.Right (usernameLabel) + 1,

            // Fill remaining horizontal space
            Width = Dim.Fill ()
        };
        

        // Create login button
        var btnLogin = new Button
        {
            Text = "Login",
            Y = Pos.Bottom (usernameLabel) + 1,

            // center the login button horizontally
            X = Pos.Center (),
            IsDefault = true
        };

        // When login button is clicked display a message popup
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

                                  // When Accepting is handled, set e.Handled to true to prevent further processing.
                                  e.Handled = true;
                              };

        // Add the views to the Window
        Add (usernameLabel, userNameText, btnLogin);
    }
}

public class DashboardWindow : Window // Vagy Runnable<bool>
{
    public DashboardWindow()
    {
        Title = "Dashboard";
        var label = new Label { Text = $"Welcome {AppState.CurrentUser!.Name}!", X = Pos.Center(), Y = Pos.Center() };
        var btnLogout = new Button { Text = "Logout", X = Pos.Center(), Y = Pos.Bottom(label) + 1 };
        
        btnLogout.Accepting += (s, e) => {
            RequestStop();
            e.Handled = true;
        };

        Add(label, btnLogout);
    }
}