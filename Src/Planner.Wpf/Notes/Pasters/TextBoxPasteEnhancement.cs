using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NodaTime;

namespace Planner.Wpf.Notes.Pasters
{
    public static class TextBoxPasteEnhancement
    {
        public static readonly DependencyProperty MarkdownPasterProperty = DependencyProperty.RegisterAttached(
            "MarkdownPaster" ,typeof(IMarkdownPaster), typeof(TextBoxPasteEnhancement),
            new FrameworkPropertyMetadata(null, NewPaster));

        public static IMarkdownPaster GetMarkdownPaster(DependencyObject obj) =>
            (IMarkdownPaster) obj.GetValue(MarkdownPasterProperty);

        public static void SetMarkdownPaster(DependencyObject obj, IMarkdownPaster value) =>
            obj.SetValue(MarkdownPasterProperty, value);
        
        
        private static readonly DependencyProperty PasterDateProperty = DependencyProperty.RegisterAttached(
            "PasterDate", typeof(LocalDate), typeof(TextBoxPasteEnhancement),
            new PropertyMetadata(LocalDate.MinIsoValue));

        public static LocalDate GetPasterDate(DependencyObject obj) => 
            (LocalDate) obj.GetValue(PasterDateProperty);

        public static void SetPasterDate(DependencyObject obj, LocalDate value) =>
            obj.SetValue(PasterDateProperty, value);
        
        
        private static void NewPaster(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox box && e.NewValue is IMarkdownPaster mp)
            {
                SetupPasteEnhancement(box, mp);
            }
        }

        private static void SetupPasteEnhancement(TextBox box, IMarkdownPaster mp)
        {
            box.PreviewKeyDown += IsPasteKeyStroke;
            box.AllowDrop = true;
            box.PreviewDragOver += HandleDragOver;
            box.PreviewDrop += HandleDrop;
        }

        #region Handle Drop
            private static void HandleDrop(object sender, DragEventArgs e)
            {
                if (!(sender is TextBox tb)) return;
                DoPaste(tb, e.Data);
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }

            private static void HandleDragOver(object sender, DragEventArgs e)
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
                
        #endregion
        
        #region Handle Paste
        private static void IsPasteKeyStroke(object sender, KeyEventArgs e)
        {
            if (IsPasteKeyStroke(e) && sender is TextBox tb && Clipboard.GetDataObject() is {} dataObject)
            {
                DoPaste(tb, dataObject);
                e.Handled = true;
            }
        }

        private static bool IsPasteKeyStroke(KeyEventArgs e) => 
            e.Key == Key.V && e.KeyboardDevice.Modifiers == ModifierKeys.Control;

        private static async void DoPaste(TextBox ctrl, IDataObject dataObject)
        {
            if (await GetSpecialPasteString(ctrl, dataObject) is {} pastedText) PasteIntoTextBox(ctrl, pastedText);
        }

        private static void PasteIntoTextBox(TextBox ctrl, string pastedText)
        {
            ctrl.SelectedText = pastedText;
            (ctrl.SelectionLength, ctrl.SelectionStart) = (0, ctrl.SelectionStart + ctrl.SelectionLength);
        }

        private static ValueTask<string?> GetSpecialPasteString(TextBox ctrl, IDataObject dataObject) => 
            GetMarkdownPaster(ctrl).GetPasteText(dataObject, GetPasterDate(ctrl));
        #endregion 
    }
}