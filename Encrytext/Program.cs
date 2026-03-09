using Encrytext.Core.Entity;
using Encrytext.Core.Services;
using Encrytext.UI.Menu;
using Encrytext.UI.Screens;
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

#region Routing
var userName = app.Run<SinginWindow> ().GetResult<string> ();

if (!string.IsNullOrEmpty(userName))
{
    AppState.CurrentUser.Name = userName;
    app.Run<DashboardWindow>();
}
#endregion


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

