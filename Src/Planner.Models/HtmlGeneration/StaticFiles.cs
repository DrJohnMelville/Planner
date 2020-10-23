using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Melville.MVVM.FileSystem;

namespace Planner.Models.HtmlGeneration
{
    public class StaticFiles :  IHtmlContentOption
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

        public Task? TryRespond(string url, Stream destination)
        {
            return files.TryGetValue(url, out var file)?
                destination.WriteAsync(file).AsTask(): null;
        }
    }
}