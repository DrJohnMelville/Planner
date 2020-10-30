using System;
using System.Collections.Generic;
using NodaTime;
using Planner.Models.Notes;

namespace Planner.Models.HtmlGeneration
{
    public class NoteEditRequestEventArgs : EventArgs
    {
        public IList<Note> DailyList { get; }
        public Note Note { get; }

        public NoteEditRequestEventArgs(IList<Note> dailyList, Note note)
        {
            DailyList = dailyList;
            Note = note;
        }
    }

    public class PlannerNagivateRequestEventArgs:EventArgs
    {
        public PlannerNagivateRequestEventArgs(LocalDate date)
        {
            Date = date;
        }

        public LocalDate Date { get; }
    }
}