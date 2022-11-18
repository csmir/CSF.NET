using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CSF.Utils
{
    internal static class ComponentExtensions
    {
        // todo: improve formatting
        public static string GetQualifiedNames(this IConditionalComponent component)
        {
            var values = new List<string>();

            if (component is Module module)
                values.AddRange(GetRoot(module));
            else
                values.Add(component.Name);

            return string.Join(" -> ", values);
        }

        private static List<string> GetRoot(Module components)
        {
            var values = new List<string>();

            foreach (var component in components.Components)
                if (component is Module module)
                    values.AddRange(GetRoot(module));
                else
                    values.Add(component.Name);

            return values;
        }
    }
}
