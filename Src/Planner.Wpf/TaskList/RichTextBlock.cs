using System.Windows;
using System.Windows.Controls;
using Melville.WpfControls.Buttons;

namespace Planner.Wpf.TaskList
{
    public class RichTextBlock: Control
    {
        static RichTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichTextBlock), 
                new FrameworkPropertyMetadata(typeof(RichTextBlock)));
        }
    }
}