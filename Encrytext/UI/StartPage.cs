using Spectre.Console;

namespace Encrytext.UI;

public class StartPage
{
    public void Run()
    {
        var logo = new FigletText("Encrytext")
        {
            Color = Color.DarkRed,
            Justification = Justify.Center
        };
        AnsiConsole.Write(logo);
        
        AnsiConsole.Write(new Rule());
        AnsiConsole.MarkupLine("Welcome to Encrytext!\nPlease provide a username of your choice below:");
        
        var userName = AnsiConsole.Ask<string>("");
        
        AnsiConsole.Clear();
        var table = new Table();
        
        AnsiConsole.MarkupLine($"Welcome, [blue]{userName}[/]!");
        AnsiConsole.MarkupLineInterpolated($"[yellow]Choose an option below: [/]");

        var choices = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("[yellow]Choose an option below: [/]")
            .AddChoices("Search for available users on the network", "Current contacts", "sdf")
        );
        
        var panel = new Panel(choices)
            .BorderColor(Color.Aqua);
        AnsiConsole.Write(panel);
        AnsiConsole.Live(
    }
}