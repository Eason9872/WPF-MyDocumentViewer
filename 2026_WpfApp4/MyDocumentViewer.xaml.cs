using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace _2026_WpfApp4
{
    public partial class MyDocumentViewer : Window
    {
        private bool ready, updatingControls, modified;
        private string? currentFile;

        public MyDocumentViewer()
        {
            InitializeComponent();
            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(font => font.Source);
            FontFamilyComboBox.SelectedItem = new FontFamily("Microsoft JhengHei");
            FontSizeComboBox.ItemsSource = new double[] { 8, 10, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };
            FontSizeComboBox.SelectedItem = 16.0;
            ready = true;
            UpdateStatus();
            MainRichTextBox.Focus();
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new MyDocumentViewer().Show();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "文件 (*.rtf;*.txt)|*.rtf;*.txt" };
            if (dialog.ShowDialog(this) != true || !ConfirmSaveChanges()) return;
            try
            {
                // 讀取成功後才取代目前文件，讀取失敗時保留原內容。
                var document = new FlowDocument { FontFamily = new FontFamily("Microsoft JhengHei"), FontSize = 16 };
                var range = new TextRange(document.ContentStart, document.ContentEnd);
                using (var stream = File.OpenRead(dialog.FileName))
                    range.Load(stream, GetDataFormat(dialog.FileName));
                MainRichTextBox.Document = document;
                currentFile = dialog.FileName;
                modified = false;
                UpdateStatus();
                MainRichTextBox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "無法開啟文件：" + ex.Message, "開啟失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDocument();
        }

        private bool SaveDocument()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "RTF 文件（保留格式）|*.rtf|純文字（不保留格式）|*.txt",
                DefaultExt = ".rtf",
                FileName = currentFile == null ? "未命名.rtf" : Path.GetFileName(currentFile),
                FilterIndex = currentFile != null && GetDataFormat(currentFile) == DataFormats.Text ? 2 : 1
            };
            if (currentFile != null) dialog.InitialDirectory = Path.GetDirectoryName(currentFile);
            if (dialog.ShowDialog(this) != true) return false;
            try
            {
                var range = new TextRange(MainRichTextBox.Document.ContentStart, MainRichTextBox.Document.ContentEnd);
                // 先轉換到記憶體，避免轉換失敗時清空原檔。
                using var buffer = new MemoryStream();
                range.Save(buffer, GetDataFormat(dialog.FileName));
                File.WriteAllBytes(dialog.FileName, buffer.ToArray());
                currentFile = dialog.FileName;
                modified = false;
                UpdateStatus();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "無法儲存文件：" + ex.Message, "儲存失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static string GetDataFormat(string fileName)
        {
            return Path.GetExtension(fileName).Equals(".txt", StringComparison.OrdinalIgnoreCase)
                ? DataFormats.Text : DataFormats.Rtf;
        }

        private bool ConfirmSaveChanges()
        {
            if (!modified) return true;
            var result = MessageBox.Show(this, "文件尚未儲存，是否先儲存？", "WPF小作家",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            return result == MessageBoxResult.No || (result == MessageBoxResult.Yes && SaveDocument());
        }

        private void Window_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = !ConfirmSaveChanges();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "確定清空文件內容？可使用 Ctrl+Z 復原。", "清空文件",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            MainRichTextBox.SelectAll();
            MainRichTextBox.Selection.Text = "";
            MainRichTextBox.Focus();
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ready || updatingControls || FontFamilyComboBox.SelectedItem is not FontFamily font) return;
            MainRichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, font);
            MainRichTextBox.Focus();
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ready || updatingControls || FontSizeComboBox.SelectedItem is not double size) return;
            MainRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
            MainRichTextBox.Focus();
        }

        private void FontColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!ready || updatingControls || e.NewValue is not Color color) return;
            MainRichTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            MainRichTextBox.Focus();
        }

        private void MainRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ready) return;
            modified = true;
            UpdateStatus();
        }

        private void MainRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!ready) return;
            updatingControls = true;
            try
            {
                var selection = MainRichTextBox.Selection;
                BoldButton.IsChecked = Equals(selection.GetPropertyValue(TextElement.FontWeightProperty), FontWeights.Bold);
                ItalicButton.IsChecked = Equals(selection.GetPropertyValue(TextElement.FontStyleProperty), FontStyles.Italic);
                UnderlineButton.IsChecked = selection.GetPropertyValue(Inline.TextDecorationsProperty)
                    is TextDecorationCollection decorations && decorations.Any(item => item.Location == TextDecorationLocation.Underline);
                FontFamilyComboBox.SelectedItem = selection.GetPropertyValue(TextElement.FontFamilyProperty) is FontFamily font ? font : null;
                FontSizeComboBox.SelectedItem = selection.GetPropertyValue(TextElement.FontSizeProperty) is double size ? size : null;
                FontColorPicker.SelectedColor = selection.GetPropertyValue(TextElement.ForegroundProperty) is SolidColorBrush brush ? brush.Color : null;
            }
            finally
            {
                updatingControls = false;
            }
        }

        private void UpdateStatus()
        {
            string name = currentFile == null ? "未命名" : Path.GetFileName(currentFile);
            Title = "MyDocumentViewer - " + name + (modified ? " *" : "");
            StatusText.Text = name + (modified ? "｜尚未儲存" : "｜就緒");
        }
    }
}
