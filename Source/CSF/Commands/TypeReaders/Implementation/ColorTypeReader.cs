using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSF
{
    internal class ColorTypeReader : TypeReader<Color>
    {
        private readonly Dictionary<string, Color> _colors;
        private readonly IReadOnlyDictionary<string, string> _spacedColors;

        public ColorTypeReader()
        {
            var properties = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            var colors = new Dictionary<string, Color>(properties.Length - 1, StringComparer.OrdinalIgnoreCase);

            foreach (var property in properties)
                colors[property.Name] = (Color)property.GetValue(null);

            _colors = colors;

            var spacedNames = new Dictionary<string, string>(_colors.Comparer);
            var builder = new StringBuilder();
            foreach (var name in _colors.Keys)
            {
                builder.Clear();
                var hasSpace = false;
                for (var i = 0; i < name.Length; i++)
                {
                    var character = name[i];
                    if (i != 0 && char.IsUpper(character))
                    {
                        hasSpace = true;
                        builder.Append(' ');
                    }

                    builder.Append(character);
                }

                if (hasSpace)
                    spacedNames[builder.ToString()] = name;
            }

            _spacedColors = spacedNames;
        }

        public override Task<TypeReaderResult> ReadAsync(IContext context, Parameter info, string value, IServiceProvider provider)
        {
            if (int.TryParse(value.Replace("#", "").Replace("0x", ""), NumberStyles.HexNumber, null, out var hexNumber))
                return Task.FromResult(TypeReaderResult.FromSuccess(Color.FromArgb(hexNumber)));

            var name = value;

            _spacedColors.TryGetValue(name, out name);

            if (_colors.TryGetValue(name, out var color))
                return Task.FromResult(TypeReaderResult.FromSuccess(color));

            return Task.FromResult(TypeReaderResult.FromError($"The provided value is not a color. Expected {typeof(Color).Name}, got: '{value}'. At: '{info.Name}'"));
        }
    }
}
