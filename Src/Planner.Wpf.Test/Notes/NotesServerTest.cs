using Moq;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.WpfViewModels.Notes;
using Xunit;

namespace Planner.Wpf.Test.Notes
{
    public class NotesServerTest
    {
        public readonly NotesServer sut;

        public NotesServerTest()
        {
            sut = new NotesServer(Mock.Of<INoteHtmlGenerator>());
        }

        [Fact]
        public void CorrectUrl()
        {
            Assert.Equal("http://localhost:28775/", sut.BaseUrl);
        }
    }
}