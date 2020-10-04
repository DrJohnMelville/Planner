using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Melville.MVVM.FileSystem;

namespace Planner.Models.HtmlGeneration
{
    public interface IStaticFiles
    {
        bool TryGetValue(string name, [NotNullWhen(true)] out byte[]? value);
    }

    public class StaticFiles : IStaticFiles
    {
        private readonly Dictionary<string, byte[]> files = 
            new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public StaticFiles()
        {
            const string prefix = "Planner.Models.HtmlGeneration.Content.";
            var assembly = GetType().Assembly;
            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (name.StartsWith(prefix) &&
                    assembly.GetManifestResourceStream(name) is {} stream)
                {
                    files[name[prefix.Length..]] = stream.ReadToArray();
                }
            }
        }

        public bool TryGetValue(string name, [NotNullWhen(true)] out byte[]? value) => 
            files.TryGetValue(name, out value);
    }
}