using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using NodaTime;

namespace Planner.Wpf.Notes.Pasters
{
    public class CompositeMarkdownPaster : IMarkdownPaster
    {
        private readonly IEnumerable<IMarkdownPaster> pasters;

        public CompositeMarkdownPaster(IEnumerable<IMarkdownPaster> pasters)
        {
            this.pasters = pasters;
        }

        public async ValueTask<string?> GetPasteText(IDataObject clipboard, LocalDate targetDate)
        {
            foreach (var paster in pasters)
            {
                var ret = await paster.GetPasteText(clipboard, targetDate);
                if (ret != null) return ret;
            }
            return null;
        }
    }
}