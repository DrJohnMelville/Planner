using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Windows.Media.MediaProperties;
using Melville.MVVM.Wpf.Clipboards;
using Moq;
using NodaTime;
using Planner.Models.Blobs;
using Planner.WpfViewModels.Notes.Pasters;
using Xunit;

namespace Planner.Wpf.Test.Notes.Pasters
{
    public class MarkdownPasterTest
    {
        private readonly Mock<IDataObject> clip = new Mock<IDataObject>();
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public MarkdownPasterTest()
        {
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, "B", "B")]
        [InlineData("A", "B", "A")]
        [InlineData("A", null, "A")]
        public async Task CompositeTester(string a, string b, string result)
        {
            var pa = new Mock<IMarkdownPaster>();
            pa.Setup(i => i.GetPasteText(clip.Object, date)).ReturnsAsync(a);
            var pb = new Mock<IMarkdownPaster>();
            pb.Setup(i => i.GetPasteText(clip.Object,date)).ReturnsAsync(b);
            
            var sut = new CompositeMarkdownPaster(new IMarkdownPaster[]{pa.Object, pb.Object});
            Assert.Equal(result, await sut.GetPasteText(clip.Object, date));
            
        }

        private void PutTextInClipboard(string format, object text)
        {
            clip.Setup(i => i.GetDataPresent(It.IsAny<string>()))
                .Returns((string tdf)=> tdf == format);
            clip.Setup(i => i.GetData(format)).Returns(text);
        }

        [Theory]
        [InlineData("UnicodeText", "Pasted")]
        [InlineData("HTML Format", null)]
        public async Task ReadTextFromClipboard(string fmt, string result)
        {
            PutTextInClipboard(fmt, "Pasted");
            var sut = new StringPaster(DataFormats.UnicodeText);
            Assert.Equal(result, await sut.GetPasteText(clip.Object, date));
        }

        [Theory]
        [InlineData("UnicodeText", null)]
        [InlineData("HTML Format", "Pasted")]
        public async Task ReadHtmlFromClipboard(string fmt, string result)
        {
            PutTextInClipboard(fmt, "SFAK<!--StartFragment-->Pasted<!--EndFragment-->");
            var sut = new HtmlMarkdownPaster();
            Assert.Equal(result, await sut.GetPasteText(clip.Object, date));
        }

        [Theory]
        [InlineData("UnicodeText", "a,b\r\nc,d", null)]
        [InlineData("CSV", "a,b\r\nc,d", "a|b\r\n---|---\r\nc|d\r\n")]
        [InlineData("CSV", "a,b\r\nc,d\r\nf,g", "a|b\r\n---|---\r\nc|d\r\nf|g\r\n")]
        [InlineData("CSV", "a,b", "a|b\r\n---|---\r\n")]
        public async Task CSVParser(string format, string text, string result)
        {
            PutTextInClipboard(format, text);
            var sut = new CsvPaster();
            Assert.Equal(result, await sut.GetPasteText(clip.Object, date));
        }

        [Fact]
        public async Task ParsePng()
        {
            var payloed = new MemoryStream(new byte[]{1,2});
            PutTextInClipboard("PNG", payloed);
            var blobCreaor = new Mock<IBlobCreator>();
            var result = "![Pasted Photo](/0/7.28.1975#1)";
            blobCreaor.Setup(i => i.MarkdownForNewImage("Pasted Photo", "image/png", date, payloed))
                .ReturnsAsync(result);
            var sut = new PngMarkdownPaster(blobCreaor.Object);
            Assert.Equal(result, await sut.GetPasteText(clip.Object, date));
            
        }
    }
}