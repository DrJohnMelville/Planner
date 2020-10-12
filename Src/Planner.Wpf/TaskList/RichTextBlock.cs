using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
            new FrameworkPropertyMetadata(null, OnTextChanged));

        private TextBlock? target;
        private static void OnTextChanged(DependencyObject d, 
            DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBlock) d).OnTextChanged(e.NewValue?.ToString()??"");
        }

        public void InitializeDisplay(TextBlock target)
        {
            this.target = target;
            OnTextChanged(Text);
        }

        private void OnTextChanged(string toString)
        {
            RichRenderer.Instance.FillTextBlock(toString, target);
        }
        
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }


        public void MouseDoubleClickEvent(Grid grid)
        {
            var tb = (TextBox) grid.Children.OfType<TextBox>().First();
            tb.Focus();
            Keyboard.Focus(tb);
        }

    }
    public class RichRenderer
    {
        public static readonly RichRenderer Instance = new RichRenderer();
        private static readonly TaskNameParser parser = new TaskNameParser();
        public void FillTextBlock(string source, TextBlock? ret)
        {
            if (ret == null) return;
            ret.Inlines.Clear();
            var ac = (IAddChild) ret;
            foreach (var span in parser.Parse(source))
            {
                var run = new Run(span.Text);
                ac.AddChild(span.Label == TaskTextType.NoLink ? (Inline) run : CreateLink(run, span.Label));
            }
        }

        private Hyperlink CreateLink(Run run, TaskTextType label)
        {
            var ret = new Hyperlink(run);
            ret.Background= Brushes.Transparent;
            ret.IsEnabled = true;
            ret.Click += (s, e) => RunOnVisualTreeSearch.Run((DependencyObject)s, 
                label.ToString(), new object[] {e}, out var _);
            return ret;
        }
    }
}