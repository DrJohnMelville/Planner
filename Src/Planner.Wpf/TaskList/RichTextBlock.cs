using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Melville.MVVM.Wpf.EventBindings.SearchTree;
using Planner.Models.Tasks;

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

        public static readonly DependencyProperty RenderedTBProperty =
            DependencyProperty.Register("RenderedTB", typeof(TextBlock), typeof(RichTextBlock),
                new FrameworkPropertyMetadata(null));

        public TextBlock RenderedTB
        {
            get => (TextBlock) GetValue(RenderedTBProperty);
            set => SetValue(RenderedTBProperty, value);
        }

        public RichTextBlock()
        {
            BindingOperations.SetBinding(this, RenderedTBProperty, new Binding()
            {
                Path = new PropertyPath("Text"),
                Source = this,
                Converter = RichRenderer.Instance
            });
        }
    }
    public class RichRenderer: IValueConverter
    {
        public static readonly RichRenderer Instance = new RichRenderer();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            RenderString(value?.ToString() ?? "");

        private static readonly TaskNameParser parser = new TaskNameParser();

        private TextBlock RenderString(string source)
        {
            var ret = new TextBlock() {Background = Brushes.White};
            var ac = (IAddChild) ret;
            foreach (var span in parser.Parse(source))
            {
                var run = new Run(span.Text);
                // if (span.Label != TaskTextType.NoLink)
                // {
                //     run.Foreground = Brushes.DodgerBlue;
                //     run.TextDecorations.Add(TextDecorations.Underline)
                // }
                ac.AddChild(span.Label == TaskTextType.NoLink?(Inline)run:CreateLink(run, span.Label));
            }
            return ret;
        }

        private Hyperlink CreateLink(Run run, TaskTextType label)
        {
            var ret = new Hyperlink(run);
            ret.Click += (s, e) => RunOnVisualTreeSearch.Run((DependencyObject)s, 
                label.ToString(), new object[] {e}, out var _);
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
            DependencyProperty.UnsetValue;
    }
}