using CSF.Tests.Console.Complex;

namespace CSF.Tests.Modules
{
    public class Module : ModuleBase<CommandContext>
    {
        private readonly IParser _parser;
        private readonly ICommandFramework _framework;

        public Module(IParser parser, ICommandFramework framework)
        {
            _framework = framework;
            _parser = parser;
        }

        [Command("multiple")]
        public void Test(bool truee, bool falsee)
        {
            Success($"Success: {truee}, {falsee}");
        }

        [Command("multiple")]
        public void Test(int i1, int i2)
        {
            Success($"Success: {i1}, {i2}");
        }

        [Command("optional")]
        public void Test(int i = 0, string str = "")
        {
            Success($"Success: {i}, {str}");
        }

        [Command("nullable")]
        public void Nullable(long? l)
        {
            Success($"Success: {l}");
        }

        [Command("complex")]
        public IResult Complex([Complex] ComplexType complex)
        {
            return Success($"({complex.X}, {complex.Y}, {complex.Z}) {complex.Complexer}: {complex.Complexer.X}, {complex.Complexer.Y}, {complex.Complexer.Z}");
        }

        [Command("complexnullable")]
        public IResult Complex([Complex] ComplexerType? complex)
        {
            return Success($"({complex?.X}, {complex?.Y}, {complex?.Z})");
        }

        [Command("findcommand", "commandinfo", "getcommand", "matches")]
        public async Task<IResult> GetCommandInfo([Remainder] string command)
        {
            var parseResult = _parser.Parse(command);

            if (!parseResult.IsSuccess)
                return Error("Failed to parse the input as valid command input.");

            var context = new CommandContext(parseResult);

            var searchResult = await _framework.SearchAsync(context, default);

            if (!searchResult.IsSuccess)
                return Error("Failed to find any commands with this name.");

            foreach (var result in searchResult.Result)
            {
                if (result != null)
                    await SuccessAsync(result.ToString());
            }

            return ExecuteResult.FromSuccess();
        }
    }
}
