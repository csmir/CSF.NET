using Spectre.Console;
using System.Threading.Tasks;

namespace CSF.Spectre
{
    public class SpectreCommandBase : CommandBase<CommandContext>
    {
        public override void RespondError(string message)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
        }

        public override Task RespondErrorAsync(string message)
        {
            RespondError(message);
            return Task.CompletedTask;
        }

        public override void RespondInformation(string message)
        {
            AnsiConsole.MarkupLineInterpolated($"[yellow]{message}[/]");
        }

        public override Task RespondInformationAsync(string message)
        {
            RespondInformation(message);
            return Task.CompletedTask;
        }

        public override void RespondSuccess(string message)
        {
            AnsiConsole.MarkupLineInterpolated($"[green]{message}[/]");
        }

        public override Task RespondSuccessAsync(string message)
        {
            RespondSuccess(message);
            return Task.CompletedTask;
        }

        public override void Respond(string message)
        {
            AnsiConsole.MarkupLine($"{message}");
        }

        public override Task RespondAsync(string message)
        {
            Respond(message);
            return Task.CompletedTask;
        }
    }
}
