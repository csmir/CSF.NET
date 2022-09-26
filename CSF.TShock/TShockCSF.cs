using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TShockAPI;

namespace CSF.TShock
{
    public class TShockCSF : CommandStandardizationFramework
    {
        public TShockCSF(CommandConfiguration config)
            : base (config)
        {
            config.InvokeOnlyNameRegistrations = true;
            base.CommandRegistered += CommandRegistered;
        }

        private new Task CommandRegistered(CommandInfo arg)
        {
            var permissions = new List<string>();
            foreach (var attribute in arg.Attributes)
            {
                if (attribute is RequirePermissionAttribute permAttribute)
                    permissions.Add(permAttribute.PermissionNode);
            }

            TShockAPI.Commands.ChatCommands.Add(new Command(string.Join(".", permissions), async (x) => await ExecuteCommandAsync(x), arg.Aliases));

            return Task.CompletedTask;
        }

        public async Task ExecuteCommandAsync(CommandArgs args)
        {
            var context = new TShockCommandContext(args, args.Message);

            var result = await base.ExecuteCommandAsync(context);
            
            if (!result.IsSuccess)
                args.Player.SendErrorMessage(result.ErrorMessage);
        }
    }
}
