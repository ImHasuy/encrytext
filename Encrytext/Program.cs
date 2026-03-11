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
    IpAddress = AppState.LocalIpAddress.GetIpAddress(),
    CurrentMessageProfile = null
};
    
IApplication app = Application.Create ().Init ();

#region Routing
var userName = app.Run<SinginWindow> ().GetResult<string> ();

if (!string.IsNullOrEmpty(userName))
{
    AppState.CurrentUser.Name = userName;
    app.Run<DashboardWindow>();
    if (AppState.CurrentUser.CurrentMessageProfile != null)
    {
        app.Run<ChatWindow>();
    }
    
}
#endregion


app.Dispose ();
public static class AppState 
{
    public static User? CurrentUser { get; set; }
    public static IpAddressService LocalIpAddress = new IpAddressService();
}


