using System;
using System.IO;
using System.Linq;

namespace Planner.Wpf.Notes.Pasters
{
    public class FilePaster : StringPaster
    {
        public FilePaster() : base("FileDrop")
        {
        }
          // NEEDS TO MAKE AN ACTUAL FILE LINK
        protected override string? ResultFromObject(object? data) =>
            string.Join("\r\n", (data as string[] ?? Array.Empty<String>()).Select(FormatFile));

        private string FormatFile(string fileName)
        {
            return $"[{Path.GetFileName(fileName)}](/LocalFile/{fileName.Replace(' ', '+')})";
        }
    }
}