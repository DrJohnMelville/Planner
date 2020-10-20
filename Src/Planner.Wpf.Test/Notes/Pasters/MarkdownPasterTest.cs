using System.Runtime.InteropServices;
using System.Windows;
using Melville.MVVM.Wpf.Clipboards;
using Moq;
using NodaTime;
using Planner.WpfViewModels.Notes.Pasters;
using Xunit;

namespace Planner.Wpf.Test.Notes.Pasters
{
    public class MarkdownPasterTest
    {
        private readonly Mock<IReadFromClipboard> readFromClipboard = new Mock<IReadFromClipboard>();
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public MarkdownPasterTest()
        {
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, "B", "B")]
        [InlineData("A", "B", "A")]
        [InlineData("A", null, "A")]
        public void CompositeTester(string a, string b, string result)
        {
            var pa = new Mock<IMarkdownPaster>();
            pa.Setup(i => i.GetPasteText(date)).Returns(a);
            var pb = new Mock<IMarkdownPaster>();
            pb.Setup(i => i.GetPasteText(date)).Returns(b);
            
            var sut = new CompositeMarkdownPaster(new IMarkdownPaster[]{pa.Object, pb.Object});
            Assert.Equal(result, sut.GetPasteText(date));
            
        }

        private void PutTextInClipboard(TextDataFormat format, string text)
        {
            readFromClipboard.Setup(i => i.ContainsText(It.IsAny<TextDataFormat>()))
                .Returns((TextDataFormat tdf)=> tdf == format);
            readFromClipboard.Setup(i => i.GetText(format)).Returns(text);
        }

        [Theory]
        [InlineData(TextDataFormat.UnicodeText, "Pasted")]
        [InlineData(TextDataFormat.Html, null)]
        public void ReadTextFromClipboard(TextDataFormat fmt, string result)
        {
            PutTextInClipboard(fmt, "Pasted");
            var sut = new TextMarkdownPaster(readFromClipboard.Object);
            Assert.Equal(result, sut.GetPasteText(date));
        }

        [Theory]
        [InlineData(TextDataFormat.UnicodeText, null)]
        [InlineData(TextDataFormat.Html, "SFAK<!--StartFragment-->Pasted<!--EndFragment-->")]
        public void ReadHtmlFromClipboard(TextDataFormat fmt, string result)
        {
            PutTextInClipboard(fmt, "Pasted");
            var sut = new HtmlMarkdownPaster(readFromClipboard.Object);
            Assert.Equal(result, sut.GetPasteText(date));
        }

        [Theory]
        [InlineData(TextDataFormat.UnicodeText, "a,b\r\nc,d", null)]
        [InlineData(TextDataFormat.CommaSeparatedValue, "a,b\r\nc,d", "a|b\r\n---|---\r\nc|d\r\n")]
        [InlineData(TextDataFormat.CommaSeparatedValue, "a,b\r\nc,d\r\nf,g", "a|b\r\n---|---\r\nc|d\r\nf|g\r\n")]
        [InlineData(TextDataFormat.CommaSeparatedValue, "a,b", "a|b\r\n---|---\r\n")]
        public void CSVParser(TextDataFormat format, string text, string result)
        {
            PutTextInClipboard(format, text);
            var sut = new CsvPaster(readFromClipboard.Object);
            Assert.Equal(result, sut.GetPasteText(date));
        }
    }
}