using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace Planner.WpfViewModels.Notes.Pasters
{
    public class CompositeMarkdownPaster : IMarkdownPaster
    {
        private readonly IEnumerable<IMarkdownPaster> pasters;

        public CompositeMarkdownPaster(IEnumerable<IMarkdownPaster> pasters)
        {
            this.pasters = pasters;
        }

        public string? GetPasteText(LocalDate targetDate) =>
            pasters
                .Select(i => i.GetPasteText(targetDate))
                .FirstOrDefault(i => i != null);
    }
}