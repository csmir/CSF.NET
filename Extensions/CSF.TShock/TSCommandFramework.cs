using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace CSF.TShock
{
    /// <summary>
    ///     Represents a <see cref="CommandStandardizationFramework"/> intended to be functional for TShock plugins.
    /// </summary>
    public class TSCommandFramework : CommandStandardizationFramework
    {
        /// <summary>
        ///     Represents the configuration for the framework in its current state.
        /// </summary>
        public new TSCommandConfiguration Configuration { get; }

        /// <summary>
        ///     Creates a new <see cref="TSCommandFramework"/> for processing modules inside the framework.
        /// </summary>
        /// <param name="config"></param>
        public TSCommandFramework(TSCommandConfiguration config, TerrariaPlugin registrator)
            : base(config)
        {
            config.InvokeOnlyNameRegistrations = true;

            Configuration = config;
            base.CommandRegistered += CommandRegistered;

            var assembly = registrator.GetType().Assembly;

            var result = BuildModulesAsync(assembly).GetAwaiter().GetResult();

            if (!result.IsSuccess)
                TShockAPI.TShock.Log.ConsoleError(result.ErrorMessage);
        }

        private new Task CommandRegistered(CommandInfo arg)
        {
            var permissions = new List<string>();
            bool shouldReplace = false;
            string description = "";
            foreach (var attribute in arg.Attributes)
            {
                if (attribute is RequirePermissionAttribute permAttribute)
                    permissions.Add(permAttribute.PermissionNode);

                if (attribute is ReplaceExistingAttribute replaceAttribute)
                    shouldReplace = replaceAttribute.ShouldReplace;

                if (attribute is DescriptionAttribute descriptionAttribute)
                    description = descriptionAttribute.Description;
            }

            if (shouldReplace)
                Commands.ChatCommands.RemoveAll(x => x.Names.Any(o => arg.Aliases.Any(n => o == n)));

            Commands.ChatCommands.Add(new Command(string.Join(".", permissions), async (x) => await ExecuteCommandAsync(x), arg.Aliases)
            {
                HelpText = description
            });

            return Task.CompletedTask;
        }

        public virtual async Task<ITSCommandContext> CreateContextAsync(CommandArgs args, string rawInput)
        {
            await Task.CompletedTask;
            return new TSCommandContext(args, rawInput);
        }

        public virtual async Task ExecuteCommandAsync(CommandArgs args)
        {
            var context = await CreateContextAsync(args, args.Message);

            var result = await ExecuteCommandAsync(context);

            if (!result.IsSuccess)
                args.Player.SendErrorMessage(result.ErrorMessage);
        }
    }
}
