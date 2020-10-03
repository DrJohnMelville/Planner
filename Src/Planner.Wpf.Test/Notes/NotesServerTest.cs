using Planner.WpfViewModels.Notes;
using Xunit;

namespace Planner.Wpf.Test.Notes
{
    public class NotesServerTest
    {
        public readonly NotesServer sut;

        public NotesServerTest()
        {
            sut = new NotesServer();
        }

        [Fact]
        public void CorrectUrl()
        {
            Assert.Equal("http://localhost:72875/", sut.BaseUrl);
            
        }
    }
}