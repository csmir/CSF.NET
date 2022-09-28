namespace CSF.Console
{
    // Preconditions are allowed to exist on class & method level.
    // This precondition will return if the provided ICommandContext does not directly match CommandContext.
    [RequireContext(typeof(CommandContext))]
    public sealed class ExampleModule : CommandBase<CommandContext>
    {
        // This is a simple command with 1 parameter.
        //
        // 'example "this is an example input"'
        [Command("example")]
        public async Task<ExecuteResult> HandleAsync(string input1)
        {
            await RespondSuccessAsync($"Succesfully received your command! Input: {input1}");

            // You can return execute results to get error or success results out to the post-execution handler.
            return ExecuteResult.FromSuccess();
        }

        // This is a simple command with 2 parameters, one of which will throw an error if it's not provided as integer.
        //
        // 'example "this is an example input" 14' will succeed.
        // 'example "this is an example input" hi' will fail.
        [Command("example")]
        public async Task HandleAsync(string input1, int input2)
        {
            await RespondSuccessAsync($"Succesfully received your command! Input: {input1} && {input2}");
        }

        // This is a more advanced command with 4 parameters, 2 of which optional, and one the remainder.
        //
        // 'example "this is" "an example input" 14 and it has some pretty cool features' will succeed.
        // 'example "this is" "an example input" 14' will also succeed, because the 'remainder' parameter is optional.
        // 'example "this is" "an example input" will fail, because it will try to parse the best matching length, thus the command here above.
        // 
        // If you were to remove the method above, this method will work for the last input.
        [Command("example")]
        public async Task HandleAsync(string input1, string input2, int input3 = 0, [Remainder] string remainder = "")
        {
            await RespondSuccessAsync($"Succesfully received your command! Input: {input1} && {input2} && {input3} && {remainder}");
        }

        // This example will fail when the app is ran, because a command cannot be registered without a name.
        //[Command("")]
        //public async Task ErrorAsync()
        //{
        //    await Task.CompletedTask;
        //}

        // This is a command with a unique precondition attribute.
        // The precondition will check the current environment for its os version, and compare the platform.
        //
        // 'require-unix' will fail if the platform is not unix. 
        [Command("require-unix")]
        [RequireOperatingSystem(PlatformID.Unix)]
        public IResult UnixAsync()
        {
            RespondSuccess("Success!");

            // Notice how this method is sync, for which is also full support, both returning void and result.
            return ExecuteResult.FromSuccess();
        }
    }
}
