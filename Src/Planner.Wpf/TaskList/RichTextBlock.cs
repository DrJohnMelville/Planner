using System.Windows;
using System.Windows.Controls;

namespace Planner.Wpf.TaskList
{
    public class RichTextBlock : Control
    {
        static RichTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextBlock),
                new FrameworkPropertyMetadata(typeof(RichTextBlock)));
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(RichTextBlock),
            new FrameworkPropertyMetadata(null));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty EditingProperty = DependencyProperty.Register(
            "Editing", typeof(bool), typeof(RichTextBlock),
            new FrameworkPropertyMetadata(false));

        public bool Editing
        {
            get => (bool) GetValue(EditingProperty);
            set => SetValue(EditingProperty, value);
        }

        public void LeftEdit() => Editing = false;
        public void BeginEdit() => Editing = true;
    }
}