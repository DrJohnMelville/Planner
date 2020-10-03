using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Notes
{
    public class NoteHtmlGeneratorTest
    {
        private readonly Mock<ILocalRepository<Note>> repo = new Mock<ILocalRepository<Note>>();
        private readonly List<Note> notes = new List<Note>();
        private readonly NoteHtmlGenerator sut;
        private readonly StringWriter output = new StringWriter();
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public NoteHtmlGeneratorTest()
        {
            repo.Setup(i => i.TasksForDate(date)).Returns(notes);
            sut = new NoteHtmlGenerator();
        }

        [Fact]
        public async Task EmptyTest()
        {
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Equal("", output.ToString());
            
        }
        [Fact]
        public async Task SingleTest()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Equal("", output.ToString());
            
        }
    }
}