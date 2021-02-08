using System.Globalization;
using System.Windows;
using Planner.Wpf.TaskList;
using Planner.Wpf.TaskList;
using Xunit;

namespace Planner.Wpf.Test.TaskList
{
    public class AndVisibilityConverterTest
    {
        [Theory]
        [InlineData(true, true, true, Visibility.Visible)]
        [InlineData(false, true, true, Visibility.Collapsed)]
        [InlineData(true, true, false, Visibility.Collapsed)]
        [InlineData(true, true, 1, Visibility.Collapsed)]
        [InlineData(true, true, null, Visibility.Collapsed)]
        public void AndVisibilityTest(object o1, object o2, object o3, Visibility visibility)
        {
            Assert.Equal(visibility, AndVisibilityConverter.Instance.Convert(
                new []{o1,o2,o3}, typeof(bool), null, CultureInfo.CurrentCulture));
            
        }

        [Fact]
        public void NullReturnsCollapsed()
        {
            Assert.Equal(Visibility.Collapsed, AndVisibilityConverter.Instance.Convert(
                null, typeof(bool), null, CultureInfo.CurrentCulture));
            
        }
    }
}