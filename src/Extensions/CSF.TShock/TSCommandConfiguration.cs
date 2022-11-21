namespace CSF.TShock
{
    /// <summary>
    ///     Configuration for the <see cref="TSCommandFramework"/>.
    /// </summary>
    public class TSCommandConfiguration : CommandConfiguration
    {
        /// <summary>
        ///     Defines if all existing commands should be replaced.
        /// </summary>
        /// <remarks>
        ///     If a command with a matching name is registered after the plugin in question is registered, this feature will not override registered command.
        ///     <br/><br/>
        ///     Make sure to call <see cref="CommandFramework.BuildModulesAsync(System.Reflection.Assembly)"/> post-initialization to guarantee successful replacement.
        /// </remarks>
        public bool ReplaceAllExisting { get; set; } = false;
    }
}
