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
    
    public static class RichRenderer
    {
        private static readonly TaskNameParser parser = new TaskNameParser();
        public static void FillTextBlock(string source, TextBlock ret)
        {
            ret.Inlines.Clear();
            var ac = (IAddChild) ret;
            foreach (var span in parser.Parse(source))
            {
                var run = new Run(span.Text);
                ac.AddChild(span.Label == TaskTextType.NoLink ? SetupRun(run) : CreateLink(run, span.Label));
            }
        }

        private static Run SetupRun(Run run)
        {
            run.Background = null;
            run.Focusable = true;
            return run;
        }

        private static Hyperlink CreateLink(Run run, TaskTextType label)
        {
            var ret = new Hyperlink(run);
            ret.Focusable = false;
            ret.IsEnabled = true;
            ret.Click += (s, e) => RunOnVisualTreeSearch.Run((DependencyObject)s, 
              label.ToString()+"LinkClicked", new object[] {e}, out var _);
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