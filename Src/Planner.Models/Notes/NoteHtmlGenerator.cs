using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;

namespace Planner.Models.Notes
{
    public interface INoteHtmlGenerator
    {
        Task GenerateResponse(string url, TextWriter destination);
    }

    public class NoteHtmlGenerator : INoteHtmlGenerator
    {
        private static bool TryParseLocalDate(string s, out LocalDate ret)
        {
            if (DateTime.TryParse(s, out var dt))
            {
                ret = LocalDate.FromDateTime(dt);
                return true;
            }
            ret = LocalDate.MinIsoValue;
            return false;
        }
        public Task GenerateResponse(string url, TextWriter destination)
        {
            if (TryParseLocalDate(url, out var date)) return PlannerDayView(date, destination);
            destination.Write("<html><body></body></html>");
            return Task.CompletedTask;
        }

        private async Task PlannerDayView(LocalDate date, TextWriter destination)
        {
            
        }
    }
}