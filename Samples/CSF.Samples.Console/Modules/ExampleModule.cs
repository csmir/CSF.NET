namespace CSF.Console
{
    public sealed class ExampleModule : ModuleBase<CommandContext>
    {
        // 'example "this is an example input"'
        [Command("example")]
        public async Task<ExecuteResult> HandleAsync(string input1)
            => await SuccessAsync($"Succesfully received your command! Input: {input1}");

        // 'example "this is an example input" 14' will succeed.
        // 'example "this is an example input" hi' will fail.
        [Command("example")]
        public async Task<ExecuteResult> HandleAsync(string input1, int input2)
        {
            return await SuccessAsync($"Succesfully received your command! Input: {input1} && {input2}");
        }

        // 'example "this is" "an example input" 14 and it has some pretty cool features' will succeed.
        // 'example "this is" "an example input" 14' will also succeed, because the 'remainder' parameter is optional.
        // 'example "this is" "an example input" will fail, because it will try to parse the best matching length, thus the command here above.
        
        // If you were to remove the method above, this method will work for the last input.
        [Command("example")]
        public async Task HandleAsync(string input1, string input2, int input3 = 0, [Remainder] string remainder = "")
        {
            await SuccessAsync($"Succesfully received your command! Input: {input1} && {input2} && {input3} && {remainder}");
        }

        // This is a command with a unique precondition attribute.
        // The precondition will check the current environment for its os version, and compare the platform.
        
        // 'require-unix' will fail if the platform is not unix. 
        [Command("require-unix")]
        [RequireOperatingSystem(PlatformID.Unix)]
        public IResult UnixAsync()
        {
            Success("Success!");

            // Notice how this method is sync, for which is also full support, both returning void and result.
            return ExecuteResult.FromSuccess();
        }
    }
}
