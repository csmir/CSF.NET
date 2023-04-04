using CSF.Tests.Complex;

namespace CSF.Tests
{
    public class Module : ModuleBase<CommandContext>
    {
        private readonly IParser _parser;
        private readonly CommandManager _framework;

        public Module(IParser parser, CommandManager framework)
        {
            _framework = framework;
            _parser = parser;
        }

        [Command("multiple")]
        public void Test(bool truee, bool falsee)
        {
            Respond($"Success: {truee}, {falsee}");
        }

        [Command("multiple")]
        public void Test(int i1, int i2)
        {
            Respond($"Success: {i1}, {i2}");
        }

        [Command("optional")]
        public void Test(int i = 0, string str = "")
        {
            Respond($"Success: {i}, {str}");
        }

        [Command("nullable")]
        public void Nullable(long? l)
        {
            Respond($"Success: {l}");
        }

        [Command("complex")]
        public IResult Complex([Complex] ComplexType complex)
        {
            return Respond($"({complex.X}, {complex.Y}, {complex.Z}) {complex.Complexer}: {complex.Complexer.X}, {complex.Complexer.Y}, {complex.Complexer.Z}");
        }

        [Command("complexnullable")]
        public IResult Complex([Complex] ComplexerType? complex)
        {
            return Respond($"({complex?.X}, {complex?.Y}, {complex?.Z})");
        }

        [Command("findcommand", "commandinfo", "getcommand", "matches")]
        public async Task<IResult> GetCommandInfo([Remainder] string command)
        {
            if (!_parser.TryParse(command, out var output))
                return Respond("Failed to parse the input as valid command input.");

            var context = new CommandContext(output);

            var searchResult = await _framework.SearchAsync(context, default);

            if (!searchResult.IsSuccess)
                return Respond("Failed to find any commands with this name.");

            foreach (var result in searchResult.Result)
            {
                if (result != null)
                    Respond(result.ToString());
            }

            return ExecuteResult.FromSuccess();
        }
    }
}
