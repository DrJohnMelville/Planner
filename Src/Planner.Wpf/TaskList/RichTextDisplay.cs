using System.Windows;
using System.Windows.Controls;

namespace Planner.Wpf.TaskList
{
    public class RichTextDisplay : Control
    {
        static RichTextDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextDisplay),
                new FrameworkPropertyMetadata(typeof(RichTextDisplay)));
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(RichTextDisplay),
            new FrameworkPropertyMetadata(null));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}