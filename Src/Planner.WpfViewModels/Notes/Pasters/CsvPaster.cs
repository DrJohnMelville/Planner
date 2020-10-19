using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using CsvHelper;
using Melville.MVVM.Wpf.Clipboards;
using NodaTime;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public class CsvPaster: IMarkdownPaster
    {
        private readonly IReadFromClipboard rfc;

        public CsvPaster(IReadFromClipboard rfc)
        {
            this.rfc = rfc;
        }

        public string? GetPasteText(LocalDate targetDate) => 
            rfc.ContainsText(TextDataFormat.CommaSeparatedValue) ? 
                Parse(rfc.GetText(TextDataFormat.CommaSeparatedValue)) : 
                null;

        private string Parse(string source)
        {
            var ret = new StringBuilder();
            var reader = new CsvReader(new StringReader(source), CultureInfo.CurrentUICulture);
            if (!reader.Read()) return "";
            var cols = WriteSingleLine(reader, ret);
            WriteFenceLine(ret, cols);
            while (reader.Read())
            {
                WriteSingleLine(reader, ret);
            }

            return ret.ToString();
        }

        private void WriteFenceLine(StringBuilder ret, in int cols)
        {
            ret.AppendLine(string.Join("|", Enumerable.Repeat("---", cols)));
        }

        private static int WriteSingleLine(CsvReader reader, StringBuilder ret)
        {
            var col = 0;
            while (reader.TryGetField(typeof(string), col++, out var cell))
            {
                if (col != 1) ret.Append('|');
                ret.Append(cell);
            }
            ret.AppendLine();
            return col-1;
        }
    }
}