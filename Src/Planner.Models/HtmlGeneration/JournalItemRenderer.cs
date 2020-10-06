using System.Collections.Generic;
using System.IO;
using System.Linq;
using Planner.Models.Markdown;
using Planner.Models.Notes;

namespace Planner.Models.HtmlGeneration
{
    public class JournalItemRenderer 
    {
        private readonly TextWriter destination;
        private readonly IMarkdownTranslator markdown;
        private readonly INoteUrlGenerator urlGenerator;

        public JournalItemRenderer(TextWriter destination, IMarkdownTranslator markdown, INoteUrlGenerator urlGenerator)
        {
            this.destination = destination;
            this.markdown = markdown;
            this.urlGenerator = urlGenerator;
        }

        public void WriteJournalList(IList<Note> notes, Note? desiredNote = null)
        {
            WritePrologue();
            int position = 1;
            foreach (var note in notes.OrderBy(i => i.TimeCreated))
            {
                TryRenderHorizontalRule(desiredNote, position);
                if (ShouldRenderThisNote(desiredNote, note)) GenerateNote(note, position);
                position++;
            }
            WriteEpilogue();
        }

        private void TryRenderHorizontalRule(Note? desiredNote, int position)
        {
            if (position > 1 && desiredNote == null)
            {
                destination.Write("<hr/>");
            }
        }

        private static bool ShouldRenderThisNote(Note? desiredNote, Note note) => 
            desiredNote == null || note == desiredNote;

        private void WritePrologue() => destination.Write("<html><head><link rel=\"stylesheet\" href=\"journal.css\"></head><body>");

        private void WriteEpilogue() => destination.Write("</body></html>");

        private void GenerateNote(Note note, int itemNumber)
        {
            destination.Write("<h3>");
            destination.Write($"<a href=\"{urlGenerator.EditNoteUrl(note)}\">");
            destination.Write(itemNumber);
            destination.Write(".");
            destination.Write("</a>");
            destination.Write(" ");
            destination.Write(markdown.RenderLine(note.Title));
            destination.Write("</h3>");
            destination.Write("<div>");
            destination.Write(markdown.Render(note.Text));
            destination.Write("</div>");
        }
    }
}