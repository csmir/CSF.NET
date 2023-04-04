using System.Collections.Generic;
using System.Linq;

namespace CSF
{
    public sealed class ComponentContainer
    {
        public List<IConditionalComponent> Values { get; }

        public ComponentContainer(IEnumerable<ModuleBase> discoveredModules, TypeReaderContainer typeReaders)
        {
            IEnumerable<IConditionalComponent> Yield()
            {
                foreach (var module in discoveredModules)
                    foreach (var component in new Module(typeReaders, module.GetType()).Components)
                        yield return component;
            }

            Values = Yield().ToList();
        }

        public ComponentContainer Include(Module module)
        {
            foreach (var component in module.Components)
            {
                if (!Values.Contains(component))
                    Values.Add(component);
            }

            return this;
        }

        public ComponentContainer Exclude(IConditionalComponent component)
        {
            Values.Remove(component);
            return this;
        }
    }
}
