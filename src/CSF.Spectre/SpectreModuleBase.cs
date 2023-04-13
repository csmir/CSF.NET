using Spectre.Console;
using System;
[assembly: CLSCompliant(true)]

namespace CSF.Spectre
{
    /// <inheritdoc/>
    public class SpectreModuleBase<T> : ModuleBase<T>
        where T : IContext
    {
        /// <inheritdoc/>
        public override void Respond(string message)
        {
            AnsiConsole.MarkupLineInterpolated($"{message}");
        }

        /// <summary>
        ///     Returns the input value of the requested question.
        /// </summary>
        /// <param name="question">The question to ask.</param>
        /// <returns>The string with selected value within.</returns>
        public string Ask(string question)
        {
            return AnsiConsole.Ask<string>(question);
        }

        /// <summary>
        ///     Returns the input value of the requested prompt.
        /// </summary>
        /// <param name="prompt">The prompt to request a value for.</param>
        /// <returns>The string with selected value within.</returns>
        [CLSCompliant(false)]
        public string Prompt(TextPrompt<string> prompt)
        {
            return AnsiConsole.Prompt(prompt);
        }

        /// <summary>
        ///     Returns the value of a selected <see cref="SelectionPrompt{T}"/> item.
        /// </summary>
        /// <param name="selection">The selection to pick from.</param>
        /// <returns>The string with selected value within.</returns>
        [CLSCompliant(false)]
        public string Select(SelectionPrompt<string> selection)
        {
            return AnsiConsole.Prompt(selection);
        }
    }
}
