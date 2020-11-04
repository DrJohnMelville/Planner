using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Melville.MVVM.Wpf.EventBindings.SearchTree;
using Planner.Models.Tasks;

namespace Planner.Wpf.TaskList
{
    public static class RichRenderer
    {
        private static readonly TaskNameParser parser = new TaskNameParser();
        public static void FillTextBlock(string source, TextBlock ret)
        {
            ret.Inlines.Clear();
            var ac = (IAddChild) ret;
            foreach (var span in parser.Parse(source))
            {
                var run = new Run(span.DisplayText);
                ac.AddChild(span.Label == TaskTextType.NoLink ? SetupRun(run) : 
                    CreateLink(run, span));
            }
        }

        private static Run SetupRun(Run run)
        {
            run.Background = null;
            run.Focusable = true;
            return run;
        }

        private static Hyperlink CreateLink(Run run, Segment<TaskTextType> segment)
        {
            var ret = new Hyperlink(run);
            ret.ToolTip = segment.Text;
            ret.Focusable = false;
            ret.IsEnabled = true;
            ret.Click += (s, e) => RunOnVisualTreeSearch.Run((DependencyObject)s, 
                segment.Label+"LinkClicked", new object[] {e, segment}, out var _);
            return ret;
        }
        
        
        public static readonly DependencyProperty RichTextProperty = DependencyProperty.RegisterAttached(
            "RichText", typeof(string), typeof(RichRenderer),
            new FrameworkPropertyMetadata(null, OnNewRichText));

        public static string GetRichText(DependencyObject obj) => (string) obj.GetValue(RichTextProperty);
        public static void SetRichText(DependencyObject obj, string value) => obj.SetValue(RichTextProperty, value);
        

        private static void OnNewRichText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock tb)
            {
                FillTextBlock(e.NewValue?.ToString()??"", tb);
            }
        }

    }
}