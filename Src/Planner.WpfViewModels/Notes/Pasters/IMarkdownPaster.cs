using NodaTime;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public interface IMarkdownPaster
    {
        string? GetPasteText(LocalDate targetDate);
    }
}