﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Planner.Models.Tasks;
using Xunit;

namespace Planner.Models.Test.Tasks
{
    public class TaskNameParserTest
    {
        private readonly TaskNameParser sut = new TaskNameParser();

        private List<Segment<TaskTextType>> DoParse(string text) => 
            sut.Parse(text).ToList();

        [Fact]
        public void TestName()
        {
            var list = DoParse("Foo");
            Assert.Single(list);
            Assert.Equal("Foo", list[0].Text);
            Assert.Equal(TaskTextType.NoLink, list[0].Label);
            
        }

        [Fact]
        public void SimpleDotExpression()
        {
            var list = DoParse("Party (7.8.1) supplies");
            Assert.Equal(3, list.Count);
            Assert.Equal("Party ", list[0].Text);
            Assert.Equal("(7.8.1)", list[1].Text);
            Assert.Equal(" supplies", list[2].Text);
            Assert.Equal(TaskTextType.NoLink, list[0].Label);
            Assert.Equal(TaskTextType.PlannerPage, list[1].Label);
            Assert.Equal(TaskTextType.NoLink, list[2].Label);
            Assert.Equal(4, list[1].Match.Groups.Count);
            Assert.Null(list[0].Match);
            Assert.Null(list[2].Match);
        }

        [Theory]
        [InlineData(" ", 1)]
        [InlineData("(1.2.3)(3.4.5)", 2)]
        [InlineData("(1.2.3) (3.4.5)", 3)]
        [InlineData("(1.2.3)(3.4.5) ", 3)]
        [InlineData(" (1.2.3)(3.4.5)", 3)]
        public void SpanCountingTest(string input, int expectedCount)
        {
            Assert.Equal(expectedCount, DoParse(input).Count);
        }

        [Theory]
        [InlineData("a (1.2.3) b", TaskTextType.PlannerPage)]
        [InlineData("a (1.2.20.3) b", TaskTextType.PlannerPage)]
        [InlineData("a https://zxci.abc.com b", TaskTextType.WebLink)]
        [InlineData("a c:\\xxx.jod b", TaskTextType.FileLink)]
        [InlineData("a \\\\da\\xxx.jod b", TaskTextType.FileLink)]
        [InlineData("a \"c:\\xxx.jod xx yy\" b", TaskTextType.FileLink)]
        [InlineData("a \"\\\\da\\xxx is 333.jod\" b", TaskTextType.FileLink)]
        public void RegcognizeMiddleLink(string text, TaskTextType middleType)
        {
            var list = DoParse(text);
            Assert.Equal(3, list.Count);
            Assert.Equal(middleType, list[1].Label);
            Assert.Equal(text[2..^2], list[1].Text);
            
        }

    }
}