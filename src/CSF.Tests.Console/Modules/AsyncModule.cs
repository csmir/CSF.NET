namespace CSF.Tests.Console.Modules
{
    public sealed class AsyncModule : ModuleBase<CommandContext>
    {
        [Command("delayed")]
        public async Task AsyncRunDelayed()
        {
            await Task.Delay(5000);

            Respond("Success. (Delayed).");
        }

        [Command("direct")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AsyncRunDirect()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            => Respond("Success. (Direct).");
    }
}
