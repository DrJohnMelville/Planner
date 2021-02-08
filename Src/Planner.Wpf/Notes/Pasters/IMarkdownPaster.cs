using System.Threading.Tasks;
using System.Windows;
using NodaTime;

namespace Planner.Wpf.Notes.Pasters
{
    public interface IMarkdownPaster
    {
        ValueTask<string?> GetPasteText(IDataObject clipboard, LocalDate targetDate);
    }

    public abstract class SynchronousPaster : IMarkdownPaster
    {
        private string dataType;

        protected SynchronousPaster(string dataType)
        {
            this.dataType = dataType;
        }

        public ValueTask<string?> GetPasteText(IDataObject clipboard, LocalDate targetDate) =>
            new ValueTask<string?>(SynchronousGetPasteText(clipboard));

        private string? SynchronousGetPasteText(IDataObject clipboard) =>
            clipboard.GetDataPresent(dataType)?
                ResultFromObject(clipboard.GetData(dataType)): 
                null;

        protected abstract string? ResultFromObject(object? data);
    }

    public class StringPaster : SynchronousPaster
    {
        public StringPaster(string dataType) : base(dataType)
        {
        }

        protected override string? ResultFromObject(object? data) => data?.ToString() ?? null;
    }
}