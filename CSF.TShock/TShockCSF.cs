using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TShockAPI;

namespace CSF.TShock
{
    /// <summary>
    ///     Represents a <see cref="CommandStandardizationFramework"/> intended to be functional for TShock plugins.
    /// </summary>
    public class TShockCSF : CommandStandardizationFramework
    {
        /// <summary>
        ///     Represents the configuration for the framework in its current state.
        /// </summary>
        public new TShockCommandConfiguration Configuration { get; }
        
        /// <summary>
        ///     Creates a new <see cref="TShockCSF"/> for processing modules inside the framework.
        /// </summary>
        /// <param name="config"></param>
        public TShockCSF(TShockCommandConfiguration config)
            : base (config)
        {
            config.InvokeOnlyNameRegistrations = true;

            Configuration = config;
            base.CommandRegistered += CommandRegistered;
        }

        private new Task CommandRegistered(CommandInfo arg)
        {
            var permissions = new List<string>();
            bool shouldReplace = false;
            foreach (var attribute in arg.Attributes)
            {
                if (attribute is RequirePermissionAttribute permAttribute)
                    permissions.Add(permAttribute.PermissionNode);

                if (attribute is ReplaceExistingAttribute replaceAttribute)
                    shouldReplace = replaceAttribute.ShouldReplace;
            }

            if (shouldReplace)
                Commands.ChatCommands.RemoveAll(x => x.Names.Any(o => arg.Aliases.Any(n => o == n)));

            Commands.ChatCommands.Add(new Command(string.Join(".", permissions), async (x) => await ExecuteCommandAsync(x), arg.Aliases));

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
