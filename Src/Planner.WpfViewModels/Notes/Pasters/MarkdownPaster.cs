using NodaTime;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public interface IMarkdownPaster
    {
        string? GetPasteText(LocalDate targetDate);
    }

    public class MarkdownPaster: IMarkdownPaster
    {
        public string GetPasteText(LocalDate targetDate)
        {
            return "foo";
        }
    }
}