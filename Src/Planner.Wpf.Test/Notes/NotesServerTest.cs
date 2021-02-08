using System;
using System.Collections.Generic;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Wpf.Notes;
using Xunit;

namespace Planner.Wpf.Test.Notes
{
    public class NotesServerTest
    {
        private readonly Mock<INoteHtmlGenerator> generator = new Mock<INoteHtmlGenerator>();
        public readonly NotesServer sut;

        public NotesServerTest()
        {
            sut = new NotesServer(generator.Object);
        }

        [Fact]
        public void CorrectUrl()
        {
            Assert.Equal("http://localhost:28775/", sut.BaseUrl);
        }
    }
}