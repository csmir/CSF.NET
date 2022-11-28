namespace CSF.Samples.Console.Modules
{
    public sealed class DependencyModule : ModuleBase<CommandContext>
    {
        private readonly bool _selfCreated;
        private readonly HttpClient _client;

        public DependencyModule(HttpClient client, bool selfCreated)
        {
            _client = client;
            _selfCreated = selfCreated;
        }

        // This constructor will be used for dependency injection. Without this attribute, the above constructor will be used instead.
        [InjectionConstructor]
        public DependencyModule(HttpClient? client = null) // The httpclient is nullable, meaning that it can set null if the service provider does not contain it.
            : this(client ?? new(), client is null)
        {

        }

        [Command("get-string")]
        public async Task<ExecuteResult> GetAsync()
            => await SuccessAsync(await _client.GetStringAsync("https://mydomain/user/123?format=json", default));
    }
}
